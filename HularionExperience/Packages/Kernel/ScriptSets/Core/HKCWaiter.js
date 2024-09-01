/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



//HKC = Hularion Kernel Core
function HKCWaiter(totalTime, maxTimeout) {
    this.totalTime = totalTime;
    this.maxTimeout = maxTimeout;
    if (this.totalTime == null) { this.totalTime = 2000; }
    if (this.maxTimeout == null) { this.maxTimeout = 20; }
    this.index = 0;
    this.isCompleted = false;
    this.waits = new Map();
};
HKCWaiter.prototype = {

    AddWaiter: function () {
        var t = this;
        var index = t.index++;
        t.waits.set(index, index);
        return index;
    },

    RemoveWaiter: function (index) {
        var t = this;
        t.waits.delete(index);
    },

    Start: function () {
        var t = this;
        hularion.Control.WaitClimb(t.totalTime, t.maxTimeout, () => {
            return (t.waits.size == 0);
        }).then(() => { t.isCompleted = true; });
        return t.When();
    },

    When: function () {
        var t = this;
        return hularion.Control.WaitClimb(t.totalTime, t.maxTimeout, () => { return (t.isCompleted === true); }).then(() => true);
    },

    CreateSynchronizer: function () {
        var t = this;
        return new HKCSynchronizer(t.totalTime, t.maxTimeout, t);
    }

};



