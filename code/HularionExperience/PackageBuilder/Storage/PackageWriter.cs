#region License
/*
MIT License

Copyright (c) 2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition;
using HularionText.Language.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder.Storage
{
    public class PackageWriter
    {

        public PackageWriter()
        {

        }

        public void WritePackage(string outputDirectory, HXPackage package, bool overwrite = false)
        {

            var serializer = new JsonSerializer();

            var clientPartial = serializer.Serialize(package.ClientPackage, JsonSerializationSpacing.Minimized);
            var serverPartial = serializer.Serialize(package.ServerPackage, JsonSerializationSpacing.Minimized);
            var serverBinary = serializer.Serialize(package.ServerBinaryPackage, JsonSerializationSpacing.Minimized);

            var header = new PackageHeader();
            header.ClientPartialLength = clientPartial.Length;
            header.ServerPartialLength = serverPartial.Length;
            header.ServerBinaryLength = serverBinary.Length;
            header.PackageSummary = package.PackageSummary;
            var headerJson = serializer.Serialize(header, JsonSerializationSpacing.Minimized);

            var content = String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n", PackageHeader.Marker, headerJson, clientPartial, serverPartial, serverBinary);
            var filename = String.Format(@"{0}\{1}", outputDirectory, HularionExperienceKeyword.MakePackageFilename(package.Key, package.Version));
            if (File.Exists(filename) && !overwrite)
            {
                throw new PackageStorageException(String.Format("Filename {0} already exists.", filename));
            }
            File.WriteAllBytes(filename, Encoding.UTF8.GetBytes(content));

        }


    }
}
