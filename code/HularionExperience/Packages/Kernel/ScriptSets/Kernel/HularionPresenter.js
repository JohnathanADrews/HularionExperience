/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function HularionPresenter(properties, presenterProperties, hularionProperties) {
    this.presenter = {};
    hularion.Set.InitializeObject(this, properties, {
        presentationFrameProvider: (frameName) => null,
    });
    hularion.Set.InitializeObject(this.presenter, presenterProperties, {
        view: null,
        code: null
    });
    this.presenter.code.hularion = {};
    //console.log("HularionPresenter - ", properties, this);
    var router = hularion.router.CreateProxy(properties.routeInfo);
    hularion.Set.InitializeObject(this.presenter.code.hularion, hularionProperties, {
        principal: null,
        set: null,
        title: "title",
        router: router
    });

    this.componentHandlers = new Map();
    this.thisPackageComponentHandler = "this";
    this.eventManager = null;

    this.Initialize();
};
HularionPresenter.prototype = {

    Initialize: function () {
        var t = this;
        //console.log("HularionPresenter.Initialize -  ", t.name, t);

        t.presenter.code.hularion.window = hularion.window;

        var proxy = t.CreateProxy();
        t.presenter.code.hularion.proxy = proxy;
        hularion.Control.ProcessArray(t.presenter.presenterProxies, pp => {
            proxy[pp.name] = eval(hularion.Control.CreateInterfaceEvalText(code[pp.method], "code[pp.method]"));
        });
        hularion.Control.ProcessObjectMemberValues(t.presenter.componentHandlers, ch => {
            proxy[ch.name] = eval(hularion.Control.CreateInterfaceEvalText(code[ch.method], "method"));
        });
    },

    CreatePresenter: function (presenterName, frameName, components, constructorParameter, startParameter) {
        var t = this;
        var frame = t.presentationFrameProvider(frameName);
        //console.log("CreatePresenter -  ", presenterName, frameName, components, frame);
        return frame.CreatePresenter(presenterName, { components: components, constructorParameter, startParameter, caller: t });
    },

    AddComponent: function (component) {
        var t = this;
        //console.log("AddComponent -  ", t.name, "-->", component.presenterName, component.handle, component.handler, component.parameter, component, t);
        //console.log("AddComponent 1 -  ", component, t.presentationFrameProvider(component.frame));
        var handler = t.componentHandlers.get(component.handler);
        //console.log("AddComponent 2 handler -  ", t.name, "-->", component.setName, ":", component.presenterName, component.handle, component.handler, component.parameter, handler);
        var result = null;

        //var instance = t.CreatePresenter(component.presenterName, component.setName, component.packageAlias, component.components);
        var instance = null;
        if (component.presenterName == "#") {
            
        }
        else {
            var instance = t.CreatePresenter(component.presenterName, component.frame, component.components, component.constructorParameter, component.startParameter);
        }
        var componentResult = instance.AddComponents(component.components);
        var proxy = instance.CreateProxy();
        if (handler.methodSplit == PresentationFrame.ModuleParameterType.Params) {
            result = { instance: instance, handler: () => handler.call(proxy, component.parameter, true) };
        }
        result = { instance: instance, handler: () => handler.call(proxy, component.parameter) };
        result.componentResult = componentResult;
        result.source = component;
        result.handler();
        return result;
    },

    AddComponents: function (components) {
        var t = this;
        var result = { handles: {} };
        hularion.Control.ProcessArray(components, component => {
            var addResult = t.AddComponent(component);
            //console.log("HularionPresenter.AddComponents - ", component.handle, component, "-->", addResult);
            if (component.handle != null) {
                result.handles[component.handle] = addResult;
            }
        });
        return result;
    },

    AddPresenter: function (presenter) {
        var t = this;
        //console.log("HularionPresenter.AddPresenter 1 - ", t.name, "-->", presenter.presenterName, t, presenter);
        var instance = t.CreatePresenter(presenter.presenterName, presenter.frame);
        //console.log("HularionPresenter.AddPresenter 2 - ", t.name, "-->", presenter.presenterName, t, presenter, instance, instance.presenter.dome.style, presenter.elementStyle);
        if (presenter.elementClass != null && presenter.elementClass.trim() != "") {
            if (instance.presenter.dome.className == null) {
                instance.presenter.dome.className = presenter.elementClass;
            }
            else {
                instance.presenter.dome.className = instance.presenter.dome.className + " " + presenter.elementClass;
            }
        }
        if (presenter.elementStyle != null && presenter.elementStyle != "") {
            presenter.elementStyle.split(";").forEach(style => {
                var pair = style.split(":");
                if (pair.length < 2) {
                    return;
                }
                instance.presenter.dome.style[pair[0]] = pair[1];
            });
        }
        var componentResult = instance.AddComponents(presenter.components);
        //console.log("HularionPresenter.AddPresenter 2 - ", instance, componentResult);
        var componentHandles = hularion.TreeTraverser.CreateEvaluationPlan(hularion.TreeTraverser.LRPOrder, componentResult, cr => {
            var cresult = [];
            var handles = cr.handles;
            if (handles == null) {
                handles = cr.value.componentResult.handles;
            }
            hularion.Control.ProcessArray(hularion.Control.GetObjectMemberNameValuePairs(handles), crh => {
                cresult.push(crh);
            });
            return cresult;
        }, true);
        //console.log("HularionPresenter.AddPresenter componentHandles - ", t.name, "-->", presenter.presenterName, componentHandles);
        hularion.Control.ProcessArray(componentHandles, pair => {
            if (pair == null || pair.name == null || pair.value == null) { return; }
            //(start, reference, value, delimiter
            hularion.Control.SetObjectAtReference(t.presenter.code, pair.name, pair.value.instance.CreateProxy(), ".");
            hularion.Control.ProcessArray(pair.value.source.subscriptions, subscription => {
                //console.log("HularionPresenter.AddPresenter pair.value.instance - ", t.name, "-->", presenter.presenterName, pair, pair.value.instance.publisher, subscription.event);
                if (pair.value.instance.publisher[subscription.event] == null) {
                    console.error("[g3SjYYbioUuLSQGqz2GS9A] The presenter frame reference '" + t.name, "-->", presenter.presenterName + " with component '" + pair.name + "': Component does not publish to '" + subscription.event + "'");
                    return;
                }
                if (t.presenter.code[subscription.method] == null) {
                    console.error("[jQiTWBatIkGuhC8PY6dOGw] The presenter frame reference " + t.name, "-->", presenter.presenterName + " with component '" + pair.name + "': Containing presenter does not contain the method '" + subscription.method + "'");
                    return;
                }
                var method = (x, y) => t.presenter.code[subscription.method](x, y);
                pair.value.instance.publisher[subscription.event].subscribe((x, y) => {
                    //console.log("HularionPresenter.AddPresenter presenter.subscriptions 1 - ", t.name, "-->", presenter.presenterName, instance, subscription, method);
                    method(x,y);
                });
            });

            //t.presenter.code[pair.name] = pair.value.instance.presenter;
        });
        hularion.Control.ProcessArray(presenter.subscriptions, subscription => {
            //console.log("HularionPresenter.AddPresenter presenter.subscriptions - ", t.name, "-->", presenter.presenterName, instance, subscription);
            if (instance.publisher[subscription.event] == null) {
                console.error("[6cBLnBI7yUW115uzszTUQw] The presenter frame reference '" + t.name, "-->", presenter.presenterName + " : Presenter Reference does not publish to '" + subscription.event + "'");
                return;
            }
            if (t.presenter.code[subscription.method] == null) {
                console.error("[jvEvjKS8g0uCklsxzbZfZg] The presenter frame reference " + t.name, "-->", presenter.presenterName + " : Containing presenter does not contain the method '" + subscription.method + "'");
                return;
            }
            var method = (x, y) => t.presenter.code[subscription.method](x, y);
            instance.publisher[subscription.event].subscribe((x, y) => {
                //console.log("HularionPresenter.AddPresenter presenter.subscriptions 2 - ", t.name, "-->", presenter.presenterName, instance, subscription);
                method(x,y);
            });
        });
        //console.log("HularionPresenter.AddPresenter presenter.subscriptions indexedNodes4 - ", t.name, "-->", presenter.presenterName, t.indexedNodes, presenter.index);
        t.indexedNodes[presenter.index].after(instance.presenter.dome);
        t.indexedNodes[presenter.index].remove();
        var proxy = instance.CreateProxy();
        if (presenter.handle != null) { hularion.Control.SetObjectAtReference(t.presenter.code, presenter.handle, proxy, "."); }
    },

    AddPresenters: function (presenters) {
        var t = this;
        hularion.Control.ProcessArray(presenters, presenter => t.AddPresenter(presenter));
    },

    SetEventManager: function (eventManager) {
        this.eventManager = eventManager;
    },

    CreateProxy: function () {
        var t = this;
        var proxy = {
            dome: t.presenter.dome,
            view: t.presenter.view,
            proxy: t.presenter.proxy,
            publisher: t.presenter.publisher
        };

        if (t.source.members.length > 0) {
            //console.log("HularionPresenter.CreateProxy proxy Access - ", t.name, "-->", t.source.name, t);
            hularion.Control.ProcessArray(t.source.members, member => {

                var access = {};
                for (var i = 0; i < member.access.length; i++) {
                    access[member.access[i]] = 0;
                }
                //console.log("HularionPresenter.CreateProxy proxy Access  getterMembers- ", t.source.name, "-->", t.source.name, member, access);
                try {
                    if (access["get"] == 0 && access["set"] == 0) {
                        Object.defineProperty(proxy.proxy, member.name, {
                            get: function () { return t.presenter.code[member.name]; },
                            set: function (value) { t.presenter.code[member.name] = value; }
                        })
                    }
                    else if (access["get"] == 0 && access["set"] == null) {
                        Object.defineProperty(proxy.proxy, member.name, {
                            get: function () { return t.presenter.code[member.name]; }
                        });
                    }
                    else if (access["get"] == null && access["set"] == 0) {
                        Object.defineProperty(proxy.proxy, member.name, {
                            set: function (value) { t.presenter.code[member.name] = value; }
                        });
                    }
                }
                catch (e) {
                    //console.log("HularionPresenter.CreateProxy proxy Access  getterMembers catch- ", t.source.name, "-->", t.source.name, member, access, proxy, e);
                }
            });
            //hularion.Control.ProcessArray(t.source.getterMembers, member => {
            //    console.log("HularionPresenter.CreateProxy proxy Access  getterMembers- ", t.source.name, "-->", t.source.name, member);
            //    Object.defineProperty(proxy, member, {
            //        get: function () { return t.presenter.code[member]; }
            //    });	
            //});
            //hularion.Control.ProcessArray(t.source.setterMembers, member => {
            //    console.log("HularionPresenter.CreateProxy proxy Access  setterMembers- ", t.source.name, "-->", t.source.name, member);
            //    Object.defineProperty(proxy, member, {
            //        set: function (value) { t.presenter.code[member] = value; }
            //    });	
            //});
        }
        return proxy;
    }

};
