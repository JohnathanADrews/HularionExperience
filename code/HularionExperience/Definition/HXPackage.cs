#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Package;
using HularionExperience.Definition.PackageClient;
using HularionExperience.Definition.PackageServer;
using HularionExperience.Definition.Project.Concept;
using HularionExperience.PackageBuilder.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition
{
    public class HXPackage
    {
        /// <summary>
        /// The unique key of the package, including version number.
        /// </summary>
        public string Key { get; set; }

        public string Version { get; set; }

        public string Name { get; set; }

        public PackageSummary PackageSummary { get; set; }

        public ClientPackage ClientPackage { get; set; }

        //public ServerSourcePackage ServerSourcePackage { get; set; } = new ServerSourcePackage();

        public ServerPackage ServerPackage { get; set; } = new ServerPackage();

        public ServerBinaryPackage ServerBinaryPackage { get; set; } = new ServerBinaryPackage();


        public HXPackage()
        {
            
        }

        public HXPackage(HularionProject project)
        {
            Key = project.Key;
            Version = project.Version;
            Name = project.Name;
            PackageSummary = new PackageSummary(project);
            ClientPackage = new ClientPackage(project);
            ServerBinaryPackage.ServerFiles = project.ServerFiles.Select(x => new ServerFile(x)).ToList();
            ServerPackage.RouteProviders = project.ServerRouteProviders.Select(x => new ServerRouteProviderFile(x)).ToList();
        }


        public HXPartial GetPartial()
        {
            var partial = new HXPartial()
            {
                ClientPackage = ClientPackage,
                ServerPackage = ServerPackage
            };
            partial.PackageKey = Key;
            partial.Version = Version;
            partial.Name = Name;
            return partial;
        }
    }
}
