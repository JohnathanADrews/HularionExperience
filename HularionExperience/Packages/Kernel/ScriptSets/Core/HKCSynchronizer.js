/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


//HKC = Hularion Kernel Core
//Executes a series of functions returning promises in synchronized order, and with the provided maxTimeout.
function HKCSynchronizer(totalTime, maxTimeout, waiter) {
    this.totalTime = totalTime;
    this.maxTimeout = maxTimeout;
    this.waiter = waiter;
    if (this.totalTime == null) { this.totalTime = 2000; }
    if (this.maxTimeout == null) { this.maxTimeout = 20; }
    this.map = new Map();
    this.index = 0;
    this.isDone = false;
    this.waitIndex = 0;
    if (this.waiter != null) { this.waitIndex = this.waiter.AddWaiter(); }
};
HKCSynchronizer.prototype = {

    AddItem: function (func) {
        var t = this;
        t.map.set(t.index++, func);
    },

    Start: function () {
        var t = this;
        t.Next(0);
        return t.When();
    },

    Next: function (i) {
        var t = this;
        if (i >= t.index) {
            t.waiter.RemoveWaiter(t.waitIndex);
            t.isDone = true;
            return;
        }
        t.map.get(i)().then(() => t.Next(i + 1));
    },

    When: function () {
        var t = this;
        return hularion.Control.WaitClimb(t.totalTime, t.maxTimeout, () => { return t.isDone; }).then(() => true);
    }

}
