#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder.Collectors
{
    internal class FileCollector : IResourceCollector
    {

        private string directory;
        string[] ignoredDirectories;
        string[] ignoredDirectorySuffixes;

        //private const string manifestName = "manifest.html";
        private const char fileDelimiter = '\\';

        public FileCollector(string directory, string[] ignoredDirectories = null, string[] ignoredDirectorySuffixes = null)
        {
            this.directory = directory;
            if (ignoredDirectories == null) { this.ignoredDirectories = new string[] { }; }
            else { this.ignoredDirectories = ignoredDirectories; }
            if (ignoredDirectorySuffixes == null) { this.ignoredDirectorySuffixes = new string[] { }; }
            else { this.ignoredDirectorySuffixes = ignoredDirectorySuffixes; }
        }

        public IDictionary<string, ResourceValue> CollectProjectResources()
        {
            var result = new Dictionary<string, ResourceValue>();
            var traverser = new TreeTraverser<string>();
            var directories = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, directory, node =>
            {
                var next = Directory.GetDirectories(node);
                if (ignoredDirectories.Length > 0)
                {
                    next = next.Where(x => ignoredDirectories.Contains(x)).ToArray();
                }
                if (ignoredDirectorySuffixes.Length > 0)
                {
                    var filtered = new List<string>();
                    foreach(var nextDirectory in next)
                    {
                        bool match = false;
                        foreach(var suffix in ignoredDirectorySuffixes)
                        {
                            if (nextDirectory.EndsWith(suffix))
                            {
                                match = true;
                                break;
                            }
                        }
                        if (!match) { filtered.Add(nextDirectory); }
                    }
                    next = filtered.ToArray();
                }
                return next;
            }, true);

            foreach (var directory in directories)
            {
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    var bytes = File.ReadAllBytes(file);
                    var resource = new ResourceValue() { Name = file, Found = true, FileByteContent = bytes };
                    result.Add(file, resource);
                }
            }

            var resourceDirectories = new List<string>();
            foreach (var item in result)
            {
                //if (item.Key.EndsWith(manifestName))
                //{
                //    var projectDirectory = item.Key.Substring(0, item.Key.LastIndexOf(manifestName));
                //    resourceDirectories.Add(projectDirectory);
                //}
                if (item.Key.EndsWith(HularionExperienceKeyword.HXProjectName.Value))
                {
                    //var projectDirectory = item.Key.Substring(0, item.Key.LastIndexOf(HularionExperienceKeyword.HXProjectName.Value));
                    var projectDirectory = item.Key.Substring(0, item.Key.LastIndexOf(@"\"));
                    resourceDirectories.Add(projectDirectory);
                }
            }
            foreach (var projectDirectory in resourceDirectories)
            {
                var projectResources = result.Where(x => x.Key.StartsWith(projectDirectory)).ToDictionary(x => x.Key, x => x.Value);
                foreach (var resource in projectResources)
                {
                    var nameIndex = resource.Key.LastIndexOf(fileDelimiter);
                    resource.Value.Name = resource.Key.Substring(nameIndex + 1, resource.Key.Length - nameIndex - 1);
                    resource.Value.Path = resource.Key.Substring(projectDirectory.Length + 1, resource.Key.Length - projectDirectory.Length - 1);
                    var directoryNameIndex = projectDirectory.Trim(fileDelimiter).LastIndexOf(fileDelimiter);
                    resource.Value.DirectoryName = projectDirectory.Substring(directoryNameIndex + 1, projectDirectory.Length - directoryNameIndex - 1);
                }
            }


            return result;
        }
    }
}
