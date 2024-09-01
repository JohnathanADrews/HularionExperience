#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Identifier;
using HularionCore.Pattern.Topology;
using HularionExperience.Client;
using HularionExperience.Definition;
using HularionExperience.PackageBuilder;
using HularionPlugin.Route;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores.ProjectDirectoryStore
{
    public class ProjectFilePackageStore : IPackageStore
    {

        private List<ProjectLocator> locators = new List<ProjectLocator>();

        public Dictionary<ProjectLocator, List<string>> locatorDirectories = new Dictionary<ProjectLocator, List<string>>();

        private Dictionary<string, ProjectLocation> packageProviders = new Dictionary<string, ProjectLocation>();


        private const string projectEndsWith = ".hxproject";

        public ProjectFilePackageStore()
        {
            
        }


        public void AddLocator(ProjectLocator locator)
        {
            lock (locators)
            {
                locators.Add(locator);
                RefreshSearch(locator);
            }
        }

        public void AddLocators(params ProjectLocator[] locators)
        {
            foreach(var locator in locators)
            {
                AddLocator(locator);
            }
        }

        public void RefreshSearch(ProjectLocator locator)
        {
            IEnumerable<string> projectDirectories = new string[] { };
            switch (locator.SearchMethod)
            {
                case ProjectDirectorySearchMethod.ImmediateDirectory:
                    projectDirectories = SearchForProjectDirectories(locator.Directory, 0, locator.ExcludedDirectories);
                    break;
                case ProjectDirectorySearchMethod.LimitOneLevel:
                    projectDirectories = SearchForProjectDirectories(locator.Directory, 1, locator.ExcludedDirectories);
                    break;
                case ProjectDirectorySearchMethod.TraverseAllDescendents:
                    projectDirectories = SearchForProjectDirectories(locator.Directory, null, locator.ExcludedDirectories);
                    break;
            }

            var projects = projectDirectories.ToDictionary(x => x, x => Directory.GetFiles(x).Where(x => x.EndsWith(projectEndsWith)).FirstOrDefault())
                .Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);

            var builder = new HularionPackageBuilder();
            foreach(var project in projects)
            {
                var package = builder.DirectoryToHXPackageTransform.Transform(project.Key).FirstOrDefault();
                if(package.Key == null)
                {
                    continue;
                }
                packageProviders[package.Key] = new ProjectLocation()
                {
                    PackageKey = package.Key,
                    PackageDirectory = project.Key,
                    ProjectFilename = project.Value,
                    PackageProvider = new Func<HXPackage>(() => builder.DirectoryToHXPackageTransform.Transform(project.Key).FirstOrDefault())
                };
            }

        }

        private IEnumerable<string> SearchForProjectDirectories(string directory, int? limit, IEnumerable<string> directoryExclusions)
        {
            var root = FormatDirectory(directory);
            var traverser = new TreeTraverser<string>();
            var exclusions = new HashSet<string>(directoryExclusions.Select(x=> String.Format(@"\{0}", FormatDirectory(x))));
            var projectDirectories = new List<string>();

            traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, root, node =>
            {
                try
                {
                    if (Directory.GetFiles(node).Where(x => x.EndsWith(projectEndsWith)).Count() > 0)
                    {
                        projectDirectories.Add(node);
                        return new string[] { };
                    }

                    if(limit != null)
                    {
                        if(node.Where(x=>x == '\\').Count() >= limit)
                        {
                            return new string[] { };
                        }
                    }

                    var next = new List<string>();
                    var directories = Directory.GetDirectories(node).Select(x => FormatDirectory(x)).Where(x => !exclusions.Contains(x)).ToList();
                    foreach(var directory in directories)
                    {
                        var excluded = false;
                        foreach(var exclusion in exclusions)
                        {
                            if (directory.EndsWith(exclusion))
                            {
                                excluded = true;
                                break;
                            }
                        }
                        if (!excluded)
                        {
                            next.Add(directory);
                        }
                    }
                    return next.ToArray();
                }
                catch
                {
                    //issue accessing the directory.
                    return new string[] { };
                }
            }, true);

            return projectDirectories;
        }

        public string FormatDirectory(string directory)
        {
            return string.Format("{0}", directory).Trim(new char[] { ' ', '\t', '\n', '\r', '\\' }).ToLower();
        }


        public ProvidePackageResult GetPackage(PackageIndicator indicator)
        {
            var result = new ProvidePackageResult()
            {
                PackageWasFound = true
            };

            try
            {
                if (!packageProviders.ContainsKey(indicator.PackageKey))
                {
                    RefreshSearch(new ProjectLocator() 
                    { 
                        Directory = indicator.ProjectLocation, 
                        SearchMethod = ProjectDirectorySearchMethod.TraverseAllDescendents
                    });
                    if (!packageProviders.ContainsKey(indicator.PackageKey))
                    {
                        return new ProvidePackageResult()
                        {
                            PackageWasFound = false
                        };
                    }
                }
                var location = packageProviders[indicator.PackageKey];
                var hxPackage = location.PackageProvider();
                result.Package.Partial = hxPackage.GetPartial();
                result.Package.Partial.Version = String.IsNullOrWhiteSpace(indicator.Version) ? indicator.Version = ObjectKey.CreateUniqueTag() : indicator.Version;

                var serverDirectory = String.Format(@"{0}", location.PackageDirectory);
                foreach (var provider in result.Package.Partial.ServerPackage.RouteProviders)
                {
                    try
                    {
                        var path = string.Format(@"{0}\{1}", serverDirectory, provider.FilePath);
                        var assembly = Assembly.LoadFile(path);
                        var assemblyTypes = assembly.GetTypes().ToDictionary(x => x.FullName, x => x);
                        var prefixedProvider = new RouterProvider();
                        foreach (var providerType in provider.ProviderTypes)
                        {
                            if (!assemblyTypes.ContainsKey(providerType))
                            {
                                continue;
                            }
                            var routeProvider = (IRouteProvider)assembly.CreateInstance(providerType);
                            result.Package.RouteProviders.Add(routeProvider);
                        }
                    }
                    catch (Exception e)
                    {
                        //load issue.
                    }
                }
            }
            catch (Exception e)
            {
                result.PackageWasFound = false;
            }
            return result;
        }

        public bool HasPackage(PackageIndicator indicator)
        {
            var packageResult = GetPackage(indicator);
            return packageResult.PackageWasFound;
        }
    }
}
