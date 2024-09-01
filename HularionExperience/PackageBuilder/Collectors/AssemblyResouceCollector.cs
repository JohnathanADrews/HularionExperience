#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

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

namespace HularionExperience.PackageBuilder.Collectors
{
    /// <summary>
    /// Collects resources from embedded resource files.
    /// </summary>
    /// <remarks>By convention, each resource must have exactly one '.' in the name. "manifest.html" is fine. "manifest.one.htnl" is not fine.</remarks>
    public class AssemblyResouceCollector : IResourceCollector
    {

        private Assembly assembly;
        private string containsText;

        private const char fileSlash = '\\';
        private const char dot = '.';
        private const string packages = "HularionExperience.Packages";
        //private const string demoPackages = "HularionExperience.Demo.Packages";
        private const string demoPackages = "HXCore.Demo.Packages";

        private string projectFileName = HularionExperienceKeyword.HXProjectName.Value.ToLower();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <param name="containsText">If not null, only resources with names containing containsText will be extracted.</param>
        public AssemblyResouceCollector(Assembly assembly, string containsText = null)
        {
            this.assembly = assembly;
            this.containsText = containsText;
        }

        public IDictionary<string, ResourceValue> CollectProjectResources()
        {
            return CollectResources(packages);
        }

        public IDictionary<string, ResourceValue> CollectResources(string prefix)
        {
            var result = new Dictionary<string, ResourceValue>();
            var resourceNames = assembly.GetManifestResourceNames().Order().ToArray();

            var projectFiles = resourceNames.Where(x => x.ToLower().EndsWith(projectFileName)).ToDictionary(x=>x, x=>x.Substring(0, x.Length - projectFileName.Length));
            foreach(var file in projectFiles)
            {
                projectFiles[file.Key] = file.Value.Substring(0, file.Value.LastIndexOf("."));
            }
            var projectResources = projectFiles.Values.Distinct().Order().ToDictionary(x=>x, x => new List<string>());


            var startIndex = 0;
            foreach (var projectPrefix in projectResources.Keys)
            {
                var resources = projectResources[projectPrefix];
                var found = false;
                for(var i = startIndex; i < resourceNames.Length; i++)
                {
                    var resourceName = resourceNames[i];
                    if (resourceName.StartsWith(projectPrefix))
                    {
                        resources.Add(resourceName);
                        found = true;
                    }
                    else if(found)
                    {
                        startIndex = i;
                        break;
                    }
                }
            }

            foreach (var resource in projectResources)
            {
                foreach (var resourceName in resource.Value)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        stream.Position = 0;
                        stream.CopyTo(memoryStream);
                        var value = new ResourceValue()
                        {
                            Found = true,
                            FileByteContent = memoryStream.ToArray()
                        };
                        var newName = resourceName.Substring(resource.Key.LastIndexOf(dot) + 1);
                        var dotIndex = newName.IndexOf(dot);

                        value.DirectoryName = newName.Substring(0, dotIndex);
                        var newNewName = newName.Substring(dotIndex + 1);
                        if(newNewName.IndexOf(dot) >= 0)
                        {
                            newName = newNewName;
                        }

                        dotIndex = newName.LastIndexOf(dot);
                        value.Path = String.Format("{0}{1}{2}", newName.Substring(0, dotIndex).Replace(dot, fileSlash), dot, newName.Substring(dotIndex + 1, newName.Length - dotIndex - 1));
                        if (value.Path.Contains(fileSlash))
                        {
                            value.Name = value.Path.Substring(value.Path.LastIndexOf(fileSlash) + 1);
                        }
                        else
                        {
                            value.Name = value.Path;
                        }
                        result[resourceName] = value;
                    }
                }
            }

            return result;
        }
    }
}
