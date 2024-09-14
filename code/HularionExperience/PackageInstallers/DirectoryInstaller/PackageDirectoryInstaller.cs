#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.PackageBuilder.Storage;
using HularionExperience.PackageStores.DirectoryStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HularionCore.Pattern.Topology;
using HularionExperience.Definition.PackageClient;
using HularionExperience.Definition.PackageServer;
using HularionExperience.Definition;
using HularionText.Language.Json;
using HularionExperience.PackageAccess.Directory;

namespace HularionExperience.PackageInstallers.DirectoryInstaller
{
    public class PackageDirectoryInstaller
    {


        public string BaseDirectory { get; set; }

        private DirectoryFormat directoryFormat;

        private JsonSerializer serializer = new();

        public PackageDirectoryInstaller(string baseDirectory)
        {
            BaseDirectory = string.Format("{0}", baseDirectory).Trim(new char[] { ' ', '\t', '\n', '\r', '\\' });
            directoryFormat = new DirectoryFormat(BaseDirectory);
        }

        public PackageInstallResult InstallPackage(PackageInstallationDetail install)
        {
            var result = new PackageInstallResult();

            var packageReader = new PackageReader();
            var summary = packageReader.GetPackageSummary(install.Source.Location);
            if (install.Source.Location.EndsWith(HularionExperienceKeyword.HXPackageExtension.Value))
            {
                install.Source.Location = install.Source.Location.Substring(0, install.Source.Location.LastIndexOf(@"\"));
            }
            var dependencies = GetDependencies(summary.PackageKey, summary.Version, new PackageSource[] { install.Source });
            var failed = dependencies.Where(x => x.FailedToFind).ToList();
            if (failed.Count > 0)
            {
                result.WasSuccessful = false;
                var message = new StringBuilder();
                message.Append("Failed to find the following necessary packages");
                foreach (var item in failed)
                {
                    message.Append(item.GetLowerFilename());
                    message.Append("\n");
                }
                result.Message = message.ToString();
                return result;
            }


            foreach (var dependency in dependencies)
            {
                var packageDirectory = directoryFormat.GetPackageDirectory(dependency.Summary.PackageKey, dependency.Summary.Version);
                var sourceDirectory = string.Format(@"{0}\{1}", packageDirectory, HularionExperienceKeyword.HXSourcePackagesDirectory.Value);
                Directory.CreateDirectory(sourceDirectory);

                var packageLocation = string.Format(@"{0}\{1}", sourceDirectory, dependency.PackageFilename);
                if (File.Exists(packageLocation))
                {
                    result.WasAlreadyInstalled = true;
                }

                if (!result.WasAlreadyInstalled)
                {
                    File.Copy(dependency.Source, packageLocation);

                    var package = packageReader.ReadPackage(dependency.Source);
                    var partialDirectory = directoryFormat.GetPartialDirectory(dependency.PackageKey, dependency.PackageVersion);
                    Directory.CreateDirectory(partialDirectory);
                    var clientJson = serializer.Serialize(package.ClientPackage);
                    File.WriteAllText(string.Format(@"{0}\{1}.{2}", partialDirectory, HularionExperienceKeyword.HXClientPartialExtension.Value, HularionExperienceKeyword.HXPartialExtension.Value), clientJson);
                    var serverJson = serializer.Serialize(package.ServerPackage);
                    File.WriteAllText(string.Format(@"{0}\{1}.{2}", partialDirectory, HularionExperienceKeyword.HXServerPartialExtension.Value, HularionExperienceKeyword.HXPartialExtension.Value), serverJson);
                    var summaryJson = serializer.Serialize(summary);
                    File.WriteAllText(string.Format(@"{0}\{1}.{2}", partialDirectory, HularionExperienceKeyword.HXSummaryPartialExtension.Value, HularionExperienceKeyword.HXPartialExtension.Value), summaryJson);

                    var serverDirectory = string.Format(@"{0}\{1}", packageDirectory, HularionExperienceKeyword.HXServerPackagesDirectory.Value);
                    foreach (var file in package.ServerBinaryPackage.ServerFiles)
                    {
                        var serverFilename = string.Format(@"{0}\{1}", serverDirectory, file.Filename);
                        var fileDirectory = serverFilename.Substring(0, serverFilename.LastIndexOf(@"\"));
                        Directory.CreateDirectory(fileDirectory);
                        File.WriteAllBytes(serverFilename, file.Blob);
                    }
                }
            }

            return result;
        }


        public PackageInstallDependency[] GetDependencies(string packageKey, string packageVersion, PackageSource[] sources)
        {

            var packages = new Dictionary<string, string>();
            var sourceMap = new Dictionary<string, string>();
            foreach (var source in sources)
            {
                if (Directory.Exists(source.Location))
                {
                    var packageFiles = Directory.GetFiles(source.Location).Where(x => x.EndsWith(HularionExperienceKeyword.HXPackageExtension.Value)).Select(x => x.Trim().ToLower()).ToList();
                    foreach (var file in packageFiles)
                    {
                        var filename = file;
                        if (filename.Contains(@"\"))
                        {
                            filename = file.Substring(filename.LastIndexOf(@"\") + 1);
                        }
                        packages[filename] = file;
                        sourceMap[file] = filename;
                    }
                }
            }

            var dependency = new PackageInstallDependency()
            {
                PackageKey = packageKey,
                PackageVersion = packageVersion
            };

            var traverser = new TreeTraverser<PackageInstallDependency>();
            var packageReader = new PackageReader();

            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, dependency, node =>
            {
                if (!packages.ContainsKey(node.GetLowerFilename()))
                {
                    node.FailedToFind = true;
                    return node.Dependencies.ToArray();
                }
                var summary = packageReader.GetPackageSummary(packages[node.GetLowerFilename()]);
                node.PackageName = summary.Name;
                node.Summary = summary;
                node.Source = packages[node.GetLowerFilename()];
                var parent = node.Parent;
                while (parent != null)
                {
                    if (parent.GetLowerFilename() == node.GetLowerFilename())
                    {
                        node.HasCircularReference = true;
                        return node.Dependencies.ToArray();
                    }
                }
                node.Dependencies = summary.PackageDependencies.Select(x => new PackageInstallDependency()
                {
                    PackageKey = x.PackageKey,
                    PackageVersion = x.Version
                }).ToList();
                return node.Dependencies.ToArray();
            }, true);

            return plan;
        }

    }
}
