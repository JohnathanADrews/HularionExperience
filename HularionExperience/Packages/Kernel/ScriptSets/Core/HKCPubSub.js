/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


//HKC = Hularion Kernel Core
function HKCPubSub() {
    this.routes = new Map(); 
};
HKCPubSub.prototype = {

    Publish: function (route, object, parameter) {
        var t = this;
        if (!t.routes.has(route)) {
            t.routes.set(route, new Map());
        }
        //console.log("HKCPubSub.Publish - ", route, object, parameter, t);
        t.routes.get(route).forEach((v, k, m) => {
            k.Publish(object, parameter == null ? {} : parameter);
        });
    },

    Subscribe: function (subscriber) {
        var t = this;
        if (!t.routes.has(subscriber.route)) {
            t.routes.set(subscriber.route, new Map());
        }
        t.routes.get(subscriber.route).set(subscriber, subscriber);
    },

    Unsubscribe: function (subscriber) {
        if (t.routes.has(subscriber.route)) {
            t.routes.get(subscriber.route).delete(subscriber);
        }
    }
};



