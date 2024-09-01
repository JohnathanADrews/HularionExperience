/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function PackageLocator(contextKey, packageKey, version, isPackage, isProject, projectLocation, rootLocation) {
    this.contextKey = contextKey;
    this.packageKey = packageKey;
    this.version = version;
    this.isPackage = isPackage;
    this.isProject = isProject;
    this.projectLocation = projectLocation;
    this.rootLocation = rootLocation;
};
PackageLocator.FromKey = function (key) {
    var o = JSON.parse(key);
    var locator = new PackageLocator();
    locator.contextKey = o.contextKey;
    locator.packageKey = o.packageKey;
    locator.version = o.version;
    locator.isPackage = o.isPackage;
    locator.isProject = o.isProject;
    locator.projectLocation = o.projectLocation;
    locator.rootLocation = o.rootLocation;
    return locator;
};
PackageLocator.prototype = {

    GetContextKey: function () {
        return this.contextKey;
    },
    GetPackageKey: function () {
        return this.packageKey;
    },
    GetVersion: function () {
        return this.version;
    },
    GetKey: function () {

        var packageKey = this.packageKey;
        if (packageKey == null) {
            packageKey = "";
        }
        var version = this.version;
        if (version == null) {
            version = "";
        }
        return (packageKey + "@" + version).trim();
        return JSON.stringify(this);
    }

};
