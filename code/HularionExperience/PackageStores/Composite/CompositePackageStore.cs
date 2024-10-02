#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores.Composite
{
    /// <summary>
    /// A composite of package stores.
    /// </summary>
    public class CompositePackageStore : IPackageStore
    {
        private List<IPackageStore> packageStores = new();

        /// <summary>
        /// Adds the package store to this store.
        /// </summary>
        /// <param name="store">The package store to add.</param>
        public void AddPackageStore(IPackageStore store)
        {
            lock (packageStores)
            {
                packageStores.Add(store);
            }
        }

        /// <summary>
        /// Adds the package stores to this store.
        /// </summary>
        /// <param name="stores">The package stores to add.</param>
        public void AddPackageStores(params IPackageStore[] stores)
        {
            foreach(var store in stores)
            {
                AddPackageStore(store);
            }
        }

        /// <summary>
        /// Uses the indicator to locate the package, returning the first match.
        /// </summary>
        /// <param name="indicator">The indicator used to find the package.</param>
        /// <returns>The first matching package.</returns>
        public ProvidePackageResult GetPackage(PackageIndicator indicator)
        {
            foreach (var store in packageStores)
            {
                var storeResult = store.GetPackage(indicator);
                if (storeResult.PackageWasFound)
                {
                    return storeResult;
                }
            }
            return new ProvidePackageResult()
            {
                PackageWasFound = false
            };
        }
    }
}
