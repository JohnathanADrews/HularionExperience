/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function PresentationFrame(properties) {
    hularion.Set.InitializeObject(this, properties, {
        set: null,
        mainFrame: null
    });

    this.wrapper = x => x;
    //Get the DOME wrapper if one exists, e.g. "$".
    if (properties.domeWrapper != null && properties.domeWrapper.wrapper != null) {
        this.wrapper = properties.domeWrapper.wrapper;
    }

    this.range = document.createRange();
};
PresentationFrame.AttachMode = { Inject: "inject", Frame: "frame" }; //inject => add to the hularion object. frame => a global value
PresentationFrame.AttachOrder = { Before: "before", After: "after" }; //before => attach in first phase (zero dependencies). after => attach after other scripts have been loaded (with dependencies). *** Feature: Consider attach order mechanism.
PresentationFrame.ModuleParameterType = { Single: "single", Params: "params" }; //single => a single parameter value. params => a delimited set of parameters in a string.
PresentationFrame.AttachPresenterReferences = function (frame, references, attach, mode, order) {
    //console.log("PresentationFrame.AttachPresenterReferences - ", frame.set.name, frame, references, attach, mode, order);
    hularion.Control.ProcessArray(references, sRef => {
        if (sRef.attach == null || mode != sRef.attach.toLowerCase()) { return; }
        if (mode != PresentationFrame.AttachMode.Inject && order != sRef.order.toLowerCase()) { return; }
        if (frame.frames[sRef.frame] == null) {
            console.warn("PresentationFrame.AttachScriptReferences - reference frame not found - ", frame.node.package.key, frame.node.package.name, frame.set.name, "-->", sRef.frame, frame.frames, sRef, frame);
            return;
        }
        var rFrame = frame.frames[sRef.frame].frame;

        var o = { presenters: {}, interfaces: { presenters: {}, methods: {} }, static: { interfaces: {} } };
        hularion.Control.SetObjectAtReference(attach, sRef.handle, o, ".");

        hularion.Control.ProcessObjectMemberValues(rFrame.set.presenters, fp => {
            o.presenters[fp.name] = {
                create: () => {
                    var pi = rFrame.CreatePresenter(fp.name);
                    return pi.CreateProxy();
                }
            };
        });
        hularion.Control.ProcessObjectMemberValues(rFrame.set.presenters, fp => {
            hularion.Control.ProcessArray(fp.interfaces, pi => {
                if (o.interfaces[pi.name] == null) { o.interfaces[pi.name] = { presenters: [] }; }
                if (o.interfaces[pi.name][pi.proxy] == null) { o.interfaces[pi.name][pi.proxy] = { method: pi.proxy }; }
                o.interfaces[pi.name].presenters.push(fp);
            });
        });
    });
};

PresentationFrame.prototype = {
    Setup: function () {
        var t = this;
        //console.log("PresentationFrame.Setup - ", t.set.name, t);

        ScriptFrame.AttachScriptReferences(t, t.set.scriptReferences, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, ScriptFrame.AttachOrder.Before);
        PresentationFrame.AttachPresenterReferences(t, t.set.presenterReferences, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, PresentationFrame.AttachOrder.Before);

        hularion.Control.ProcessObjectMemberValues(t.set.presenters, presenter => {
            hularion.Control.ProcessArray(presenter.scripts, script => {
                t.mainFrame.handle.loadScript(script.content);
            });
            hularion.Control.ProcessArray(presenter.styles, style => {
                t.mainFrame.handle.loadStyle(style.content);
            });
        });
    },

    Start: function () {
        var t = this;
        //console.log("PresentationFrame.Setup - ", t.set.name, t);
        //   console.log("****************PresentationFrame.Setup - *************SD", ScriptFrame.AttachOrder);

        ScriptFrame.AttachScriptReferences(t, t.set.scriptReferences, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, ScriptFrame.AttachOrder.After);
        PresentationFrame.AttachPresenterReferences(t, t.set.presenterReferences, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, PresentationFrame.AttachOrder.After);

        var routeProxy = hularion.router.CreateProxy(t.routeInfo);
        t.routeInfo.routeProviders.forEach(routeProvider => {
            if (routeProvider.handle != null && routeProvider.handle != undefined && routeProvider.handle.trim() != "") {
                if (routeProvider.handleType != null && routeProvider.handleType != undefined && routeProvider.handleType.trim().toLowerCase() == "frame") {
                    hularion.Control.SetObjectAtReference(t.mainFrame.attachor.contentWindow, routeProvider.handle.trim(), routeProxy.groups[routeProvider.methodGroup], ".");
                }
            }
        });
    },

    CreatePresenter: function (name, parameters) {
        var t = this;
        if (parameters == null) { parameters = {}; }

        if (t.set.presenters[name] == null) {
            //console.warn("PresentationFrame.CreatePresenter - The presenter with name ", name, " is not available.  Please make sure it is properly included in the package.");
        }

        //console.log("PresentationFrame.CreatePresenter - ", name, t.set.presenters[name], t);
        var instance = null;

        //console.log("PresentationFrame.CreatePresenter - ", name, t, parameters);
        var host = {};
        if (parameters.constructorParameter == null) {
            try {
                t.mainFrame.handle.assign(host, "new " + name + "();");
            }
            catch {
                host.error = true;
                host.withParameter = false;
                console.error("A presenter in set '" + t.set.name + "' attempted to create presenter '" + name + "' but failed.");
            }
        }
        else {
            //assume JSON parameters for now.
            var constructorParameter = JSON.parse(parameters.constructorParameter);
            try {
                t.mainFrame.handle.function(host, "(p) => new " + name + "(p);", constructorParameter);
            }
            catch {
                host.error = true;
                host.withParameter = true;
            }
        }
        if (host.error === true) {
            var innerHost = {};
            t.mainFrame.handle.function(innerHost, "() => window");
            var containsPresenter = " not ";
            if (innerHost.out[name] != null) {
                containsPresenter = " ";
            }
            var presenterName = "undefined";
            if (parameters != null && parameters.caller != null && parameters.caller.source != null) {
                presenterName = parameters.caller.source.name;
            }
            console.error("Presenter '" + presenterName + "' attempted to create presenter '" + t.set.name + "/" + name + "' but failed. The frame for '" + t.set.name + "' does" + containsPresenter + "contain the presenter '" + name + "'. See the frame window:", { out: innerHost.out, parameters: parameters });
            host.callerSet = t.set.name;
            host.createPresenter = t.set.name;
            host.info = parameters;
            throw host;
        }
        //console.log("PresentationFrame.CreatePresenter 2 - ", name, host);

        var code = host.out;

        //Create the presenter
        var presenter = t.set.presenters[name];
        var view = t.range.createContextualFragment(presenter.view).firstChild;
        view.classList.add(presenter.cssClass.trim());
        //console.log("PresentationFrame.CreatePresenter 2 - ", name, host, view, presenter.view);
        //view.dataset.code = code;

        //Attach the script and presenter references for the set and specific presenter.
        PresentationFrame.AttachPresenterReferences(t, t.set.presenterReferences, code, PresentationFrame.AttachMode.Inject, ScriptFrame.AttachOrder.Presenter);
        PresentationFrame.AttachPresenterReferences(t, presenter.presenterReferences, code, PresentationFrame.AttachMode.Inject, ScriptFrame.AttachOrder.Presenter);
        ScriptFrame.AttachScriptReferences(t, presenter.scriptReferences, code, ScriptFrame.AttachMode.Inject);
        ScriptFrame.AttachScriptReferences(t, t.set.scriptReferences, code, ScriptFrame.AttachMode.Inject);

        //Run the presenter's "setup" method if it is defined.
        if (code.setup != null) { code.setup(); }
        else if (code.Setup != null) { code.Setup(); }

        //console.log("PresentationFrame.CreatePresenter presenter - ", name, presenter, code, t);
        //Setup the proxy object for the caller.
        var proxy = {};
        hularion.Control.ProcessArray(presenter.presenterProxies, pp => {
            if (code[pp.method] == null) {
                console.error("[xiVazjBWkEuFCXSpC2uZ9Q] The presenter '" + name + "' with declared proxy method public:'" + pp.name + "' and private:'" + pp.method + "' was not present in the presenter object. Check the name and make sure '" + name + "' has a method with name '" + pp.method + "' or remove the proxy directive.");
            }
            else {
                proxy[pp.name] = eval(hularion.Control.CreateInterfaceEvalText(code[pp.method], "code[pp.method]"));
            }
        });
        hularion.Control.ProcessObjectMemberValues(presenter.componentHandlers, ch => {
            if (code[ch.method] == null) {
                console.error("[COTVsYuuWkCanGh5vi4dfg] The presenter '" + name + "' with declared component method public:'" + ch.name + "' and private:'" + ch.method + "' was not present in the presenter object. Check the name and make sure '" + name + "' has a method with name '" + ch.method + "' or remove the component directive.");
            }
            else {
                proxy[ch.name] = eval(hularion.Control.CreateInterfaceEvalText(code[ch.method], "method"));
            }
        });

        //Create the presenter object.
        var publisher = {};
        var indexed = {};
        hularion.Control.ProcessArray(view.querySelectorAll("*"), node => { indexed[node.getAttribute("h-index")] = node; });
        instance = new HularionPresenter({
            name: name,
            source: presenter,
            indexedNodes: indexed,
            domeWrapper: t.wrapper,
            publisher: publisher,
            routeInfo: t.routeInfo,
            presentationFrameProvider: (frameName) => {
                var frame = t.frames[frameName];
                return frame == null ? t : frame.frame;
            },

        }, {
            view: t.wrapper(view),
            dome: view,
            code: code,
            proxy: proxy,
            publisher: publisher
        }, {
            principal: t.wrapper(view),
            set: name,
            package: t.packageAlias == null ? "this" : t.packageAlias,
            frame: t.mainFrame.attachor.contentWindow 
        });
        instance.presenter.code.hularion.communicator = hularion.communicator;

        //Setup the publish/subscribe functionality.
        t.AttachPublisher(presenter, instance, publisher);


        //Setup the components.
        hularion.Control.ProcessObjectMemberValues(presenter.componentHandlers, handler => {
            instance.componentHandlers.set(handler.name, {
                handler: handler,
                call: (presenterInstance, parameters, split) => {
                    if (split === true && parameters != null) {
                        var a = [presenterInstance];
                        hularion.Control.ProcessArray(parameters, p => a.push(p));
                        return hularion.Control.ParameterizeObjectCall(instance.presenter.code, handler.method, a);
                    }
                    return instance.presenter.code[handler.method](presenterInstance, parameters);
                }
            });
        });

        var placeGraphic = (graphic, element) => {
            //console.log("PresentationFrame.CreatePresenter placeGraphic  - ", graphic, element, presenter, t);
            if (graphic.destination == "src") {
                element.src = t.graphics.get(graphic.alias).content;
            }
            else if (graphic.destination == "background-image") {
                element.style["background-image"] = "url('" + t.graphics.get(graphic.alias).content + "')";
            }
        };
        presenter.graphics.forEach(graphic => {
            //console.log("PresentationFrame.CreatePresenter graphic - ", graphic, indexed[graphic.index], presenter, t);
            if (t.graphics.has(graphic.alias)) {

                if (graphic.isThis === true) {
                    placeGraphic(graphic, view);
                }
                else if (graphic.isHandle === true) {
                    hularion.Control.SetObjectAtReference(code, graphic.handle, t.graphics.get(graphic.alias).content, ".");
                }
                else {
                    placeGraphic(graphic, indexed[graphic.index]);
                }
            }
            else {
                console.warn("[] The requested graphic with name or alias '" + graphic.alias + "' was not found for node index " + graphic.index + ". Check the alias name and the h-graphic='relative path' with which it is associated. ", t.graphics);
                //console.log("Graphics", t.graphics);
            }
        });

        //console.log("PresentationFrame.CreatePresenter DOME2 wrapper - ", name, wrapper, t.domeWrapper, t);
        hularion.Control.ProcessArray(presenter.handles, handle => {
            if (handle.name == "hularion") { return; } //hularion is resereved.
            hularion.Control.SetObjectAtReference(code, handle.name, t.wrapper(indexed[handle.index]), ".");
        });

        //Setup the template creators
        var createTemplate = (name, parameters) => t.CreateTemplate(name, parameters, instance, presenter);
        instance.presenter.code.hularion.createTemplate = (name, parameters) => createTemplate(name, parameters);

        var createTemplates = (names, parameters) => {
            if (names == null) { names = []; }
            if (parameters == null) { parameters = []; }
            var args = [];
            hularion.Control.ProcessIterator(0, names.length - 1, i => { args.push({ n: names[i], p: parameters.length > i ? parameters[i] : null }); });
            return hularion.Control.PromiseOrder(hularion.Control.TransformEach(args, a => createTemplate(a.n, { presenter: a.p, type: PresentationFrame.ModuleParameterType.Single })));
        };
        instance.presenter.code.hularion.createTemplates = (names, parameters) => createTemplates(names, parameters);

        //Setup the clone creator
        instance.presenter.code.hularion.createClone = cloneName => t.CreateClone(cloneName, instance);

        //Add presenters
        instance.AddPresenters(presenter.presenters);

        //Add components
        //instance.AddComponents(presenter.components);

        //Add templates instances
        hularion.Control.TransformEach(presenter.templateInstances, ti => {
            var template = createTemplate(ti.name, ti.parameter);
            var node = indexed[ti.index];
            node.after(template.dome);
            node.remove();
            hularion.Control.SetObjectAtReference(instance.presenter.code, ti.handle, template, ".");
        });

        //Add clone instances
        hularion.Control.ProcessArray(presenter.cloneInstances, ci => {
            var clone = t.CreateClone(ci.name, instance);
            //console.log("cloneInstance - ", ci, clone, indexed, indexed[ci.index]);
            if (ci.elementClass != null) { clone.dome.setAttribute("class", ci.elementClass); }
            if (ci.elementStyle != null) { clone.dome.setAttribute("style", ci.elementStyle); }
            var node = indexed[ci.index];
            node.after(clone.dome);
            node.remove();
            hularion.Control.SetObjectAtReference(instance.presenter.code, ci.handle, clone, ".");
        });

        if (parameters.startParameter == null) { parameters.startParameter = "{}"; }
        var startParameter = JSON.parse(parameters.startParameter);
        if (code.start != null) { startResult = code.start(startParameter); }
        else if (code.Start != null) { startResult = code.Start(startParameter); }

        return instance;
    },

    CreateTemplate: function (name, parameters, instance, presenter) {
        var t = this;
        var source = t.set.presenters[instance.name].templates[name];
        var template = { name: name, handles: {}, data: {} };
        template.dome = t.range.createContextualFragment(source.view).firstChild;
        template.view = t.wrapper(template.dome);

        var tIndexed = {};
        var nodes = template.dome.querySelectorAll("*");
        hularion.Control.ProcessArray(nodes, node => { tIndexed[node.getAttribute("h-index")] = node; });
        hularion.Control.ProcessArray(source.handles, handle => {
            hularion.Control.SetObjectAtReference(template.handles, handle.name, t.wrapper(tIndexed[handle.index]), ".");
        });

        var pMap = new Map();
        var options = {
            componentPresenter: presenter
        };
        hularion.Control.ProcessArray(source.presenters, sp => {
            var frame = instance.presentationFrameProvider(sp.frame);
            var templatePresenter = frame.CreatePresenter(sp.presenterName, options);
            console.log("PresentationFrame.CreateTemplate - source.presenters - ", sp, templatePresenter);
            //templatePresenter.AddComponents(sp.components, options);
            pMap.set(sp, templatePresenter);
        });
        pMap.forEach((value, key, map) => {
            tIndexed[key.index].after(value.presenter.dome);
            tIndexed[key.index].remove();
            hularion.Control.SetObjectAtReference(template.handles, key.handle, value.CreateProxy(), ".");
        });

        ////instance.AddComponents(source.components);
        ////console.log("PresentationFrame.CreateTemplate - source.components - ", source, instance);

        if (source.method != null && parameters != null) {
            if (presenter.templates[name].methodSplit == PresentationFrame.ModuleParameterType.Single) { instance.presenter.code[source.method](template, parameters); }
            if (presenter.templates[name].methodSplit == PresentationFrame.ModuleParameterType.Single.Params) {
                var a = [template];
                hularion.Control.ProcessArray(parameters, p => a.push(p));
                hularion.Control.ParameterizeObjectCall(instance.presenter.code, source.method, a);
            }
        }


        hularion.Control.ProcessArray(source.templateInstances, ti => {
            var tii = t.CreateTemplate(ti.name, ti.parameter, instance, presenter);
            var e = tIndexed[ti.index];
            e.after(tii.dome);
            e.remove();
            tIndexed[ti.index] = tii;
            hularion.Control.SetObjectAtReference(template.handles, ti.handle, tii, ".");
        });

        hularion.Control.ProcessArray(source.cloneInstances, ci => {
            var clone = t.CreateClone(ci.name, instance);
            var e = tIndexed[ci.index];
            e.after(clone.dome);
            e.remove();
            tIndexed[ci.index] = clone;
            hularion.Control.SetObjectAtReference(template.handles, ci.handle, clone, ".");
        });

        return template;
    },

    CreateClone: function (name, instance) {
        var t = this;
        var source = t.set.presenters[instance.name].clones[name];
        var clone = { name: name, handles: {} };
        clone.dome = t.range.createContextualFragment(source.view).firstChild;
        clone.view = t.wrapper(clone.dome);

        var tIndexed = {};
        var nodes = clone.dome.querySelectorAll("*");
        hularion.Control.ProcessArray(nodes, node => { tIndexed[node.getAttribute("h-index")] = node; });

        var ciHandles = new Map();
        hularion.Control.ProcessArray(source.cloneInstances, ci => {
            var cii = t.CreateClone(ci.name, instance);
            var e = tIndexed[ci.index];
            e.after(cii.dome);
            e.remove();
            tIndexed[ci.index] = cii;
            ciHandles.set(ci.index, ci.handle);
        });

        hularion.Control.ProcessArray(source.handles, handle => {
            if (ciHandles.has(handle.index)) {
                hularion.Control.SetObjectAtReference(clone.handles, handle.name, tIndexed[handle.index], ".");
            }
            else {
                hularion.Control.SetObjectAtReference(clone.handles, handle.name, t.wrapper(tIndexed[handle.index]), ".");
            }
        });

        hularion.Control.ProcessArray(nodes, e => {
            e.removeAttribute("h-index");
        });

        return clone;
    },

    AttachPublisher: function (presenter, instance, publisher) {
        var t = this;
        //console.log("PresentationFrame.AttachPublisher - ", presenter, instance, publisher)

        var ip = {}; //instance publisher
        instance.presenter.code.hularion.publisher = ip;
        var pubSub = hularion.PubSubCreator();
        var p = (r, i, o) => pubSub.Publish(r, i, o);
        var subscriptions = new Map();
        hularion.Control.ProcessObjectMemberNameValuePairs(presenter.publishers, pair => {
            var n = pair.name;
            publisher[n] = {
                subscribe: (callback, options) => {
                    //if (subscriptions.has(n)) { return; }
                    var sub = hularion.SubscriberCreator(n, callback, options);
                    subscriptions.set(n, sub);
                    pubSub.Subscribe(sub);
                },
                unsubscribe: () => {
                    //if (!subscriptions.has(n)) { return; }
                    var sub = subscriptions.get(n);
                    subscriptions.delete(n);
                    pubSub.Unsubscribe(sub);
                }
            };
            ip[n] = {
                publish: (o) => { p(n, t.GetPublishObject(instance), o); }
            }
            //console.log("publisher - ", t.set.name, presenter.name, pair, publisher, presenter);
        });
        hularion.Control.ProcessObjectMemberNameValuePairs(presenter.subscribers, pair => {
            //console.log("PresentationFrame.AttachPublisher - ", ip, pair);
            var n = pair.name;
            var sx = ip[n] == null ? {} : ip[n];

            //if (ip[n] == null) {
            //    ip[n] = {};
            //}
            //var sx = ip[n];
            sx.subscribe = (callback, options) => {
                //if (subscriptions.has(n)) { return; }
                var sub = hularion.SubscriberCreator(n, callback, options);
                subscriptions.set(n, sub);
                pubSub.Subscribe(sub);
            }
            sx.unsubscribe = () => {
                //if (!subscriptions.has(n)) { return; }
                var sub = subscriptions.get(n);
                subscriptions.delete(n);
                pubSub.Unsubscribe(sub);
            }
            if (publisher[n] == null) { publisher[n] = {}; }
            publisher[n].publish = (o) => { p(n, t.GetPublishObject(instance), o); }
            //console.log("subscriber - ", t.set.name, presenter.name, pair, publisher, presenter, sx);
            pair.value.method == null ? "" : pair.value.method.trim();
            if (pair.value.method != "") {
                sx.subscribe((o, p) => instance.presenter.code[pair.value.method](o, p));
            }
        });

    },

    GetPublishObject: function (instance) {
        return {
            proxy: instance.presenter.proxy,
            view: instance.presenter.view
        };
    }
}

