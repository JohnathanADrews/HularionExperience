/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function HularionConstructor(setup) {

    //console.log("HularionConstructor setup - ", setup);

    this.principal = window.parent;
    this.principal.hularionWindow = window;

    this.iFrameContainer = document.createElement("div");
    this.scriptFrameContainer = document.createElement("div");
    this.styles = {};
    this.styleCategories = {};
    this.cssSection = this.principal.document.createElement("div");

    this.frameManager = new FrameManager();

    this.receiverRoutes = new Map();
    this.stylesLoaded = new Map();

    this.dataTypes = setup.initializer.dataTypes;

    this.isLoaded = false;

    this.Initialize(setup);
};
HularionConstructor.prototype = {
    Initialize: function (setup) {
        var t = this;

        window.hularion = {};

        hularion = {};
        hularion.communicator = {
            request: (request) => {
                var result = setup.requestProcessor(request);
                //console.log("communicator.request - ", request, result);
                result.then(response => {
                    if (response.state == "Failure") {
                        console.error("Server Request Error - ", request, response);
                    }
                    //console.log("communicator.request 2 - ", request, r);
                });
                return result;
            }
        }

        hularion.constructor = this;
        var keyIndex = 0;
        hularion.window = window.parent;
        hularion.keyProvider = ()=>(keyIndex++).toString();
        hularion.Set = new HKCSet();
        hularion.Control = new HKCControl();
        hularion.TreeTraverser = new HKCTreeTraverser();
        hularion.PubSubCreator = ()=> new HKCPubSub();
        hularion.SubscriberCreator = (route, callback, options) => new HKCPubSubSubscriber(route, callback, options);
        hularion.constants = new HularionConstants();
        hularion.frameManager = new FrameManager(setup.initializer.iFrameLoader, "hularion.frameManager", t.iFrameContainer);
        hularion.router = new HKCRouter(hularion.communicator);
        hularion.packageSdk = new PackageSdk();

        hularion.router.AddClientRoute({
            route: "hx/application/run",
            handler: (appInfo) => {
                //console.log("HularionConstructor route run app -", appInfo);//, appInfo.presenterSet, appInfo.entryPresenter, appInfo);
                return t.LoadPresenter(appInfo.detail.packageKey, appInfo.detail.presenterSet, appInfo.detail.entryPresenter, appInfo.detail.contextKey);
            }
        });
        hularion.router.AddClientRoute({
            route: "hx/application/getentry",
            handler: (appInfo) => {
                //console.log("HularionConstructor route hx/application/getentry -", appInfo);
                return t.GetApplicationEntry(appInfo.detail.packageKey, appInfo.detail.presenterSet, appInfo.detail.entryPresenter, appInfo.detail.contextKey);
            }
        });
        hularion.router.AddClientRoute({
            route: "hx/application/iframe/get",
            handler: (request) => {
                //console.log("HularionConstructor route hx/application/getiframe -", request, hularion.router);
                return hularion.router.Request({ route: "hularion/hx/index/get", detail: {} }).then(response => {
                    //console.log("HularionConstructor  hularion/hx/getindex", response);

                    var iframe = document.createElement("iframe");
                    iframe.setAttribute("srcDoc", response.detail.indexPage);
                    iframe.setAttribute("style", "width:100%;border:none;");
                    var result = { detail: { iframe: iframe } };
                    var constants = new HularionConstants();
                    result.detail.loaded = constants.WaitClimb(() => (iframe.contentWindow != null
                        && iframe.contentWindow.hularionWindow != null
                        && iframe.contentWindow.hularionWindow.hularion != null
                        && iframe.contentWindow.hularionWindow.hularion.constructor != null
                        && iframe.contentWindow.hularionWindow.hularion.constructor.isLoaded === true
                    )).then(() => {
                        var appInfo = request.detail;
                        console.log("HularionConstructor - Loading Application: ", appInfo.applicationName, appInfo);
                        var options = { rebuild: request.parameters.get("rebuild") };
                        iframe.contentWindow.hularionWindow.hularion.constructor.LoadPresenter(appInfo.packageKey, appInfo.version, appInfo.presenterSet, appInfo.entryPresenter, appInfo.contextKey, appInfo, options);
                    });

                    return result
                });
                //return t.GetApplicationEntry(appInfo.detail.packageKey, appInfo.detail.presenterSet, appInfo.detail.entryPresenter, appInfo.detail.contextKey);
            }
        });
        hularion.router.AddClientRoute({
            route: "hx/application/close",
            handler: (detail) => {

            }
        });

        t.principal.hularionReceiver = (message) => {
            hularion.router.Request(message);
        };

        hularion.routes = new Map();


        hularion.packageManager = new PackageManager({
            scriptContainer: t.scriptFrameContainer,
            keyProvider: setup.keyProvider,
            dataTypes: t.dataTypes,
            attachments: {
                communicator: t.communicator,
                dataTypes: t.dataTypes
            }
        });

        hularion.scriptFrameContainer = document.createElement("div");
        hularion.scriptFrameContainer.style = "display:none;";
        document.getElementsByTagName("body")[0].appendChild(hularion.scriptFrameContainer);

        hularion.cssSection = this.principal.document.createElement("div");
        hularion.cssSection.setAttribute("style", "display:none;");
        hularion.cssSection.setAttribute("class", "h-style-container");
        this.principal.document.getElementsByTagName("body")[0].appendChild(hularion.cssSection);

        hularion.iFrameContainer = document.createElement("div");
        hularion.iFrameContainer.style = "display:none;";
        document.getElementsByTagName("body")[0].appendChild(t.iFrameContainer);


        hularion.styleManager = new StyleManager();
        hularion.styleManager.SetContainer(hularion.cssSection);


        if (t.principal == window.top && setup.initializer.applicationStartup != null) {
            var appStartup = setup.initializer.applicationStartup;
            var locator = new PackageLocator();
            locator.packageKey = appStartup.packageKey;
            locator.version = appStartup.packageVersion;
            console.log("HularionConstructor.Initialize appStartup - ", appStartup, locator);
            hularion.packageManager.LoadPackage(locator).then((package) => {
                hularion.Control.ProcessObjectMemberNameValuePairs(package.specification.applications, pair => {
                    if (pair.value.key == appStartup.startApplication) {
                        pair.value.styleSelections.forEach(style => {
                            hularion.styleManager.SelectCategory(style.categoryName, style.selectedStyle);
                        });
                        t.LoadPresenter(package.specification.key, package.specification.version, pair.value.presenterSet, pair.value.presenter);
                        return false;
                    }
                });
            });
        }

        t.isLoaded = true;
    },


    GetApplicationEntry: function (packageKey, setName, presenterName, contextKey) {
        var t = this;
        //console.log("HularionConstructor.GetApplicationEntry - packageKey, setName, presenterName, contextKey - ", packageKey, setName, presenterName, contextKey);
        var build = null;
        return hularion.packageManager.GetPackage(packageKey, contextKey).then(pk => {
            //console.log("HularionConstructor.GetApplicationEntry - package - ", packageKey, setName, presenterName, pk);
            build = pk.CreatePresenterBuildFrame(setName);
            //console.log("HularionConstructor.GetApplicationEntry 2 - build - ", packageKey, setName, presenterName, build);
            return build.When();

        }).then(() => {
            //console.log("HularionConstructor.GetApplicationEntry - build frame 1 - ", packageKey, setName, presenterName, build);
            var pf = build.GetRootFrame();
            var presenter = pf.CreatePresenter(presenterName);
            //console.log("HularionConstructor.GetApplicationEntry - presenter - ", presenter);
            return presenter.presenter;
        });
    },


    LoadPresenter: function (packageKey, version, setName, presenterName, contextKey, appInfo, options) {
        var t = this;
        if (version == null || version.trim() == "") {
            version = "latest"
        }
        var locator = new PackageLocator(contextKey, packageKey, version);
        //if (appInfo != null) {
        //    locator.isPackage = appInfo.isPackage;
        //    locator.isProject = appInfo.isProject;
        //    locator.projectLocation = appInfo.projectLocation;
        //}
        //console.log("HularionConstructor.LoadPresenter - packageKey, setName, presenterName, contextKey - ", packageKey, setName, presenterName, contextKey, locator.GetKey(), locator, appInfo, locator);
        //return hularion.packageManager.GetPackage(packageKey, contextKey, options).then(pk => {
        return hularion.packageManager.GetPackage(locator, options).then(pk => {
            //console.log("HularionConstructor.LoadPresenter - package - ", packageKey, setName, presenterName, pk);
            var build = pk.CreatePresenterBuildFrame(setName);
            //console.log("HularionConstructor.LoadPresenter 2 - build - ", packageKey, setName, presenterName, build);

            build.When().then(() => {

                //console.log("HularionConstructor.LoadPresenter - build frame 1 - ", packageKey, setName, presenterName, build);
                var pf = build.GetRootFrame();
                //console.log("HularionConstructor.LoadPresenter - build frame 1 - ", packageKey, version, setName, presenterName, build, pf);
                var presenter = pf.CreatePresenter(presenterName, { caller: { source: { name: "system::HularionPlayer" } } });
                //console.log("HularionConstructor.LoadPresenter - presenter - ", presenter);
                this.principal.document.body.appendChild(presenter.presenter.view);
            });
        });
    },

    LoadPresenter: function (packageKey, version, setName, presenterName, contextKey, appInfo, options) {
        var t = this;
        if (version == null || version.trim() == "") {
            version = "latest"
        }
        var locator = new PackageLocator(contextKey, packageKey, version);
        if (appInfo != null) {
            locator.isPackage = appInfo.isPackage;
            locator.isProject = appInfo.isProject;
            locator.projectLocation = appInfo.projectLocation;
        }
        //console.log("HularionConstructor.LoadPresenter - packageKey, setName, presenterName, contextKey - ", packageKey, setName, presenterName, contextKey, locator.GetKey(), locator, appInfo, locator);
        console.log("HularionConstructor.LoadPresenter ", packageKey, setName, presenterName, locator);
        //return hularion.packageManager.GetPackage(packageKey, contextKey, options).then(pk => {
        return hularion.packageManager.GetPackage(locator, options).then(pk => {
            //console.log("HularionConstructor.LoadPresenter - package - ", packageKey, setName, presenterName, pk);
            var build = pk.CreatePresenterBuildFrame(setName);
            //console.log("HularionConstructor.LoadPresenter 2 - build - ", packageKey, setName, presenterName, build);

            build.When().then(() => {

                //console.log("HularionConstructor.LoadPresenter - build frame 1 - ", packageKey, setName, presenterName, build);
                var pf = build.GetRootFrame();
                //console.log("HularionConstructor.LoadPresenter - build frame 1 - ", packageKey, version, setName, presenterName, build, pf);
                var presenter = pf.CreatePresenter(presenterName, { caller: { source: { name: "system::HularionPlayer" } } });
                //console.log("HularionConstructor.LoadPresenter - presenter - ", presenter);
                this.principal.document.body.appendChild(presenter.presenter.dome);
            });
        });
    }
}


