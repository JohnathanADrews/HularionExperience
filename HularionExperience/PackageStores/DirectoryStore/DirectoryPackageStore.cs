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
using HularionExperience.Definition.PackageClient;
using HularionExperience.Definition.PackageServer;
using HularionExperience.Definition;
using HularionExperience.PackageBuilder.Storage;
using HularionExperience.PackageBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HularionText.Language.Json;
using HularionPlugin.Route;
using HularionExperience.PackageAccess.Directory;

namespace HularionExperience.PackageStores.DirectoryStore
{
    public class DirectoryPackageStore : IPackageStore
    {

        public string BaseDirectory { get; set; }

        //private JsonSerializer serializer = new();
        private JsonSerializer serializer = new(new HularionText.StringCase.StringCaseModifier(HularionText.StringCase.StringCaseDefinition.StartLower));

        private DirectoryFormat directoryFormat;

        public DirectoryPackageStore(string baseDirectory)
        {
            BaseDirectory = string.Format("{0}", baseDirectory).Trim(new char[] { ' ', '\t', '\n', '\r', '\\' });
            directoryFormat = new DirectoryFormat(BaseDirectory);
        }


        public ProvidePackageResult GetPackage(PackageIndicator indicator)
        {
            var result = new ProvidePackageResult();
            var package = new ProvidedPackage();
            try
            {
                package.Partial = GetPartial(indicator);
                if (package == null)
                {
                    return new ProvidePackageResult() { PackageWasFound = false };
                }
                result.Package = package;
                result.PackageWasFound = true;
                var serverDirectory = directoryFormat.GetServerFilesDirectory(package.Partial.PackageKey, package.Partial.Version);
                foreach (var provider in package.Partial.ServerPackage.RouteProviders)
                {
                    try
                    {
                        var path = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, serverDirectory, provider.FilePath);
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
                result.PackageWasFound = false;
            }
            return result;
        }

        public HXPartial GetPartial(PackageIndicator indicator)
        {
            var version = directoryFormat.FormatVersion(indicator.Version);
            var partial = new HXPartial()
            {
                ClientPackage = GetClientPartial(indicator.PackageKey, version),
                ServerPackage = GetServerPartial(indicator.PackageKey, version)
            };
            partial.AssignValuesFromClient();
            return partial;
        }

        public ClientPackage GetClientPartial(string packageKey, string version)
        {
            var filename = directoryFormat.GetClientPartialFilename(packageKey, version);
            var json = File.ReadAllText(filename);
            var partial = serializer.Deserialize<ClientPackage>(json);
            return partial;
        }

        public ServerPackage GetServerPartial(string packageKey, string version)
        {
            var filename = directoryFormat.GetServerPartialFilename(packageKey, version);
            var json = File.ReadAllText(filename);
            var partial = serializer.Deserialize<ServerPackage>(json);
            return partial;
        }

    }
}
