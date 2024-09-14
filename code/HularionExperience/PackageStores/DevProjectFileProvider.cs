#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores
{
    /// <summary>
    /// Provides project files given an assembly. This assumes project folder names match assembly names.
    /// </summary>
    public class DevProjectFileProvider
    {
        private const string projectEndsWith = ".hxproject";

        /// <summary>
        /// Gets the directories containing HX projects given a type in the c# project's assembly.
        /// </summary>
        /// <typeparam name="TypeInAssembly">A type in the target assembly.</typeparam>
        /// <returns>The directories in the C# project that contain HX projects.</returns>
        public string[] LocateHXFromCSharpType<TypeInAssembly>()
        {
            return LocateHXFromCSharpType(typeof(TypeInAssembly));
        }

        /// <summary>
        /// Gets the directories containing HX projects given a type in the c# project's assembly.
        /// </summary>
        /// <param name="type">A type in the target assembly.</param>
        /// <returns>The directories in the C# project that contain HX projects.</returns>
        public string[] LocateHXFromCSharpType(Type type)
        {
            return LocateHXFromCSharpProject(type.Assembly);
        }

        /// <summary>
        /// Gets the directories containing HX projects given the c# project's assembly.
        /// </summary>
        /// <param name="assembly">The assembly used as a basis to search for HX projects.</param>
        /// <returns>The directories in the C# project that contain HX projects.</returns>
        public string[] LocateHXFromCSharpProject(Assembly assembly)
        {
            var entryName = Assembly.GetEntryAssembly().GetName().Name;
            var baseDirectory = assembly.Location.Substring(0, assembly.Location.IndexOf(entryName));
            var files = assembly.GetManifestResourceNames().Where(x => x.EndsWith(projectEndsWith)).ToList();
            var directories = files.Select(x => baseDirectory + x.Substring(0, x.Length - projectEndsWith.Length)).ToList();
            directories = directories.Select(x => x.Substring(0, x.LastIndexOf(".")).Replace(".", @"\")).ToList();
            return directories.ToArray();
        }

    }
}
