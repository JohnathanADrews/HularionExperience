/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function ResourceManager(name) {
    this.map = new Map();
    this.name = name;
    this.promiseProto = (hularion.Control.Promise(null)).__proto__.constructor.name;
}
ResourceManager.prototype = {
    LoadResourceTree: function (keys, subKeysProvider, resourceProvider, resourceInitializer, resourceFinalizer) {
        //console.log("ResourceManager.LoadResourceTree  - ", keys, subKeysProvider, resourceProvider, resourceProvider.__proto__);
        var t = this;

        var waiting = new Map();
        var loader = (key, parent, stack) => {
            //console.log("ResourceManager.loader 1  keys- ", key, "-->", parent);
            //console.log("ResourceManager.loader 2  keys- ", key, "-->", parent, stack);
            if (t.map.has(key)) { return; }
            var o = { key: key, value: null, isLoaded: false, promise: null, attach: {}, children: new Map(), parent: parent };
            if (parent != null) { parent.children.set(key, o); }
            t.map.set(key, o);
            waiting.set(key, false);

            var done = false;
            resourceProvider(key).then(x => {
                //console.log("ResourceManager.LoadResourceTree 0  keys- ", key, x);
                o.value = x;
                var keys = subKeysProvider(x);
                var waiter = new HKCWaiter();
                var sync = waiter.CreateSynchronizer();
                //console.log("ResourceManager.LoadResourceTree 1  keys- ", key, "-->", keys, " ---- ", x);
                var promises = [];
                hularion.Control.ProcessArray(keys, k => {
                    var s = [];
                    for (var i = 0; i < stack.length; i++) { s.push(stack[i]) }
                    s.push(k);
                    var p = loader(k, o, s)
                    promises.push(p);
                    sync.AddItem(() => p);
                });
                //waiter.Start();
                //sync.Start();
                //sync.When().then(() => {
                Promise.all(promises).then(() => {
                    //console.log("ResourceManager.LoadResourceTree 2 keys- ", key, "-->", keys, " ---- ", o.value);
                    if (resourceInitializer != null) { resourceInitializer(o.value, o.key); }
                    o.isLoaded = true;
                    o.promise = hularion.Control.Promise(o.value);
                    //console.log("ResourceManager.LoadResourceTree  loaded - ", key, t.map.get(key));
                    waiting.set(key, true);
                    done = true;
                    //console.log("ResourceManager.LoadResourceTree 3 keys- ", key, "-->", keys, " ---- ", o.value);
                });
            });
            o.promise = hularion.constants.WaitClimb(() => done === true).then(() => {
                if (resourceFinalizer != null) { resourceFinalizer(o.value, o.key); }
                return o;
            });

            //o.promise.catch(() => {
            //    console.warn("ResourceManager.LoadResourceTree - package load wait - ", keys);
            //});

            return o.promise;
        };

        hularion.Control.ProcessArray(keys, key=> loader(key, null, [key]));

        return hularion.constants.WaitClimb(() => {
            waiting.forEach((value, key, map) => {
                if (value === true) { waiting.delete(key); }
            });
            return (waiting.size == 0);
        }).then(() => {
            return t.GetValues(keys);
        });
    },

    GetKeys: function () {
        var t = this;
        var result = [];
        t.map.forEach((value, key, map) => {
            result.push(key);
        });
        return result;
    },

    Has: function (key) {
        return this.map.has(key);
    },

    GetValue: function (key) {
        var t = this;
        if (!t.map.has(key)) { return null; }
        var o = t.map.get(key);
        if (!o.isLoaded) { return null; }
        if (o == null) { return null; }
        return o.value;
    },

    GetValues: function (keys) {
        var t = this;
        var result = {};
        hularion.Control.ProcessArray(keys, key => {
            result[key] = t.GetValue(key);
        });
        return result;
    },

    When: function (key) {
        var t = this;
        var done = false;
        hularion.constants.WaitClimb(() => (t.map.has(key))).then(() => {
            hasKey = true;
            var o = t.map.get(key);
            if (o != null) { o.promise.then(() => { done = true; }); }            
        });
        return hularion.constants.WaitClimb(() => (done === true)).then(() => t.map.get(key).value);
    },

    AssociateValue: function (name, key, value) {
        var t = this;
        if (!t.map.has(key)) { return false; }
        var o = t.map.get(key);
        o.attach[name] = value;
        return true;
    },

    GetAssociatedValue: function (name, key) {
        var t = this;
        if (!t.map.has(key)) { return null; }
        var o = t.map.get(key);
        return o.attach[name];
    },

    GetParent: function (key) {
        var t = this;
        if (!t.map.has(key)) { return null; }
        var p = t.map.get(key).parent;
        if (p == null) { return null; }
        return { key: p.key, value: p.value };
    },

    GetChildren: function (key) {
        var t = this;
        //console.log("ResourceManager.GetChildren - ", key, t.map.get(key));
        if (!t.map.has(key)) { return []; }
        var children = t.map.get(key).children;
        //console.log("ResourceManager.GetChildren - ", key, children);
        if (children == null) { return []; }
        var result = [];
        children.forEach((value, key, map) => { result.push({ key: key, value: value.value }); });
        //console.log("ResourceManager.GetChildren - ", key, result);
        return result;
    }
}


