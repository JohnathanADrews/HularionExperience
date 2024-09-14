/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function PackageManager(properties) {
    hularion.Set.InitializeObject(this, properties, {
        frameManager: null,
        communicator: null,
        keyProvider: null
    });
    this.resourceManager = new ResourceManager("PackageManager");
};
PackageManager.prototype = {
    LoadPackages: function (packageLocators) {
        var t = this;
        hularion.Control.ProcessArray(packageLocators, packageLocator => t.LoadPackage(packageLocator));
    },
    LoadPackage: function (packageLocator) {
        var t = this;
        //console.log("PackageManager.LoadPackage - '" + packageLocator.packageKey + "', version: '" + packageLocator.version + "'");
        packageLocator.rootLocation = packageLocator.projectLocation;

        var keyMap = new Map();
        var valueMap = new Map();

        keyMap.set(packageLocator.GetKey(), packageLocator);

        var promises = [];
        return t.resourceManager.LoadResourceTree(//(keys, subKeysProvider, resourceProvider, resourceInitializer, resourceFinalizer) 
            [packageLocator.GetKey()],
            value => {
                if (value == null) {
                    console.error("PackageManager.LoadPackage LoadResourceTree[subKeysProvider] Package is null : ", packageLocator.GetKey(), { packageLocator: packageLocator, value: value, resourceManager: t.resourceManager });
                }
                var keys = [];
                //console.log("PackageManager.LoadPackage parentLocator.isPackage === true ", keyMap.get(value));
                //console.log("PackageManager.LoadPackage subKeysProvider ", value);
                var parentLocator = valueMap.get(value);
                hularion.Control.ProcessObjectMemberValues(value.packageImportsByAlias, p => {
                    var locator = new PackageLocator(packageLocator.contextKey, p.packageKey, p.version);
                    if (parentLocator.isProject === true) {
                        locator.isProject = true;
                    }
                    if (parentLocator.isPackage === true) {
                        locator.isPackage = true;
                    }
                    locator.projectLocation = p.project;
                    locator.rootLocation = parentLocator.rootLocation;
                    keyMap.set(locator.GetKey(), locator);
                    keys.push(locator.GetKey());
                });
                //console.log("PackageManager.LoadPackage resourceManager.subKeysProvider - ", value, keys);
                return keys;
            },
            (key) => {
                //console.log("PackageManager.LoadPackage LoadResourceTree[resourceProvider] - ", key, PackageLocator.FromKey(key),  {  packageSdk: hularion.packageSdk });
                var locator = keyMap.get(key);
                locator.contextKey = packageLocator.contextKey;
                return hularion.packageSdk.GetPackage(locator, { rebuild: t.rebuild }).then(package => {
                    if (package == null) {
                        console.error("PackageManager.LoadPackage LoadResourceTree[resourceProvider] result package is null : ", locator.packageKey, locator.version, locator.GetKey(), locator);
                    }
                    //console.log("PackageManager.LoadPackage - Package Result - ", key, package);
                    //keyMap.set(key, locator);
                    valueMap.set(package, locator);
                    return package;
                });
            },
            (specification, locatorKey) => {
                //console.log("PackageManager.LoadPackage specification - ", specification.key, specification, locatorKey);
                //console.log("PackageManager.LoadPackage specification t.resourceManager - ", t.resourceManager);
                var properties = {};
                //properties.key = specification.key;
                properties.key = locatorKey;
                properties.name = specification.name;
                properties.specification = specification;
                properties.packageLoader = key => t.LoadPackage(key);
                properties.resourceManager = t.resourceManager;
                properties.dataTypes = t.dataTypes;
                properties.locatorKey = t.locatorKey;
                var package = new HularionPackage(properties);
                promises.push(package.When());
                hularion.styleManager.RegisterPackage(package);
                t.resourceManager.AssociateValue("package", locatorKey, package);
                t.resourceManager.AssociateValue("package", specification.key, package);
            },
            (specification, locatorKey) => {
                var locator = new PackageLocator(packageLocator.contextKey, specification.key, specification.version);
                //console.log("PackageManager.LoadPackage finalizer specification 1 - ", specification.key, specification, locator.GetKey(), locator, t.resourceManager.GetKeys(), locatorKey);
                var package = t.resourceManager.GetAssociatedValue("package", locatorKey);
                //console.log("PackageManager.LoadPackage finalizer specification 2 - ", specification.key, specification, package);
                package.Initialize();
            })
        .then(p => {
            //console.log("PackageManager.LoadPackage promises - ", packageLocator, promises);
            return Promise.all(promises).then(() => {
                //console.log("PackageManager.LoadPackage promises then - ", packageLocator, promises);
                return t.resourceManager.GetAssociatedValue("package", packageLocator.GetKey());
            });
        });

    },

    GetPackage: function (packageLocator, options) {
        var t = this;
        //console.log("PackageManager.GetPackage ", packageLocator.GetKey(),packageLocator,  t);


        if (options != null && (options.rebuild === true || options.rebuild === "true")) {
            var packageManager = new PackageManager({
                frameManager: t.frameManager,
                communicator: t.communicator,
                keyProvider: t.keyProvider,
                rebuild: true
            });
            return packageManager.GetPackage(packageLocator);
        }

        if (!t.resourceManager.Has(packageLocator.GetKey())) {

            //console.log("PackageManager.GetPackage resourceManager.Has NOT", packageLocator.GetKey(), packageLocator, t);
            
            return t.LoadPackage(packageLocator)
                .then((result) => {
                    //console.log("PackageManager.GetPackage LoadPackage p1 ", packageLocator, result);
                    return t.resourceManager.When(packageLocator.GetKey());
                }).then((result) => {
                    //console.log("PackageManager.GetPackage LoadPackage p2 ", packageLocator, result);
                    return t.resourceManager.GetAssociatedValue("package", packageLocator.GetKey());
                });
        }
        //console.log("PackageManager.GetPackage resourceManager.Has ", packageLocator.GetKey(), packageLocator, t);

        return t.resourceManager.When(packageLocator.GetKey()).then(x => {
            return t.resourceManager.GetAssociatedValue("package", packageLocator.GetKey());
        });

    }
}
