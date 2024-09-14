#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionPlugin.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageStores.RouteInjector
{
    public class RouteInjectorPackageStore : IPackageStore
    {
        public IPackageStore PackageStore { get; set; }

        private Dictionary<string, Func<IRouteProvider[]>> routeProviderers = new();

        private Dictionary<string, Dictionary<string, Func<IRouteProvider[]>>> versionRouteProviderers = new();

        public RouteInjectorPackageStore()
        {
            
        }

        public RouteInjectorPackageStore(IPackageStore packageStore)
        {
            this.PackageStore = packageStore;    
        }

        public ProvidePackageResult GetPackage(PackageIndicator indicator)
        {
            var packageResult = PackageStore.GetPackage(indicator);

            if (routeProviderers.ContainsKey(indicator.PackageKey))
            {
                var providers = routeProviderers[indicator.PackageKey]();
                packageResult.Package.RouteProviders.AddRange(providers);

                if (!String.IsNullOrWhiteSpace(indicator.Version) 
                    && versionRouteProviderers.ContainsKey(indicator.PackageKey)
                    && versionRouteProviderers[indicator.PackageKey].ContainsKey(indicator.Version))
                {
                    var versionProviders = versionRouteProviderers[indicator.PackageKey][indicator.Version]();
                    packageResult.Package.RouteProviders.AddRange(versionProviders);
                }
            }

            return packageResult;
        }

        /// <summary>
        /// Adds a Func that provides a route to the package with the given key.
        /// </summary>
        /// <param name="packageKey">The package key.</param>
        /// <param name="routeProviderer">A Func that provides a route to the package with the given key.</param>
        public void AddRouteProvider(string packageKey, Func<IRouteProvider[]> routeProviderer)
        {
            routeProviderers[packageKey] = routeProviderer;
        }

        /// <summary>
        /// Adds a Func that provides a route to the package with the given key and version.
        /// </summary>
        /// <param name="packageKey">The package key.</param>
        /// <param name="version">The version.</param>
        /// <param name="routeProviderer">A Func that provides a route to the package with the given key and version.</param>
        public void AddVersionRouteProviderer(string packageKey, string version, Func<IRouteProvider[]> routeProviderer)
        {
            if (!versionRouteProviderers.ContainsKey(packageKey))
            {
                versionRouteProviderers.Add(packageKey, new Dictionary<string, Func<IRouteProvider[]>>());
            }
            versionRouteProviderers[packageKey][version] = routeProviderer;
        }
    }
}
