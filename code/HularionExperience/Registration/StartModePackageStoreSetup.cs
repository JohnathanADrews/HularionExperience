#region License
/*
MIT License

Copyright (c) 2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.PackageStores.Composite;
using HularionExperience.PackageStores.Embedded;
using HularionExperience.PackageStores.ProjectDirectoryStore;
using HularionExperience.PackageStores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Registration
{
    /// <summary>
    /// Sets up package stores for startup modes for Debug (using the projects' directories) and Release (using embedded files) automatically.
    /// </summary>
    public class StartModePackageStoreSetup
    {

        private List<Assembly> projectAssemblies { get; set; } = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        public StartModePackageStoreSetup()
        {

        }

        /// <summary>
        /// Adds the calling assembly as a package source.
        /// </summary>
        /// <returns>this</returns>
        public StartModePackageStoreSetup AddCallerAssembly()
        {
            projectAssemblies.Add(Assembly.GetCallingAssembly());
            return this;
        }

        /// <summary>
        /// Adds the assembly as a package source.
        /// </summary>
        /// <param name="assembly">The assembly to add.</param>
        /// <returns>this</returns>
        public StartModePackageStoreSetup AddAssembly(Assembly assembly)
        {
            projectAssemblies.Add(assembly);
            return this;
        }

        /// <summary>
        /// Adds the assembly in which memberType is defined. Defining a type in the assmbly just for this purpose may be helpful.
        /// </summary>
        /// <typeparam name="MemberType">The type belonging to the assembly to add.</typeparam>
        /// <returns>this</returns>
        public StartModePackageStoreSetup AddAssembly<MemberType>()
        {
            var type = typeof(MemberType);
            projectAssemblies.Add(type.Assembly);
            return this;
        }

        /// <summary>
        /// Adds the assembly in which memberType is defined. Defining a type in the assmbly just for this purpose may be helpful.
        /// </summary>
        /// <param name="memberType">The type belonging to the assembly to add.</param>
        /// <returns>this</returns>
        public StartModePackageStoreSetup AddAssembly(Type memberType)
        {
            projectAssemblies.Add(memberType.Assembly);
            return this;
        }

        /// <summary>
        /// Creates the package store using the provided assemblies.
        /// </summary>
        /// <returns>The package store.</returns>
        public IPackageStore GetPackageStore()
        {
            var compositeProvider = new CompositePackageStore();


            var runDebug = new Action(() =>
            {
                var projectProvider = new DevProjectFileProvider();
                List<string> projectDirectories = new();
                foreach (var assembly in projectAssemblies)
                {
                    projectDirectories.AddRange(projectProvider.LocateHXFromCSharpProject(assembly));
                }
                var projectLocators = projectDirectories.Distinct().Select(x => new ProjectLocator()
                {
                    Directory = x,
                    SearchMethod = ProjectDirectorySearchMethod.ImmediateDirectory
                }).ToArray();
                var fileProjectStore = new ProjectFilePackageStore();
                fileProjectStore.AddLocators(projectLocators);
                compositeProvider.AddPackageStore(fileProjectStore);
            });


            var runRelease = new Action(() =>
            {
                var projectProvider = new EmbeddedPackageStore();
                foreach (var assembly in projectAssemblies)
                {
                    projectProvider.AddAssembly(assembly);
                }
                projectProvider.MatchAnyVersion = true;
                projectProvider.Initialize();
                compositeProvider.AddPackageStore(projectProvider);
            });

            if (System.Diagnostics.Debugger.IsAttached)
            {
                runDebug();
            }
            else
            {
                runRelease();
            }

            return compositeProvider;
        }


    }
}
