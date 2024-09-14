#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Client;
using HularionExperience.PackageAccess.Directory;
using HularionExperience.PackageBuilder;
using HularionExperience.PackageBuilder.Collectors;
using HularionExperience.PackageStores.ProjectDirectoryStore;
using HularionPlugin.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores.Embedded
{
    public class EmbeddedPackageStore : IPackageStore
    {

        /// <summary>
        /// The assemblies that contain packages using embedded files.
        /// </summary>
        public List<Assembly> AssembliesContainingPackages { get; set; } = new();

        /// <summary>
        /// (PackageKey, (Version, package))
        /// </summary>
        private Dictionary<string, Dictionary<string, PackageMeta>> embeddedPackages = new();

        /// <summary>
        /// iff true, any version will be matched on GetPackage. The first appearing version will be returned.
        /// </summary>
        public bool MatchAnyVersion { get; set; } = false;

        private Dictionary<string, Assembly> assemblies { get; set; } = new();

        /// <summary>
        /// Constructor. Add the assemblies to check for packages as embedded project files. Then, call Initialize();
        /// </summary>
        public EmbeddedPackageStore()
        {
            
        }

        /// <summary>
        /// Adds the assembly to check for packages.
        /// </summary>
        /// <param name="assembly">The assembly to check.</param>
        public void AddAssembly(Assembly assembly)
        {
            AssembliesContainingPackages.Add(assembly);
        }

        /// <summary>
        /// Adds the assembly to which the provided type belongs.
        /// </summary>
        /// <typeparam name="TypeInAssembly">A type in the assembly.</typeparam>
        public void AddTypeAssembly<TypeInAssembly>()
        {
            AssembliesContainingPackages.Add(typeof(TypeInAssembly).Assembly);
        }

        /// <summary>
        /// Adds the assembly to which the provided type belongs.
        /// </summary>
        /// <param name="typeInAssembly">A type in the assembly.</param>
        public void AddTypeAssembly(Type typeInAssembly)
        {
            AssembliesContainingPackages.Add(typeInAssembly.Assembly);
        }


        public void Initialize()
        {
            var packageBuilder = new HularionPackageBuilder();
            foreach (var assembly in AssembliesContainingPackages.Distinct())
            {
                var packages = packageBuilder.AssemblyToPackageTransform.Transform(assembly);
                foreach (var package in packages)
                {
                    if(String.IsNullOrWhiteSpace(package.Key) || String.IsNullOrWhiteSpace(package.Version))
                    {
                        continue;
                    }
                    if (!embeddedPackages.ContainsKey(package.Key))
                    {
                        embeddedPackages.Add(package.Key, new Dictionary<string, PackageMeta>());
                    }
                    embeddedPackages[package.Key][package.Version] = package;

                    foreach(var resource in package.ProjectResources.Plugin)
                    {
                        foreach(var resourceFile in resource.Resources)
                        {
                            assemblies[resourceFile.Key] = Assembly.Load(resourceFile.Blob);
                        }
                    }
                }                
            }
        }

        public ProvidePackageResult GetPackage(PackageIndicator indicator)
        {


            if (MatchAnyVersion)
            {
                if (String.IsNullOrWhiteSpace(indicator.PackageKey) || !embeddedPackages.ContainsKey(indicator.PackageKey) || embeddedPackages[indicator.PackageKey].Count() == 0)
                {
                    return new ProvidePackageResult()
                    {
                        PackageWasFound = false
                    };
                }
                var anyResult = new ProvidePackageResult()
                {
                    PackageWasFound = true,
                    Package = new ProvidedPackage()
                };
                anyResult.Package.Partial = embeddedPackages[indicator.PackageKey].First().Value.CreatePartial();
                AddRoutes(anyResult.Package);
                return anyResult;
            }


            if (String.IsNullOrWhiteSpace(indicator.PackageKey) || String.IsNullOrWhiteSpace(indicator.Version))
            {
                return new ProvidePackageResult()
                {
                    PackageWasFound = false
                };
            }

            if(!embeddedPackages.ContainsKey(indicator.PackageKey) || !embeddedPackages[indicator.PackageKey].ContainsKey(indicator.Version))
            {
                return new ProvidePackageResult()
                {
                    PackageWasFound = false
                };
            }


            var result = new ProvidePackageResult()
            {
                PackageWasFound = true,
                Package = new ProvidedPackage()
            };

            result.Package.Partial = embeddedPackages[indicator.PackageKey][indicator.Version].CreatePartial();
            AddRoutes(result.Package);

            return result;

        }

        private void AddRoutes(ProvidedPackage package)
        {

            try
            {
                foreach (var provider in package.Partial.ServerPackage.RouteProviders)
                {
                    try
                    {
                        var assembly = assemblies[provider.FilePath];
                        var assemblyTypes = assembly.GetTypes().ToDictionary(x => x.FullName, x => x);
                        var prefixedProvider = new RouterProvider();
                        foreach (var providerType in provider.ProviderTypes)
                        {
                            if (!assemblyTypes.ContainsKey(providerType))
                            {
                                continue;
                            }
                            var routeProvider = (IRouteProvider)assembly.CreateInstance(providerType);
                            package.RouteProviders.Add(routeProvider);
                        }
                    }
                    catch
                    {
                        //load issue.
                    }
                }

            }
            catch
            {
            }
        }
    }
}
