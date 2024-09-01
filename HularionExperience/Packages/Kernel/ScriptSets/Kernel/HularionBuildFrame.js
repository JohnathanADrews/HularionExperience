/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

function HularionBuildFrame(packageProvider, packageKey, setType, setName, frameManager, communicator, frameOptions) {
    this.packageProvider = packageProvider;
    this.packageKey = packageKey;
    this.setType = setType;
    this.setName = setName;
    this.frameManager = frameManager;
    this.communicator = communicator;
    this.frameOptions = frameOptions;
    this.resourceManager = new ResourceManager();
    this.rootFrame = null;

    this.waiter = hularion.constants.CreateWaiter();
    this.waitIndex = this.waiter.AddWaiter();
    this.synchronizer = this.waiter.CreateSynchronizer();

    this.Initialize();
}
HularionBuildFrame.prototype = {

    Initialize: function () {

        var t = this;

        //console.log("HularionBuildFrame.Initialize - ", "Package Key: ", t.packageKey + ", Set Name: " + t.setName, ", this: ", t);

        var frameId = 0;
        createPresenterKey = (frame, importFrame) => {
            //console.log("HularionBuildFrame.Initialize createPresenterKey - ", importFrame.packageAlias, frame.package.GetPackageByAlias(importFrame.packageAlias));
            var key = { id: frameId++, setName: importFrame.setName, type: "Presenter", packageProvider: frame.package.GetPackageByAlias(importFrame.packageAlias), parent: frame, caller: importFrame, frames: new Map(), collaborators: new Map() };
            if (key.setName == null || key.setName == "" || key.setName == "this") { key.setName = frame.setName; }
            return key;
        };
        createScriptKey = (frame, importFrame) => {
            //console.log("HularionBuildFrame.Initialize createScriptKey - ", importFrame.packageAlias, frame.package.GetPackageByAlias(importFrame.packageAlias));
            var key = { id: frameId++, setName: importFrame.setName, type: "Script", packageProvider: frame.package.GetPackageByAlias(importFrame.packageAlias), parent: frame, caller: importFrame, frames: new Map(), collaborators: new Map() };
            if (key.setName == null || key.setName == "" || key.setName == "this") { key.setName = frame.setName; }
            return key;
        };

        checkAncestor = (currentFrame, referencedFrame) => {
            var result = { found: false, frame: null };
            var frame = currentFrame;
            while (frame != null) {
                if (referencedFrame == frame.caller) {
                    result.found = true;
                    result.frame = frame;
                    break;
                }
                frame = frame.parent;
            }
            if (result.found === true) {
                currentFrame.parent.frames.set(currentFrame.caller.frame, result.frame.parent);
                currentFrame.remove = true;
            }
            return result.found;
        };

        var idMap = new Map();
        isIdentified = (currentFrame, referencedFrame) => {
            if (referencedFrame.identifier == null || referencedFrame.identifier.trim() == "") {
                return false;
            }
            var packageKey = currentFrame.package.GetPackageKeyByAlias(referencedFrame.packageAlias);
            if (!idMap.has(packageKey)) { idMap.set(packageKey, new Map()); }
            var ip = idMap.get(packageKey);
            var setWait = false;
            if (!ip.has(referencedFrame.identifier)) {
                ip.set(referencedFrame.identifier, { state: "waiting", callbacks: [] });
                setWait = true;
            }

            var id = ip.get(referencedFrame.identifier);
            var cb = (x) => {
                currentFrame.frames.set(referencedFrame.frame, ip.get(referencedFrame.identifier).frame);
            }
            if (id.state == "waiting") {
                id.callbacks.push(cb);
                return !setWait;
            }
            cb("immediate");
            return true;
        };

        var isSelf = (currentFrame, referencedFrame) => {
            if (currentFrame.type != referencedFrame.type) { return false; }
            if (currentFrame.setName == referencedFrame.setName || referencedFrame.setName == null || referencedFrame.setName.trim() == "" || referencedFrame.setName.trim().toLowerCase() == "this") {
                currentFrame.frames.set(referencedFrame.frame, currentFrame);
                return true;
            }
            return false;
        };

        var frameChecks = (currentFrame, referencedFrame) => {
            if (isIdentified(currentFrame, referencedFrame) === true) { return true; }
            var pk = currentFrame.package.GetPackageKeyByAlias(referencedFrame.packageAlias);
            var id = null;
            if (idMap.has(pk)) { idMap.get(pk).get(referencedFrame.identifier); }
            if (checkAncestor(currentFrame, referencedFrame) === true) { return true; }
            if (isSelf(currentFrame, referencedFrame) === true) { return true; }
            return false;
        }

        var root = { id: frameId++, packageProvider: t.packageProvider(t.packageKey), setName: t.setName, type: t.setType, alias: null, parent: null, frames: new Map(), subs: [], collaborators: new Map() };

        var plan = [];

        t.resourceManager.LoadResourceTree([root],
            frame => { //subKeysProvider
                var keys = [];
                if (frame.type == "Presenter") {
                    if (frame.package.specification.presenterSets[frame.setName] == null) {
                        console.error("The presenter frame with name '" + frame.setName + "' was not present in the presenter sets for package with key '" + frame.package.specification.key + "'");
                    }
                    hularion.Control.ProcessArray(frame.package.specification.presenterSets[frame.setName].presenterImportFrames, presenterFrame => {
                        if (frameChecks(frame, presenterFrame) === true) {
                            return;
                        }
                        var key = createPresenterKey(frame, presenterFrame);
                        if (key.setName == frame.setName && key.caller.frame == presenterFrame.frame) {

                            return;
                        }
                        keys.push(key);
                    });
                    hularion.Control.ProcessArray(frame.package.specification.presenterSets[frame.setName].scriptImportFrames, scriptFrame => {
                        if (frameChecks(frame, scriptFrame) === true) { return; }
                        var key = createScriptKey(frame, scriptFrame);
                        keys.push(key);
                    });
                }
                if (frame.type == "Script") {
                    if (frame.package.specification.scriptSets[frame.setName] != null) {
                        hularion.Control.ProcessArray(frame.package.specification.scriptSets[frame.setName].presenterImportFrames, presenterFrame => {
                            if (frameChecks(frame, presenterFrame) === true) { return; }
                            var key = createPresenterKey(frame, presenterFrame);
                            if (key.setName == frame.setName) {
                                return;
                            }
                            keys.push(key);
                        });
                        hularion.Control.ProcessArray(frame.package.specification.scriptSets[frame.setName].scriptImportFrames, scriptFrame => {
                            if (frameChecks(frame, scriptFrame) === true) { return; }
                            var key = createScriptKey(frame, scriptFrame);
                            if (key.setName == frame.setName) {
                                return;
                            }
                            keys.push(key);
                        });
                    }
                }
                root.subs.push(frame);
                frame.keys = keys;

                frame.getFrames = () => {
                    var frames = {};
                    frame.frames.forEach((value, key, map) => {
                        frames[key] = { frame: value.build };
                    });
                    return frames;
                };
                return keys;
            },
            key => { //resourceProvider
                var p = key.packageProvider.then(package => {
                    key.package = package;
                }).catch(() => { console.warn("package not loaded - ", key); });
                var f = t.frameManager.CreateFrame().promise.then(result => { key.frame = result });

                return Promise.all([p, f])
                    .then(() => {
                        if (key.type == "Presenter") {
                            key.set = key.package.specification.presenterSets[key.setName];
                        }
                        if (key.type == "Script") {
                            key.set = key.package.specification.scriptSets[key.setName];
                        }
                        if (key.parent == null) { return; }
                        if (key.caller.identifier != null && key.caller.identifier.trim() != "") {
                            var ip = idMap.get(key.package.key).get(key.caller.identifier);
                            ip.state = "done";
                            ip.frame = key;
                            hularion.Control.ProcessArray(ip.callbacks, cb => cb("delayed"));
                        }
                        else {
                            key.parent.frames.set(key.caller.frame, key);
                        }
                    })
                    .then(() => key);
            },
            frame => { //Initializer

            },
            frame => { //Finalizer
                if (frame.type == "Presenter") {
                    frame.set = frame.package.specification.presenterSets[frame.setName];
                }
                if (frame.type == "Script") {
                    frame.set = frame.package.specification.scriptSets[frame.setName];
                }
                t.SetFramePath(frame);
            }
        ).then(() => {

            var traverser = new HKCTreeTraverser();
            var removeNodes = [];
            plan = traverser.CreateEvaluationPlan(traverser.PLROrder, root, node => {
                var children = t.resourceManager.GetChildren(node);
                var result = [];
                hularion.Control.ProcessArray(children, c => {
                    if (c.key.remove === true) { removeNodes.push(c); }
                    else { result.push(c.key); }
                });
                return result;
            }, true);

            t.rootFrame = plan[0];
            plan = plan.reverse();

            hularion.Control.ProcessArray(plan, node => {
                if (node.set == null) {
                    node.set = node.package.specification.scriptSets[t.setName];
                }
                t.synchronizer.AddItem(() => {
                    if (node.type == "Presenter") {
                        var properties = {
                            set: node.set,
                            mainFrame: node.frame,
                            communicator: t.communicator,
                            node: node,
                            domeWrapper: node.package.domeWrapper,
                            node:node,
                            graphics: node.package.graphicMap,
                            routeInfo: node.package.specification.routeInformation
                        };
                        node.build = new PresentationFrame(properties);
                    }
                    if (node.type == "Script") {
                        var properties = {
                            set: node.set,
                            mainFrame: node.frame,
                            communicator: t.communicator,
                            attachments: {
                                communicator: t.communicator,
                                dataTypes: t.dataTypes
                            },
                            node: node,
                            routeInfo: node.package.specification.routeInformation
                        };
                        //console.log("HularionBuildFrame.Initialize  new ScriptFrame - ", properties, node);
                        node.build = new ScriptFrame(properties);
                    }
                    return hularion.Control.Promise(true);
                });
            });
            hularion.Control.ProcessArray(plan, node => { });
            t.synchronizer.Start();
            t.waiter.RemoveWaiter(t.waitIndex);
        });

        return t.waiter.Start().then(() => {
            hularion.Control.ProcessArray(plan, node => {
                node.build.frames = node.getFrames();
                node.build.Setup();
            });
            hularion.Control.ProcessArray(plan, node => { node.build.Start(); });
        }).then(() => {
            return t;
        });
    },

    SetFramePath: function (frame) {
        var t = this;
        var path = [];
        var parent = t.resourceManager.GetParent(frame);
        if (parent != null) { parent = parent.value; }
        if (parent == null) {
            path.push("root");
        }
        else {
            path.push(parent.package.key);
            path.push(" ");
            path.push(parent.package.name);
            path.push(".");
            path.push(parent.setName);
            path.push(" (");
            path.push(parent.type);
            path.push(")");
        }
        path.push(" -> ");
        if (frame.caller == null) {
        }
        else {
            path.push("(");
            path.push(frame.caller.packageAlias);
            path.push(" := ");
            path.push(frame.package.key);
            path.push(" ");
            path.push(frame.package.name);
            path.push(").");
            path.push(frame.caller.setName);
            path.push(" (");
            path.push(frame.caller.type);
            path.push(") #");
            path.push(frame.caller.attach);
        }
        t.resourceManager.AssociateValue("path", frame, path.join(""));
    },

    When: function () {
        var t = this;
        //console.log("HularionBuildFrame.When - ", t);
        return t.waiter.When(() => t);
    },

    GetRootFrame: function () {
        var t = this;

        return t.rootFrame.build;
    }



}



