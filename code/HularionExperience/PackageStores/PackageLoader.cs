#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HularionText.Language.Json;
using HularionExperience.Definition;
using HularionExperience.Definition.PackageServer;
using HularionExperience.PackageBuilder;
using HularionCore.Pattern.Topology;
using HularionExperience.PackageBuilder.Storage;
using System.IO;
using HularionExperience.Definition.PackageClient;
using System.Reflection;
using HularionCore.Pattern.Identifier;
using HularionPlugin.Route;
using HularionExperience.Client;
using HularionExperience.PackageStores.Composite;

namespace HularionExperience.PackageStores
{
    public class PackageLoader
    {

        private CompositePackageStore compositeStore { get; set; } = new();

        private HularionRouter router;

        public PackageLoader(HularionRouter router)
        {
            this.router = router;
        }

        public void AddPackageStore(IPackageStore store)
        {
            lock (compositeStore)
            {
                compositeStore.AddPackageStore(store);
            }
        }

        public HXPartial LoadPackage(PackageIndicator indicator)
        {
            var getPackage = compositeStore.GetPackage(indicator);
            if (!getPackage.PackageWasFound)
            {
                return null;
            }

            var specification = new ClientSpecification(getPackage.Package.Partial.ClientPackage);
            specification.RouteKey = string.Format(@"{0}{1}", ObjectKey.CreateUniqueTag(), ObjectKey.CreateUniqueTag()).ToLower();

            //Remove later once client is using ClientSpecification instead of ClientPackage.
            specification.ClientPackage.RouteInformation.RouteKey = specification.RouteKey;

            var prefixedProvider = new RouterProvider();
            prefixedProvider.Key = String.Format("RouterProvider-{0}", specification.RouteKey);
            foreach (var routeProvider in getPackage.Package.RouteProviders)
            {
                prefixedProvider.AddRouteProvider(routeProvider, specification.RouteKey);
            }
            router.RegisterRouteProvider(prefixedProvider);

            return getPackage.Package.Partial;
        }
    }


}
