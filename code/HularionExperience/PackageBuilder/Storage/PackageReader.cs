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
using HularionExperience.Definition.Package;
using HularionExperience.Definition.PackageClient;
using HularionExperience.Definition.PackageServer;
using HularionText.Language.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder.Storage
{
    public class PackageReader
    {
        JsonSerializer serializer = new JsonSerializer();

        public HXPackage ReadPackage(string filename)
        {
            var package = new HXPackage();
            var content = File.ReadAllText(filename);

            var end = GetEndSegment(0, content);
            var marker = content.Substring(0, end);
            //For now, this should always be PackageHeaderVersion1.Marker. add interface to handle many versions.

            if(marker != PackageHeader.Marker)
            {
                throw new PackageStorageException(String.Format("Error reading package. Marker '{0}' is unknown", marker));
            }

            var start = end + 1;
            end = GetEndSegment(start, content);
            var headerJson = content.Substring(start, end - start);

            start = end + 1;
            end = GetEndSegment(start, content);
            var clientPartialJson = content.Substring(start, end - start);
            package.ClientPackage = serializer.Deserialize<ClientPackage>(clientPartialJson);

            start = end + 1;
            end = GetEndSegment(start, content);
            var serverPartialJson = content.Substring(start, end - start);
            package.ServerPackage = serializer.Deserialize<ServerPackage>(serverPartialJson);

            start = end + 1;
            end = GetEndSegment(start, content);
            var serverBinaryJson = content.Substring(start, end - start);
            package.ServerBinaryPackage = serializer.Deserialize<ServerBinaryPackage>(serverBinaryJson);


            package.Key = package.ClientPackage.Key;
            package.Version = package.ClientPackage.Version;
            package.Name = package.ClientPackage.Name;

            return package;

        }

        public PackageSummary GetPackageSummary(string filename)
        {
            var buffer = new byte[4096];
            int start = 0;
            int readCount = 0;
            bool markerFound = false;
            int readHead = 0;
            bool headerFound = false;
            var header = new StringBuilder();
            using(var stream = new FileStream(path: filename, mode: FileMode.Open))
            {
                stream.Position = 0;
                for (;;)
                {
                    readCount = stream.Read(buffer, start, buffer.Length);
                    var content = Encoding.UTF8.GetString(buffer, start, readCount);
                    readHead = 0;
                    if (!markerFound)
                    {
                        for (var i = 0; i < content.Length; i++)
                        {
                            if (content[i] == '\n')
                            {
                                if (content.Substring(0, i) != PackageHeader.Marker)
                                {
                                    throw new PackageStorageException("Attempted to read the package summary for package '{0}', but the file type was not recognized.");
                                }
                                markerFound = true;
                                readHead = i + 1;
                                break;
                            }
                        }
                        if (!markerFound)
                        {
                            throw new PackageStorageException("Attempted to read the package summary for package '{0}', but the header was not found.");
                        }
                    }

                    for (var i = readHead; i < content.Length; i++)
                    {
                        if (content[i] == '\n')
                        {
                            headerFound = true;
                            header.Append(content.Substring(readHead, i - readHead));
                            break;
                        }
                    }
                    if (headerFound)
                    {
                        break;
                    }
                    else
                    {
                        header.Append(content.Substring(readHead, content.Length - readHead));
                    }
                    if (readCount < buffer.Length)
                    {
                        break;
                    }
                }
                start += readCount;
            }
            if (!markerFound)
            {
                throw new PackageStorageException("Attempted to read the package summary for package '{0}', but it was not found.");
            }

            var headerJson = header.ToString();
            var summary = serializer.Deserialize<PackageHeader>(headerJson);

            return summary.PackageSummary;
        }


        private int GetEndSegment(int start, string content)
        {
            int end = start;
            for (var i = start; i < content.Length; i++)
            {
                end = i;
                if (content[i] == '\n')
                {
                    return end;
                }
            }
            return end;
        }


    }
}
