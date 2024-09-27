/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function HKCRouter() {
    this.baseRoute = "hularion/";
    this.registrationRoute = this.baseRoute + "registration/register";
    this.clientRoutes = new Map();
    this.serverRoutes = new Map();
    this.routeMethods = new Map();
};
HKCRouter.prototype = {

    AddClientRoute: function (routeDefinition) {
        var t = this;
        var route = t.baseRoute + routeDefinition.route.trim("/");
        return t.clientRoutes.set(route, routeDefinition);
    },

    AddClientRoutes: function (routeDefinitions) {
        var t = this;
        var results = [];
        hularion.Control.ProcessArray(routeDefinitions, routeDefinition => {
            results.push(t.AddClientRoute(routeDefinition));
        });
        return results;
    },

    AddServerRoute: function (routeDefinition) {
        var t = this;
        var route = t.baseRoute + routeDefinition.route.trim("/");
        t.serverRoutes.set(route, routeDefinition);
        //console.log("HKCRouter.AddServerRoute - ",  t);
        hularion.communicator.request({
            key: hularion.keyProvider().toString(),
            route: t.registrationRoute,
            detail: {
                route: route,
                handler: routeDefinition.handler
            }
        });
    },

    AddServerRoutes: function (routeDefinitions) {
        var t = this;
        var results = [];
        hularion.Control.ProcessArray(routeDefinitions, routeDefinition => {
            results.push(t.AddServerRoute(routeDefinition));
        });
    },

    Request: function (request) {
        var t = this;

        //console.log("HKCRouter.Request - ", request, t);

        if (request.route == null) {
            //console.error("HKCRouter - The requested route was not provided.", request);
            return;
        }
        var route = request.route;
        var parameters = new Map();
        var parameterIndex = route.indexOf("?");
        if (parameterIndex >= 0) {
            var params = route.substring(parameterIndex + 1, route.length);
            route = route.substring(0, parameterIndex);
            var sets = params.split("&");
            sets.forEach(set => {
                var args = set.split("=");
                if (args.length == 2) {
                    parameters.set(args[0], args[1]);
                }
            });
            //console.log("HKCRouter - params", request.route, route, params, parameters);
        }


        if (t.clientRoutes.has(route)) {
            var newRequest = {
                route: route,
                original: request.route,
                parameters: parameters,
                detail: request.detail
            }
            return t.clientRoutes.get(route).handler(newRequest);
        }
        else if (t.serverRoutes.has(route)) {

            //console.log("HKCRouter.Request - serverRoutes - ", t.serverRoutes.get(request.route));
            return t.serverRoutes.get(route).handler(request.detail);
        }
        else {
            return hularion.communicator.request(request);
        }

        //if (request.route == t.registrationRoute) {
        //    t.AddRoute({

        //    });
        //    //hularion.routes.set(request.detail.route, request.detail.handler);
        //    delete request.detail.handler;
        //}
        //if (request.isUIRequest === true) {

        //    return;
        //}

    },

    CreateProxy: function (routeInfo) {
        var t = this;
        var routeKey = routeInfo.routeKey;
        //console.log("HKCRouter.CreateProxy", routeInfo, routeKey, t);

        if (t.routeMethods.has(routeInfo.routeKey)) {
            return t.routeMethods.get(routeInfo.routeKey);
        }
        var prefix = routeKey + "/";
        if (routeKey == null || routeKey == undefined) { prefix = ""; routeKey = ""; }

        var proxy = {
            routeKey: routeKey,
            request: function (route, detail) {
                //console.log("HKCRouter.CreateProxy - request - ", route, detail, routeInfo, routeKey, t);
                if (route.startsWith("hularion/hx")) {
                    return t.Request({ route: route, detail: detail });
                }
                return t.Request({ route: prefix + route, detail: detail });
            },
            addServerRoute: function (route, handler) {
                return t.AddServerRoute({ route: prefix + route, handler: handler });
            },
            getRoutes: function () {
                return (() => t.GetRoutes(routeKey))();
            },
            groups: {}
        };

        routeInfo.routeProviders.forEach(group => {
            var mg = {};
            proxy.groups[group.methodGroup] = mg;
            group.routes.forEach(route => {
                mg[route.method] = (methodRequest) => proxy.request(route.route, methodRequest);
            });
        });
        t.routeMethods.set(routeKey, proxy);
        return proxy;
    },

    GetRoutes: function (routeKey) {
        if (routeKey == null || routeKey == undefined || routeKey == "") {
            return this.Request({ route: "hularion/meta/routes" }).then(response => response.detail);
        }
        //console.log("HKCRouter.GetRoutes", routeKey, "hularion/meta/routes?provider=" + routeKey, this);
        return this.Request({ route: "hularion/meta/routes?provider=" + routeKey }).then(response => {
            response.detail.routes.forEach(route => {
                route.route = route.route.substring(routeKey.length + 1, route.route.length);
            });
            return response.detail;
        });

    }

};
