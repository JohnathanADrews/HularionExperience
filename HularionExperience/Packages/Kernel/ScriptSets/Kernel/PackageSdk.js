/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function PackageSdk() {
    this.getPackageRoute = "hularion/package/get";
};
PackageSdk.prototype = {

    GetPackage: function (packageLocator, options) {
        var t = this;
        //console.log("PackageSdk.GetPackage - ", packageLocator, options);
        var request = {
            key: "",
            route: t.getPackageRoute,
            detail: { indicator: packageLocator }
        };
        //if (options != null && (options.rebuild === true || options.rebuild === "true")) {
        //    request.route += "?rebuild=true";
        //}
        //console.log("PackageSdk.GetPackage - ", packageKey, contextKey, options, request);
        var promise = hularion.communicator.request(request).then(result => {
            //console.log("PackageSdk.GetPackage -", t.getPackageRoute + " - ", result);
            return result.detail.package;
        });
        return promise;
    }

};
