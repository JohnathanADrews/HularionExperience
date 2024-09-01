#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion


using HularionCore.Pattern.Functional;
using HularionCore.Pattern.Identifier;
using HularionCore.Pattern.Topology;
using HularionExperience.PackageBuilder.Collectors;
using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Drawing;

namespace HularionExperience.PackageBuilder
{
    /// <summary>
    /// Provides UI resources.
    /// </summary>
    public class FileToResourcesCompiler
    {

        private static string[] ignoredDirectoriesEnds = new string[] { @"\.vs", @"\.git", @"\bin", @"\obj" };
        private static string[] ignoredDirectoriesContains = ignoredDirectoriesEnds.Select(x => x + @"\").ToArray();

        private static string[] includedFileTypes = new string[] { ".js", ".css", ".html", ".png", ".svg" };


        private IResourceCollector resourceCollector;

        private char fileDelimiter = '\\';
        private char dot = '.';
        private string manifestName = HularionExperienceKeyword.HXProjectName.Value.ToLower();

        /// <summary>
        /// Provides the web resources for Hularion.
        /// </summary>
        public FileToResourcesCompiler(IResourceCollector resourceCollector)
        {
            this.resourceCollector = resourceCollector;
        }


        public HularionProjectResources CompileProjectResouces()
        {
            var resources = resourceCollector.CollectProjectResources();

            var html = ".html";
            var js = ".js";
            var css = ".css";
            var svg = ".svg";
            var png = ".png";
            var ttf = ".ttf";


            var byteContent = new HashSet<string>(new string[] { png });

            var project = new HularionProjectResources();
            var addSetItem = new Action<string, ResourceFile, List<ResourceSet>>((division, resource, sets)=>
            {
                if (sets.Where(x => x.Name == division).Count() == 0) { sets.Add(new ResourceSet() { Name = division }); }
                sets.Where(x => x.Name == division).First().Resources.Add(resource);
                if (resource.Filename.EndsWith(html)) 
                {
                    resource.Name = resource.Filename.Substring(0, resource.Filename.Length - html.Length);
                    if (resource.Name.Contains(fileDelimiter)) { resource.Name = resource.Name.Substring(resource.Name.LastIndexOf(fileDelimiter) + 1); }
                }
                if (resource.Filename.EndsWith(js)) { resource.Name = resource.Filename.Substring(0, resource.Filename.Length - js.Length); }
                if (resource.Filename.EndsWith(css)) { resource.Name = resource.Filename.Substring(0, resource.Filename.Length - css.Length); }
                if (resource.Filename.EndsWith(svg)) { resource.Name = resource.Filename.Substring(0, resource.Filename.Length - svg.Length); }
                if (resource.Filename.EndsWith(png)) 
                { 
                    resource.Name = resource.Filename.Substring(resource.Filename.LastIndexOf(fileDelimiter) + 1);
                    resource.Type = PackageFileType.Byte;
                    resource.Content = String.Format("data:image/{0};base64,{1}", png, Convert.ToBase64String(resource.Blob));

                }
                if (resource.Filename.EndsWith(ttf)) { resource.Name = resource.Filename.Substring(0, resource.Filename.Length - ttf.Length); }
            });
            var handlers = new Dictionary<string, Action<string, ResourceFile>>();
            handlers.Add(manifestName.ToLower(), new Action<string, ResourceFile>((division, resource) => {
                project.ProjectFile = resource;
            }));
            handlers.Add("Applications".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                project.Applications.Add(resource);
            }));
            handlers.Add("Configuration".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                project.Configurations.Add(resource);
            }));
            handlers.Add("ImportScriptSets".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.ImportScriptSets);
            }));
            handlers.Add("ScriptSets".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.ScriptSets);
            }));
            handlers.Add("PresenterSets".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.PresenterSets);
            }));
            handlers.Add("StyleSets".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.StyleSets);
            }));
            handlers.Add("Graphics".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.Graphics);
            }));
            handlers.Add("Fonts".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.Fonts);
            }));
            handlers.Add("Plugin".ToLower(), new Action<string, ResourceFile>((division, resource) =>
            {
                addSetItem(division, resource, project.Plugin);
            }));

            var setContent = new Action<ResourceValue, ResourceFile>((resource, resourceFile) =>
            {
                resourceFile.Key = resource.Path;
                if (resourceFile.Type == PackageFileType.Byte)
                {
                    resourceFile.Blob = resource.FileByteProvider.Provide();
                }
                else
                {
                    resourceFile.Content = System.Text.Encoding.UTF8.GetString(resource.FileByteProvider.Provide());
                }
            });

            foreach (var resource in resources)
            {

                var resourceFile = new ResourceFile();
                resourceFile.Blob = resource.Value.FileByteProvider.Provide();
                if (resource.Value.Name.EndsWith(manifestName))
                {
                    handlers[manifestName](null, resourceFile);
                    setContent(resource.Value, resourceFile);
                    continue;
                }

                if (String.IsNullOrWhiteSpace(resource.Value.Path))
                {
                    continue;
                }


                if (resource.Value.Path.StartsWith("HX"))
                {

                }


                var lower = resource.Value.Path.ToLower();


                resourceFile.FileExtension = lower.Substring(lower.LastIndexOf(dot) + 1);

                var index = lower.IndexOf(fileDelimiter);
                if (index <= 0)
                {
                    if (handlers.ContainsKey(lower)) { handlers[lower](null, resourceFile); }
                    setContent(resource.Value, resourceFile);
                    continue;
                }
                var prefix = lower.Substring(0, index);
                var suffix = lower.Substring(prefix.Length + 1);
                var index2 = lower.IndexOf(fileDelimiter, prefix.Length + 1);
                if (index2 <= 0)
                {
                    resourceFile.Filename = resource.Key;
                    if (handlers.ContainsKey(prefix)) { handlers[prefix](suffix, resourceFile); }
                    setContent(resource.Value, resourceFile);
                    continue;
                }
                var division = resource.Value.Path.Substring(index + 1, index2 - index - 1);
                resourceFile.Filename = resource.Value.Name;

                if (handlers.ContainsKey(prefix)) { handlers[prefix](division, resourceFile); }
                setContent(resource.Value, resourceFile);

            }

            return project;
        }

    }

}
