/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function StyleManager() {
    this.container = null;
    this.categories = new Map();
    this.globalContainer = document.createElement("div");
    this.globalContainer.setAttribute("h-style-global-container", true);
    var globalStyle = document.createElement("style");
    globalStyle.setAttribute("h-style-global", true);
    globalStyle.innerHTML = ".hide { display: none; }";
    this.globalContainer.appendChild(globalStyle)
    this.categoryContainer = document.createElement("div");
    this.categoryContainer.setAttribute("h-style-category-container", true);
    this.getNotifyCategoryUpdateRoute = "hularion/style/category/update";

    var t = this;

    hularion.router.AddServerRoute({
        route: "style/category/select",
        handler: (request) => {
            t.SelectCategory(request.category, request.option);
        }
    });

};
StyleManager.prototype = {
    SetContainer: function (container) {
        this.container = container;
        container.appendChild(this.globalContainer);
        container.appendChild(this.categoryContainer);
        //console.log("global container - ", this.globalContainer);
        return this;
    },
    RegisterPackage: function (package) {
        var t = this;
        var packageContainer = document.createElement("div");
        packageContainer.setAttribute("h-package-style", package.name + " " + package.key);
        //console.log("StyleManager.RegisterPackage - ", package.name, " - ", package.key, " - ", package);


        /* Package-level styles. */
        //TODO

        //var packageMap = new Map();
        //t.styleCategories.set(package.key, packageMap);
        hularion.Control.ProcessObjectMemberNameValuePairs(package.specification.presenterSets, pair => {

            var styles = new Map();
            var setContainer = document.createElement("div");
            setContainer.setAttribute("h-set-style", pair.name);
            setContainer.setAttribute("h-presenter-style-persistent", "true");
            packageContainer.appendChild(setContainer);
            t.container.appendChild(packageContainer);

            /* Set-level styles. */
            //var setStyleContainer = document.createElement("style");
            //setStyleContainer.setAttribute("h-set-style", pair.name);
            //setStyleContainer.innerHTML = pair.value.cssStyles;
            //setContainer.appendChild(setStyleContainer);

            /* Add the persistent non-category styles. */
            var persistentStyles = new Map();
            hularion.Control.ProcessArray(pair.value.persistentStyles, style => {
                persistentStyles.set(style, null);
            });
            hularion.Control.ProcessArray(pair.value.styles, style => {
                //console.log("StyleManager.RegisterPackage - pair.value.styles - ", style);
                styles.set(style.key, style);
                if (!persistentStyles.has(style.key)) { return; }
                var styleContainer = document.createElement("style");
                styleContainer.setAttribute("h-presenter-style", style.presenterName);
                styleContainer.innerHTML = style.content;
                //console.log("StyleManager.RegisterPackage - 3 - ", styleContainer, style.content);
                setContainer.appendChild(styleContainer);
            });

            /* Add the style categories. */
            hularion.Control.ProcessObjectMemberNameValuePairs(pair.value.styleCategories, category => {
                //console.log("StyleManager.RegisterPackage - styleCategories 2 - ", category);
                t.AddCategory(category.name);
                var categoryInfo = t.categories.get(category.name);

                hularion.Control.ProcessObjectMemberNameValuePairs(category.value, categoryValue => {
                    for (var i = 0; i < categoryValue.value.length; i++) {
                        if (!categoryInfo.styles.has(categoryValue.name)) {
                            categoryInfo.styles.set(categoryValue.name, []);
                        }
                        var categoryItem = document.createElement("style");
                        categoryItem.innerHTML = styles.get(categoryValue.value[i]).content;
                        categoryInfo.styles.get(categoryValue.name).push(categoryItem);
                    }
                });
                t.SetCategory(category.name);
            });
            //console.log("StyleManager.RegisterPackage - styleCategories 3 - ", t.categories);
            var styleCategories = [];
            t.categories.forEach((value, key, map) => {
                var styleCategory = { name: key, options: []};
                styleCategories.push(styleCategory);
                value.styles.forEach((v, k, m) => {
                    styleCategory.options.push(k);
                });
            });
            //console.log("StyleManager.RegisterPackage - styleCategories 4 - ", styleCategories);
            hularion.communicator.request({
                key: hularion.keyProvider().toString(),
                route: t.getNotifyCategoryUpdateRoute,
                detail: {
                    categories: styleCategories
                }
            });
        });
    },

    SelectCategory: function (categoryName, categoryValue) {
        var t = this;
        t.AddCategory(categoryName);
        t.categories.get(categoryName).value = categoryValue;
        t.SetCategory(categoryName);

    },

    AddCategory: function (categoryName) {
        var t = this;
        if (t.categories.has(categoryName)) { return; }
        var categoryContainer = document.createElement("div");
        categoryContainer.setAttribute("h-style-category", categoryName);
        t.categories.set(categoryName, {
            container: categoryContainer,
            value: null,
            styles: new Map()
        });
        t.categoryContainer.appendChild(categoryContainer);
    },

    SetCategory: function (categoryName) {
        var t = this;
        var category = t.categories.get(categoryName);
        var styles = [];
        if (category.styles.has(category.value)) {
            styles = category.styles.get(category.value);
        }

        category.container.replaceChildren();
        for (var i = 0; i < styles.length; i++) {
            category.container.appendChild(styles[i]);
        }
    }
}

