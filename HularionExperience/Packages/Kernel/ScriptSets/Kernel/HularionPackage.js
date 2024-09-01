/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

function HularionPackage(properties, other) {
    hularion.Set.InitializeObject(this, properties, { });
    this.presenterFrameKey = "thiskey";
    this.aliasMap = new Map();
    this.domeWrapper = { isReady: (this.specification.domeWrapper == null || this.specification.domeWrapper.evalFrameKey == null) }; 
    this.scriptFrames = {};
    this.graphicMap = new Map();
};
HularionPackage.prototype = {
    Initialize: function () {
        var t = this;
        t.aliasMap.set(null, t.key);
        t.aliasMap.set(undefined, t.key);
        t.aliasMap.set("", t.key);
        t.aliasMap.set("this", t.key);
        console.log("HularionPackage.Initialize spec - ", "Name: ", t.name + " - Key: " + t.key, t.specification, t);

        var loadPromises = [];
        hularion.Control.ProcessObjectMemberValues(t.specification.packageImportsByAlias, packageImport => {
            //console.log("HularionPackage.Initialize packageImportsByAlias - ", packageImport);
            var locator = new PackageLocator(null, packageImport.packageKey, packageImport.version);
            var key = locator.GetKey();
            t.aliasMap.set(packageImport.alias.toLowerCase(), key);
            t.aliasMap.set(packageImport.packageKey.toLowerCase(), key);
            //t.aliasMap.set(packageImport.alias.toLowerCase(), packageImport.packageKey);
            //t.aliasMap.set(packageImport.packageKey.toLowerCase(), packageImport.packageKey);
        });
        t.aliasMap.set(t.presenterFrameKey.toLowerCase(), t.key);

        t.specification.scriptFrameReferences.forEach(reference => {
            //console.log("HularionPackage.Initialize scriptFrameReferences - ", reference, t.specification);
            var packageKey = reference.packageAlias;
            var alias = null;
            if (reference.packageAlias != null) {
                alias = reference.packageAlias.toLowerCase();
            }
            if (t.aliasMap.has(alias)) {
                packageKey = t.aliasMap.get(alias);
            }
            //console.log("HularionPackage.Initialize scriptFrameReferences 2 - ", reference.setName, packageKey, alias, t.aliasMap.has(alias), t.aliasMap);
            var frame = { name: reference.frame, frame: t.CreateScriptBuildFrame(reference.setName, packageKey) };
            t.scriptFrames[reference.frame] = frame;
            frame.frame.When().then(() => {
                //console.log("HularionPackage.Initialize scriptFrameReferences - ", frame.frame, frame.frame.rootFrame.frame.handle);
                if (t.specification.domeWrapper != null && t.specification.domeWrapper.evalFrameKey == reference.frame) {
                    //t.domeWrapper = { evalCode: t.specification.domeWrapper.evalCode };
                    var o = {};
                    t.domeWrapper.wrapper = frame.frame.rootFrame.frame.handle.assign(o, t.specification.domeWrapper.evalCode);
                    t.domeWrapper.isReady = true;
                }
            });
        });  

        t.specification.graphics.forEach(x => t.graphicMap.set(x.key, x));
        t.specification.graphicReferences.forEach(x => {
            if (t.graphicMap.has(x.key)) {
                var g = t.graphicMap.get(x.key);
                t.graphicMap.set(x.alias, g);
            }
        });
    },

    GetSpecification: function () {
        return this.specification;
    },

    GetPackageByKey: function (packageKey) {
        var t = this;
        return t.resourceManager.When(packageKey).then(() => t.resourceManager.GetAssociatedValue("package", packageKey));
    },

    GetPackageByAlias: function (alias) {
        var t = this;
        //console.log("HularionPackage.GetPackageByAlias - ", alias, t);
        alias = alias.toLowerCase();
        var packageKey = t.aliasMap.get(alias);
        return t.GetPackageByKey(packageKey);
    },

    GetPackageKeyByAlias: function (alias) {
        var t = this;
        alias = alias.toLowerCase();
        var packageKey = t.aliasMap.get(alias);
        return packageKey;
    },

    CreatePresenterBuildFrame: function (presenterSetName) {
        var t = this;
        //console.log("HularionPackage.CreatePresenterBuildFrame - ", presenterSetName, t.packageKey, t.aliasMap.has(t.packageKey), t.packageKey == null, t.packageKey == undefined, t);
        var buildFrame = new HularionBuildFrame(packageKey => {
            //console.log("HularionPackage.CreatePresenterBuildFrame packageProvider - ", packageKey, presenterSetName);
            return t.resourceManager.When(packageKey).then((package) => {
                //console.log("HularionPackage.CreatePresenterBuildFrame package - ", presenterSetName, package);
                return t.resourceManager.GetAssociatedValue("package", packageKey);
            });
        }, t.aliasMap.get(t.packageKey), "Presenter", presenterSetName, hularion.frameManager, t.communicator, { graphics: t.graphicMap, routeInfo: t.specification.routeInformation });
        //console.log("HularionPackage.CreatePresenterBuildFrame - ", presenterSetName, buildFrame);
        return buildFrame;
    },

    CreateScriptBuildFrame: function (scriptSetName, packageKey) {
        var t = this;
        //console.log("HularionPackage.CreateScriptBuildFrame - ", scriptSetName, packageKey, t);
        var buildFrame = new HularionBuildFrame(packageKey => {
            //console.log("HularionPackage.CreateScriptBuildFrame packageProvider - ", packageKey, scriptSetName);
            return t.resourceManager.When(packageKey).then(() => t.resourceManager.GetAssociatedValue("package", packageKey));
        }, packageKey, "Script", scriptSetName, hularion.frameManager, t.communicator, { routeInfo: t.specification.routeInformation });
        return buildFrame;
    },

    When: function () {
        var t = this;
        //console.log("HularionPackage.When - ", t.domeWrapper.isReady, t);
        var constants = new HularionConstants();
        return constants.WaitClimb(() => t.domeWrapper.isReady);
    }
};
