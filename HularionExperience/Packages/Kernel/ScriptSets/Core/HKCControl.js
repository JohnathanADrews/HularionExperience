/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


//HKC = Hularion Kernel Core
function HKCControl() {

};
HKCControl.prototype = {

    GetObjectMemberNames: function (object) {
        return Object.getOwnPropertyNames(object);
    },

    GetObjectMemberValues: function (object) {
        var names = Object.getOwnPropertyNames(object);
        var result = [];
        for (var i = 0; i < names.length; i++) {
            result.push(object[names[i]]);
        }
        return result;
    },

    GetObjectMemberNameValuePairs: function (object) {
        if (object == null) { return []; }
        var names = Object.getOwnPropertyNames(object);
        var result = [];
        for (var i = 0; i < names.length; i++) {
            result.push({ name: names[i], value: object[names[i]] });
        }
        return result;
    },

    ProcessObjectMemberValues: function (object, processor) {
        this.ProcessObjectMemberNameValuePairs(object, pair => processor(pair.value));
    },

    ProcessObjectMemberNameValuePairs: function (object, processor) {
        var pairs = this.GetObjectMemberNameValuePairs(object);
        this.ProcessArray(pairs, pair => processor(pair));
    },

    ClearMembers: function (object) {
        this.ProcessArray(this.GetObjectMemberNames(object), name => { delete object[name]; });
    },

    ProjectOnto: function (source, destination, deleteExcess) {
        if (deleteExcess) { this.ClearMembers(destination); }
        var pairs = this.GetObjectMemberNameValuePairs(source);
        this.ProcessArray(pairs, pair => { destination[pair.name] = pair.value; });
    },

    ProcessArray: function (array, processor) {
        if (array == null) { return; }
        for (var i = 0; i < array.length; i++) {
            if (processor(array[i], i) === false) { return false; }
        }
        return true;
    },

    ProcessReverseArray: function (array, processor) {
        if (array == null) { return; }
        for (var i = array.length - 1; i >= 0; i--) {
            if (processor(array[i], i) === false) { return false; }
        }
        return true;
    },

    ProcessIterator: function (min, max, processor) {
        for (var i = min; i <= max; i++) {
            if (processor(i) === false) { return false; }
        }
        return true;
    },

    ProcessReverseIterator: function (max, min, processor) {
        for (var i = max; i >= min; i--) {
            if (processor(i) === false) { return false; }
        }
        return true;
    },

    Iterate: function (boundary1, boundary2, processor) {
        if (boundary1 <= boundary2) { return this.ProcessIterator(boundary1, boundary2, processor); }
        else { return this.ProcessReverseIterator(boundary1, boundary2, processor); }
    },

    //example: object x = {}, reference "abc.defg.hij", result = x = { abc: { defg: { hij: value }}};
    SetObjectAtReference: function (start, reference, value, delimiter) {
        var parts = reference.split(delimiter);
        var node = start;
        this.ProcessIterator(0, parts.length - 2, i => {
            var part = parts[i];
            if (part != null && part.trim() != "" && node[part] == null) { node[part] = {}; }
            node = node[part];
        });
        node[parts[parts.length - 1]] = value;
    },

    OrderPriority: function (priority, items, comparer) {
        var t = this;
        var result = [];
        if (items == null || items.length == 0) { return result; }
        if (items.length == 1 || priority == null || priority.length == 0) { return items; }
        if (comparer == null) { comparer = (a, b) => (a == b); }
        var ia = [];
        t.ProcessArray(items, item => {
            var ip = -1;
            var fp = -1;
            t.ProcessIterator(0, priority.length - 1, i => {
                if (comparer(priority[i], item[i]) === true) {
                    if (fp == -1) { fp = i; }
                    ip = i;
                }
                else if (fp != -1) { return false; }
            });
            ia.push({ i: item, f: fp, l: ip - fp });
        });
        ia.sort((a, b) => ((a.f - b.f) * priority.length + (b.l - a.l)));
        t.ProcessArray(ia, ir => { result.push(ir.i); });
        return result;
    },

    TransformEach: function (array, transform) {
        var result = [];
        if (array == null || array.length == 0) { return []; }
        if (transform == null) { return array; }
        this.ProcessArray(array, a => result.push(transform(a)));
        return result;
    },

    Wait: function (waitTime, maxWaits, isReadyFunc, resultProvider, climb) {
        var result = { waitTime: waitTime, maxWaits: maxWaits, isReadyFunc: isReadyFunc, waits: 0 };
        var time = 1;
        if (climb == null) { climb = true; }
        return new Promise((resolve, reject) => {
            var waiter = () => {
                if (isReadyFunc()) {
                    if (resultProvider != null) { result.result = resultProvider(); }
                    resolve(result);
                }
                else {
                    if (++(result.waits) > maxWaits) { reject(result); return; }
                    if (climb) {
                        time *= 2;
                        if (time >= waitTime) {
                            time = waitTime;
                            climb = false;
                        }
                    }
                    setTimeout(function () { waiter(); }, time);
                }                 
            }
            waiter();
        });
    },

    WaitClimb: function (totalTime, maxTimeout, isReadyFunc) {
        var result = { totalTime: totalTime, maxTimeout: maxTimeout, isReadyFunc: isReadyFunc, waits: 0, elapsed: 0 };
        var time =1;
        var climb = true;
        return new Promise((resolve, reject) => {
            var waiter = () => {
                if (isReadyFunc(result)) { resolve(result); }
                else {
                    result.elapsed += time;
                    result.waits++;
                    var difference = totalTime - result.elapsed;
                    if (difference <= 0) { reject(result); return; }
                    if (climb) {
                        time *= 2;
                        if (time >= maxTimeout) {
                            time = maxTimeout;
                            climb = false;
                        }
                    }
                    if (difference < time) { time = difference; }
                    setTimeout(function () { waiter(); }, time);
                }
            }
            waiter();
        });
    },

    Delay: function (time) {
        var done = false;
        return this.Wait(time, 1, () => {
            var x = done ? true : false;
            done = true;
            return x;
        }, () => { }, false);
    },

    Promise: function (result) {
        return new Promise((resolve, reject) => { resolve(result); });
    },

    PromiseOrder: function (promises) {
        var t = this;
        if (promises == null || promises.length == 0) { return hularion.Control.Promise([]); }

        var pMap = new Map();
        var count = 0;
        hularion.Control.ProcessArray(promises, promise => {
            var c = ++count;
            var item = { };
            promise.then(x => item.x = x);
            pMap.set(c, item);
        });
        return Promise.all(promises, () => {
            var result = [];
            hularion.Control.ProcessIterator(1, count, i => {
                result.push(pMap.get(i).x);
            });
            return result;
        });
    },

    ParameterizeCall: function (caller, parameters) {
        var t = this;
        if (parameters == null || parameters.length == 0) { return caller(); }
        var newCall = ["caller("];
        t.ProcessIterator(0, parameters.length - 1, i => {
            if (i > 0) { newCall.push(","); }
            newCall.push("parameters[");
            newCall.push(i);
            newCall.push("]");
        });
        newCall.push(");");
        return eval(newCall.join(""));
    },

    ParameterizeObjectCall: function (object, method, parameters) {
        var t = this;
        if (parameters == null || parameters.length == 0) { return object[method](); }
        var newCall = ["object[method]("];
        t.ProcessIterator(0, parameters.length - 1, i => {
            if (i > 0) { newCall.push(","); }
            newCall.push("parameters[");
            newCall.push(i);
            newCall.push("]");
        });
        newCall.push(");");
        return eval(newCall.join(""));
    },

    CreateInterfaceEvalText: function (method, methodName) {
        var x = "x";
        var parameters = [];
        this.ProcessIterator(0, method.length - 1, i => { parameters.push(x + i); })
        var newCall = ["("];
        this.ProcessIterator(0, method.length - 1, i => {
            if (i > 0) { newCall.push(","); }
            newCall.push(x + i);
        });
        newCall.push(") => ");
        newCall.push(methodName);
        newCall.push("(");
        this.ProcessIterator(0, method.length - 1, i => {
            if (i > 0) { newCall.push(","); }
            newCall.push(x + i);
        });
        newCall.push(");");
        return newCall.join("");
    }


};
