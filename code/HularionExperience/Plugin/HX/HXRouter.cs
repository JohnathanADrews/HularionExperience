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
using HularionExperience.PackageBuilder;
using HularionExperience.PackageBuilder.Collectors;
using HularionExperience.Plugin.HX.Routes.Request;
using HularionExperience.Plugin.HX.Routes.Response;
using HularionExperience.Plugin.Packages.Routes.Request;
using HularionExperience.Plugin.Packages.Routes.Response;
using HularionExperience.Registration;
using HularionPlugin.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Plugin.HX
{
    public class HXRouter : IRouteProvider
    {
        public string Name => "Provides HX Routes";

        public string Key => "HularionExperience.HXRouter";

        public string Purpose => "Provides HX resources to Hularion Experience clients.";

        public IEnumerable<HularionRoute> Routes => routes;

        private List<HularionRoute> routes { get; set; } = new List<HularionRoute>();

        public string BaseRoute { get; private set; } = string.Format("{0}/hx/", HularionExperienceKeyword.BaseRoute);

        public HXRouter(IBootResources bootResources, IHXScreenController screenController)
        {

            routes.Add(new HularionRoute<GetIndexPageRequest, GetIndexPageResponse>()
            {
                Name = "Get Index Page",
                Route = string.Format("{0}index/get", BaseRoute),
                Usage = "Gets the index page for hularion.",
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<GetIndexPageRequest>, RoutedResponse<GetIndexPageResponse>>(request =>
                {
                    var response = request.CreateResponse<GetIndexPageResponse>();
                    response.Detail.IndexPage = bootResources.GetIndexPage();
                    return response;
                })
            });

            routes.Add(new HularionRoute<ReloadHularionPlayerRequest, ReloadHularionPlayerResponse>()
            {
                Name = "Reload Hularion Player",
                Route = string.Format("{0}player/refresh", BaseRoute),
                Usage = "Reloads Hularion Player, closing all open applications.",
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<ReloadHularionPlayerRequest>, RoutedResponse<ReloadHularionPlayerResponse>>(request =>
                {
                    var response = request.CreateResponse<ReloadHularionPlayerResponse>();
                    screenController.ReloadPlayer();
                    return response;
                })
            });

        }

    }
}
