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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder
{

    /// <summary>
    /// The value of a resource.
    /// </summary>
    public class ResourceValue
    {

        /// <summary>
        /// The key of the resource.
        /// </summary>
        //public ResourceKey Key { get; set; }

        /// <summary>
        /// The key of the resource.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the package directory.
        /// </summary>
        public string DirectoryName { get; set; }

        /// <summary>
        /// The localized path of the resource.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The content of the resource file.
        /// </summary>
        public string FileStringContent { get; set; }

        /// <summary>
        /// The content of the resource file.
        /// </summary>
        public byte[] FileByteContent { get; set; }

        /// <summary>
        /// The content provider of the resource.
        /// </summary>
        public IProvider<string> FileStringProvider { get; set; }

        /// <summary>
        /// The content provider of the resource.
        /// </summary>
        public IProvider<byte[]> FileByteProvider { get; set; }

        /// <summary>
        /// True iff the resource was found.
        /// </summary>
        public bool Found { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceValue()
        {
            //Default the providers to the content. Provider can be also be set by creator.
            FileStringProvider = new ProviderFunction<string>(() => FileStringContent);
            FileByteProvider = new ProviderFunction<byte[]>(() => FileByteContent);
        }

    }
}
