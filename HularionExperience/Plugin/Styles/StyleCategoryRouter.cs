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
//using HularionExperience.Plugin.Packages.Routes.Request;
//using HularionExperience.Plugin.Packages.Routes.Response;
using HularionExperience.Plugin.Styles.Routes.Request;
using HularionExperience.Plugin.Styles.Routes.Response;
using HularionPlugin.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Plugin.Styles
{

    public class StyleCategoryRouter : IRouteProvider
    {
        public string Name => "Style Category Selector";

        public string Key => "HularionExperience.StyleCategorySelector";

        public string Purpose => "Provides style category selection.";

        public IEnumerable<HularionRoute> Routes => routes;

        private List<HularionRoute> routes { get; set; } = new List<HularionRoute>();

        public string BaseRoute { get; private set; } = String.Format("{0}/style/", HularionExperienceKeyword.BaseRoute.Value);

        private string styleSelectRoute;

        private List<Func<StyleCategoryUpdatedNotifyRequest, StyleCategoryUpdatedNotifyResponse>> categoryUpdateHandlers = new();
        private List<Func<StyleCategorySelectRequest, StyleCategorySelectResponse>> categorySelectedHandlers = new();

        public StyleCategoryRouter()
        {
            routes.Add(new HularionRoute<StyleCategoryUpdatedNotifyRequest, StyleCategoryUpdatedNotifyResponse>()
            {
                Name = "Style Categories Updated Nofification",
                Route = string.Format("{0}category/update", BaseRoute),
                Usage = "Indicates that a style category has been updated.",
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<StyleCategoryUpdatedNotifyRequest>, RoutedResponse<StyleCategoryUpdatedNotifyResponse>>(request =>
                {
                    foreach (var handler in categoryUpdateHandlers)
                    {
                        var result = handler(request.Detail);
                    }
                    var response = request.CreateResponse<StyleCategoryUpdatedNotifyResponse>();
                    return response;
                })
            });

            styleSelectRoute = string.Format("{0}category/select", BaseRoute);
            routes.Add(new HularionRoute<StyleCategorySelectRequest, StyleCategorySelectResponse>()
            {
                Name = "Style Category Option Selected",
                Route = styleSelectRoute,
                Usage = "Indicates that a style category has been selected.",
                IsOpen = true,
                Handler = ParameterizedFacade.FromSingle<RoutedRequest<StyleCategorySelectRequest>, RoutedResponse<StyleCategorySelectResponse>>(request =>
                {
                    foreach (var handler in categorySelectedHandlers)
                    {
                        var result = handler(request.Detail);
                    }
                    var response = request.CreateResponse<StyleCategorySelectResponse>();
                    return response;
                })
            });


        }

        /// <summary>
        /// Adds a handler for the style category update notify route.
        /// </summary>
        /// <param name="handler">Handles a category update notify message.</param>
        public void SetCategoryUpdateRouteHandler(Func<StyleCategoryUpdatedNotifyRequest, StyleCategoryUpdatedNotifyResponse> handler)
        {
            categoryUpdateHandlers.Add(handler);
        }

        /// <summary>
        /// Adds a handler for the style category selected route.
        /// </summary>
        /// <param name="handler">Handles a category selected message.</param>
        public void SetCategorySelectRouteHandler(Func<StyleCategorySelectRequest, StyleCategorySelectResponse> handler)
        {
            categorySelectedHandlers.Add(handler);
        }


        public void ProcessCategoryChanged(HularionRouter router, string categoryName, string categorySelection)
        {
            var detail = new StyleCategorySelectRequest()
            {
                Category = categoryName,
                Option = categorySelection
            };
            var request = new RoutedRequest<StyleCategorySelectRequest>()
            {
                Route = styleSelectRoute,
                Detail = detail
            };
            router.ProcessRequest<StyleCategorySelectRequest, StyleCategorySelectResponse>(request);
        }
    }
}
