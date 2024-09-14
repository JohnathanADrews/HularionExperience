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
using HularionExperience.PackageBuilder.Collectors;
using HularionExperience.Definition.Package;
using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HularionExperience.Definition;
using HularionExperience.Definition.PackageServer;
using HularionExperience.PackageBuilder.ProjectResource;

namespace HularionExperience.PackageBuilder
{
    public class HularionPackageBuilder
    {

        #region EndToEnd

        /// <summary>
        /// Transforms the embedded resources within an assembly to a package.
        /// </summary>
        /// <remarks>Use for production-time package building.</remarks>
        public ITransform<Assembly, PackageMeta[]> AssemblyToPackageTransform { get; set; }

        /// <summary>
        /// Uses the assembly and embedded resources to discover the resource location on disk, and then converts the resources into a package.
        /// </summary>
        /// <remarks>Use for development-time package building to reload the package on refresh instead of re-launching the application.</remarks>
        public ITransform<Assembly, PackageMeta[]> AssemblyFilesToPackageTransform { get; set; }


        #endregion

        #region SingleStep

        // Source (assembly, file) -> HularionProjectResources -> HularionProject -> ClientPackage

        /// <summary>
        /// Gathers project files in the specified Assembly and transforms them into a project resources.
        /// </summary>
        public ITransform<Assembly, HularionProjectResources[]> AssemblyToResourcesTransform { get; set; }

        /// <summary>
        /// Gathers project files at the specified location and transforms them into a project resources.
        /// </summary>
        public ITransform<string, HularionProjectResources[]> DirectoryToResourcesTransform { get; set; }


        /// <summary>
        /// Transforms file resources into a project.
        /// </summary>
        public ITransform<HularionProjectResources, HularionProject> ResourcesToProjectTransform { get; set; }

        /// <summary>
        /// Transforms a project into a package.
        /// </summary>
        public ITransform<HularionProject, HXPackage> ProjectToPackageTransform { get; set; }

        /// <summary>
        /// Transforms the content of a directory into PackageMeta.
        /// </summary>
        public ITransform<string, PackageMeta[]> ResouresToPackageMetaTransform { get; set; }

        /// <summary>
        /// Transforms the content of a directory into HXPackage.
        /// </summary>
        public ITransform<string, HXPackage[]> DirectoryToHXPackageTransform { get; set; }

        #endregion


        private const string projectEndsWith = ".hxproject";
        private const char fileDelimiter = '\\';

        public HularionPackageBuilder()
        {
            AssemblyFilesToPackageTransform = Transform.Create<Assembly, PackageMeta[]>(assembly =>
            {

                var entryName = Assembly.GetEntryAssembly().GetName().Name;
                var baseDirectory = assembly.Location.Substring(0, assembly.Location.IndexOf(entryName));
                var files = assembly.GetManifestResourceNames().Where(x => x.EndsWith(projectEndsWith)).ToList();
                var directories = files.Select(x => baseDirectory + x.Substring(0, x.Length - projectEndsWith.Length)).ToList();
                directories = directories.Select(x => x.Substring(0, x.LastIndexOf(".")).Replace(".", @"\")).ToList();

                var result = new List<PackageMeta>();
                foreach (var directory in directories)
                {
                    var metas = DirectoryToResourcesTransform.Transform(directory).Select(x => new PackageMeta()
                    {                        
                        Assembly = assembly,
                        Directory = directory,
                        ProjectResources = x,
                        SourceForm = PackageSourceForm.Files,
                        Source = PackageSourceType.AssemblyToFiles
                    }).ToList();
                    foreach (var meta in metas)
                    {
                        meta.Project = ResourcesToProjectTransform.Transform(meta.ProjectResources);
                        meta.Package = ProjectToPackageTransform.Transform(meta.Project);
                        meta.Key = meta.Package.ClientPackage.Key;
                        meta.Name = meta.Package.ClientPackage.Name;
                        meta.ProductKey = meta.Project.ProductKey;
                    }

                    result.AddRange(metas);
                }
                return result.ToArray();
            });

            AssemblyToPackageTransform = Transform.Create<Assembly, PackageMeta[]>(assembly =>
            {
                var resources = AssemblyToResourcesTransform.Transform(assembly);
                var result = new List<PackageMeta>();
                foreach (var resource in resources)
                {
                    var project = ResourcesToProjectTransform.Transform(resource);
                    var meta = new PackageMeta()
                    {
                        Assembly = assembly,
                        ProjectResources = resource,
                        SourceForm = PackageSourceForm.Assemby,
                        Source = PackageSourceType.Assemby,
                        Project = project,
                        Package = ProjectToPackageTransform.Transform(project),
                        Key = project.Key,
                        Name = project.Name,
                        ProductKey = project.ProductKey,
                        Version = project.Version
                    };
                    result.Add(meta);
                }
                return result.ToArray();
            });

            DirectoryToResourcesTransform = Transform.Create<string, HularionProjectResources[]>(directory =>
            {
                var result = new List<HularionProjectResources>();
                var collector = new FileCollector(directory);
                var collection = collector.CollectProjectResources();


                var directories = collection.Where(x=> !String.IsNullOrWhiteSpace(x.Value.DirectoryName)).Select(x => x.Value.DirectoryName).Distinct().ToList();
                foreach(var projectDirectory in directories)
                {
                    var projectResources = collection.Where(x => x.Value.DirectoryName == projectDirectory).ToDictionary(x => x.Key, x => x.Value);
                    var extractor = new FileToResourcesCompiler(new DictionaryCollector(projectResources));
                    var resources = extractor.CompileProjectResouces();
                    resources.Location = directory;
                    result.Add(resources);
                }
                return result.ToArray();
            });

            AssemblyToResourcesTransform = Transform.Create<Assembly, HularionProjectResources[]>(assembly =>
            {
                var result = new List<HularionProjectResources>();
                var collector = new AssemblyResouceCollector(assembly);
                var collection = collector.CollectProjectResources();

                //collection = collection.Where(x => x.Key.EndsWith(manifestName)).ToDictionary(x => x.Key, x => x.Value);

                //var projects = collection.Where(x => x.Key.Contains(manifestName)).ToDictionary(x => x.Key.Substring(0, x.Key.LastIndexOf(manifestName)-1), x=>new Dictionary<string, ResourceValue>());
                var projectItems = collection.Where(x => x.Key.Contains(projectEndsWith)).Select(x => x.Key.Substring(0, x.Key.LastIndexOf(projectEndsWith))).ToList();
                var projects = new Dictionary<string, Dictionary<string, ResourceValue>>();

                foreach(var project in projectItems)
                {
                    projects.Add(project.Substring(0, project.LastIndexOf(".")), new Dictionary<string, ResourceValue>());
                }

                
                foreach (var resource in collection)
                {
                    foreach (var project in projects)
                    {
                        if (resource.Key.StartsWith(project.Key))
                        {
                            project.Value.Add(resource.Key, resource.Value);
                            break;
                        }
                    }
                }

                foreach (var project in projects)
                {
                    var extractor = new FileToResourcesCompiler(new DictionaryCollector(project.Value));
                    var resources = extractor.CompileProjectResouces();
                    result.Add(resources);
                    if(resources.ProjectFile == null)
                    {
                        extractor.CompileProjectResouces();
                    }
                }
                return result.ToArray();
            });

            ResourcesToProjectTransform = Transform.Create<HularionProjectResources, HularionProject>(resources =>
            {
                var projectTransform = new ResourcesToProjectTransform();
                var project = projectTransform.Transform(resources);
                return project;
            });

            ProjectToPackageTransform = Transform.Create<HularionProject, HXPackage>(project =>
            {
                var package = new HXPackage(project);
                return package;
            });

            ResouresToPackageMetaTransform = Transform.Create<string, PackageMeta[]>(directory =>
            {
                var metas = DirectoryToResourcesTransform.Transform(directory).Select(x => new PackageMeta()
                {
                    Directory = directory,
                    ProjectResources = x,
                    SourceForm = PackageSourceForm.Files,
                    Source = PackageSourceType.AssemblyToFiles
                }).ToList();
                foreach (var meta in metas)
                {
                    meta.Project = ResourcesToProjectTransform.Transform(meta.ProjectResources);
                    meta.Package = ProjectToPackageTransform.Transform(meta.Project);
                    meta.Key = meta.Package.ClientPackage.Key;
                    meta.Name = meta.Package.ClientPackage.Name;
                    meta.ProductKey = meta.Project.ProductKey;
                    meta.Version = meta.Project.Version;
                }
                return metas.ToArray();
            });

            DirectoryToHXPackageTransform = Transform.Create<string, HXPackage[]>(directory =>
            {
                var resources = DirectoryToResourcesTransform.Transform(directory);
                var projects = ResourcesToProjectTransform.Transform(resources);
                var packages = projects.Select(x => new HXPackage(x)).ToArray();
                return packages;
            });

        }

    }
}
