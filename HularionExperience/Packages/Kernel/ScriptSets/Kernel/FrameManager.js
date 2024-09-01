/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function FrameManager(frameLoader, thisReference, frameContainer) {
    this.frameLoader = frameLoader;
    this.reference = thisReference;
    this.frameContainer = frameContainer;
    this.frames = {};
}
FrameManager.prototype = {
    CreateFrame: function () {
        var t = this;
        var key = "FrameKey-" + hularion.keyProvider();
        var frame = { key: key, isLoaded: false };
        t.frames[key] = frame;
        var loader = t.frameLoader.replace("$$FrameKey$$", key);
        loader = loader.replace("$$Reference$$", "window.parent." + this.reference);

        var iframe = document.createElement("iframe");
        frame.attachor = iframe;
        iframe.setAttribute("style", "display:none;");
        iframe.setAttribute("srcDoc", loader);
        t.frameContainer.appendChild(iframe);
        frame.element = document.querySelector("body")[0];

        var result = { key: key };
        result.promise = hularion.Control.Wait(50, 40, () => { return t.frames[key] != null && t.frames[key].isLoaded === true; }).then(() => t.frames[key]);
        return result;
    },
    FrameLoadComplete: function (key, handle) {
        //handle: the handle to the script object within the frame.
        var t = this;
        var frame = t.frames[key];
        frame.handle = handle;
        frame.isLoaded = true;
    },
    GetFrame: function (key) {
        return this.frames[key];
    }
}