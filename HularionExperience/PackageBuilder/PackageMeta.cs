#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition;
using HularionExperience.Definition.Package;
using HularionExperience.Definition.PackageServer;
using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder
{
    public class PackageMeta
    {

        public string Name { get; set; }

        public string Version { get; set; }

        public string Key { get; set; }

        public string ProductKey { get; set; }

        public Assembly Assembly { get; set; }

        public string Directory { get; set; }

        public string Url { get; set; }

        public HularionProjectResources ProjectResources { get; set; }

        public HularionProject Project { get; set; }

        public HXPackage Package { get; set; }

        //public ClientPackage ClientPackage { get; set; }


        /// <summary>
        /// The source of the package.
        /// </summary>
        public PackageSourceType Source { get; set; }

        /// <summary>
        /// The form in which the package was delivered.
        /// </summary>
        public PackageSourceForm SourceForm { get; set; }


        public override string ToString()
        {
            return string.Format("Package: {0}", Key);
        }

        public HXPartial CreatePartial()
        {
            var partial = new HXPartial()
            {
                PackageKey = Key,
                Name = Name,
                Version = Version,
                ClientPackage = Package.ClientPackage,
                ServerPackage = Package.ServerPackage
            };

            return partial;
        }
    }

}
