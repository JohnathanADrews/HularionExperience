#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.PackageServer
{
    public class ServerSourcePackage
    {

        public string Key { get { return ServerPackage.Key; } set { ServerPackage.Key = value; ServerBinaryPackage.Key = value; } }

        public string Name { get { return ServerPackage.Name; } set { ServerPackage.Name = value; ServerBinaryPackage.Name = value; } }

        public string Version { get { return ServerPackage.Version; } set { ServerPackage.Version = value; ServerBinaryPackage.Version = value; } }

        public ServerPackage ServerPackage { get; set; } = new ServerPackage();

        public ServerBinaryPackage ServerBinaryPackage { get; set; } = new ServerBinaryPackage();



        public ServerSourcePackage()
        {
                
        }

        public ServerSourcePackage(HularionProject project)
        {

            Key = project.Key;
            Name = project.Name;
            Version = project.Version;

            ServerBinaryPackage.ServerFiles = project.ServerFiles.Select(x => new ServerFile(x)).ToList();
            ServerPackage.RouteProviders = project.ServerRouteProviders.Select(x => new ServerRouteProviderFile(x)).ToList();

        }
    }
}
