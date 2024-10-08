﻿#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Boot
{
    /// <summary>
    /// Contains information about the resources required to boot the HX kernel in the browser.
    /// </summary>
    public class BootResources : IBootResources, IKernelResources
    {
        private Assembly assembly;
        private Dictionary<string, string> resources = new();
        private string prefix;
        private string[] resourcesNames;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">The decimal delimited project prefix containing the boot loader file. (e.g. wwwroot.js.hx)</param>
        public BootResources(string prefix)
        {
            assembly = Assembly.GetExecutingAssembly();
            this.prefix = prefix;

            Refresh();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prefix">The decimal delimited project prefix containing the boot loader file. (e.g. wwwroot.js.hx)</param>
        /// <param name="assembly">The assembly containing the boot loader file.</param>
        public BootResources(string prefix, Assembly assembly)
        {
            this.assembly = assembly;
            this.prefix = prefix;

            Refresh();
        }

        private void Refresh()
        {
            resourcesNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourcesNames)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new(stream))
                {
                    var content = reader.ReadToEnd();
                    resources[resourceName] = content;
                }
            }
        }


        public virtual string GetIndexPage()
        {
            return GetResource("Index.html");
        }


        public string GetHularionKernelBootLoaderScript()
        {
            return GetResource("HularionKernelBootLoader.js");
        }

        /// <summary>
        /// iframe for the kernel if it loads in iframe mode.
        /// </summary>
        /// <returns></returns>
        public string GetIFrameKernelLoader()
        {
            return GetResource("IFrameKernelLoader.html");
        }

        /// <summary>
        /// iframe for script sets.
        /// </summary>
        /// <returns></returns>
        public string GetIFrameLoader()
        {
            return GetResource("IFrameLoader.html");
        }

        /// <summary>
        /// Gets the kernel loader js.
        /// </summary>
        /// <returns></returns>
        public string GetKernelLoader()
        {
            return GetResource("HXKernelBootLoader.js");
        }

        public IDictionary<string, string> GetKernelPackage()
        {
            return resources.Where(x => x.Key.Contains("Packages.Kernel")).ToDictionary(x => x.Key, x => x.Value);
        }

        public string GetResource(string endsWith)
        {
            endsWith = string.Format("{0}.{1}", prefix, endsWith);
            var content = resources.Where(x => x.Key.EndsWith(endsWith)).Select(x => x.Value).FirstOrDefault();
            return content;
        }
    }

}
