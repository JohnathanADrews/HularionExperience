/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


//HKC = Hularion Kernel Core
function HKCPubSubSubscriber(route, callback, options) {
    this.route = route;
    this.callback = (o,v) => callback(o,v);
    this.messages = [];
    this.paused = false;
    this.rety

    hularion.Set.InitializeObject(this, options, {
        paused: false,
        retryFailed: false
    });

}
HKCPubSubSubscriber.prototype = {
    Publish: function (source, message) {
        var t = this;
        t.messages.push(message);
        if (t.paused === true) { return; }
        var send = [];
        for (var i = 0; i < t.messages.length; i++) {
            send.push(t.messages[i]);
        }
        var last = 0;
        for (var i = 0; i < send.length; i++) {
            var result = false;
            try {
                //console.log("HKCPubSubSubscriber.Publish - ", source, message, t.callback);
                result = t.callback(source, send[i]);
            }
            catch { result = false; }
            if (result !== true && t.retryFailed) {

            }
        }
        t.messages.splice(0, send.length);
    },
    //ReadAll: function () {

    //},
    //Pause: function () {
    //    this.paused = false;
    //},
    //Resume: function (options) {
    //    var t = this;
    //    t.paused = false;
    //}
};
