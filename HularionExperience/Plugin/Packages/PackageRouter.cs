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
using HularionCore.Pattern.Topology;
using HularionExperience.Definition;
using HularionExperience.PackageBuilder;
using HularionExperience.Plugin.Packages.Routes.Request;
using HularionExperience.Plugin.Packages.Routes.Response;
using HularionPlugin.Route;
using HularionText.Language.Json;
using HularionText.StringCase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HularionExperience.PackageBuilder.Storage;
using HularionExperience.PackageStores;
using HularionExperience.PackageInstallers.DirectoryInstaller;
using HularionExperience.PackageStores.DirectoryStore;

namespace HularionExperience.Plugin.Packages
{
    public class PackageRouter : IRouteProvider
    {
        public string Name => "Package Provider";

        public string Key => "HularionExperience.Package";

        public string Purpose => "Provides package resources to Hularion Experience clients.";

        public IEnumerable<HularionRoute> Routes => routes;

        private List<HularionRoute> routes { get; set; } = new List<HularionRoute>();

        public string BaseRoute { get; private set; } = String.Format("{0}/package/", HularionExperienceKeyword.BaseRoute);

        JsonSerializer serializer = new(new StringCaseModifier());

        public PackageRouter(PackageLoader packageManager)
        {
            //Route = string.Format("{0}get", BaseRoute),
            routes.Add(new HularionRoute<PackageRequest, PackageResponse>()
            {
                Name = "Get Package",
                Route = string.Format("{0}get", BaseRoute),
                Usage = "Finds the indicated package and returns it if found.",
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<PackageRequest>, RoutedResponse<PackageResponse>>(request =>
                {
                    var response = request.CreateResponse<PackageResponse>();
                    var package = packageManager.LoadPackage(request.Detail.Indicator);
                    if(package == null)
                    {
                        response.IsFailure = true;
                        response.Messages.Add(new RoutedResponseMessage() 
                        { 
                            IsError = true, 
                            Header = "Package Not Found", 
                            Message = "The package was not found.", 
                            Type = RoutedResponseMessageType.Error 
                        });
                        return response;
                    }
                    var client = package.ClientPackage;
                    response.Detail.Package = client;
                    return response;
                })
            });

            //Route = string.Format("{0}build", BaseRoute),
            routes.Add(new HularionRoute<PackageBuildRequest, PackageBuildResponse>()
            {
                Name = "Build Package",
                Route = string.Format("{0}build", BaseRoute),
                Usage = "Builds a package from project files and saves the package where indicated..",
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<PackageBuildRequest>, RoutedResponse<PackageBuildResponse>>(request =>
                {
                    var response = request.CreateResponse<PackageBuildResponse>();
                    var packageBuilder = new HularionPackageBuilder();
                    var outputLocation = String.Format("{0}", request.Detail.OutputLocation).Trim();
                    var packageWriter = new PackageWriter();
                    var packages = packageBuilder.DirectoryToHXPackageTransform.Transform(request.Detail.ProjectLocation);
                    foreach(var package in packages)
                    {
                        try
                        {
                            packageWriter.WritePackage(outputLocation, package, overwrite: request.Detail.Overwrite);
                        }
                        catch(Exception e)
                        {
                            response.Messages.Add(new RoutedResponseMessage()
                            {
                                IsError = true,
                                Message = String.Format("An exception occurred creating package '{0}'. {1}", package.Key, e.Message)
                            });
                        }
                    }
                    return response;
                })
            });


        }

    }
}
