/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/


function hStart(initializer, requestProcessor) {

    window.hInit = (w) => {
        var scriptContainer = w.document.createElement("div");
        scriptContainer.setAttribute("class", "kernelScript");
        w.document.body.appendChild(scriptContainer);
        var loader = script => {
            var se = w.document.createElement("script");
            se.innerHTML = script;
            scriptContainer.appendChild(se);
        };

        var loadPackage = pkg => {
            setNames = Object.getOwnPropertyNames(pkg.scriptSets);
            for (var j = 0; j < setNames.length; j++) {
                var scriptSet = pkg.scriptSets[setNames[j]];
                for (var k = 0; k < scriptSet.scripts.length; k++) {
                    loader(scriptSet.scripts[k].content);
                }
            }
        };

        loadPackage(initializer.kernelPackage);

        new w.HularionConstructor({
            initializer: initializer,
            requestProcessor: requestProcessor
        });
        delete window.hInit;
    };

    var loadIFrame = () => {
        var iframe = window.document.createElement("iframe");
        iframe.setAttribute("style", "display:none;");
        var iframeContainer = window.document.createElement("div");
        iframeContainer.setAttribute("style", "display:none;");
        iframeContainer.setAttribute("class", "hularion-boot-frame");
        iframeContainer.append(iframe);
        iframe.setAttribute("srcDoc", initializer.iFrameKernelLoader);
        window.document.getElementsByTagName("body")[0].appendChild(iframeContainer);
    };

    loadIFrame();
}