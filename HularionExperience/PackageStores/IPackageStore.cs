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
using HularionExperience.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores
{
    /// <summary>
    /// Provides packages from a package store
    /// </summary>
    public interface IPackageStore
    {
        /// <summary>
        /// Gets the package from the store or null if the package was not found.
        /// </summary>
        /// <param name="indicator">Indicates the package to locate.</param>
        /// <returns>The package or null if the package was not found.</returns>
        ProvidePackageResult GetPackage(PackageIndicator indicator);

        /// <summary>
        /// Determines whether the source contains the indicated package.
        /// </summary>
        /// <param name="indicator">Indicates how to locate the package.</param>
        /// <returns>true iff the source contains the package.</returns>
        //bool HasPackage(PackageIndicator indicator);

    }
}
