/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/



function ScriptFrame(properties) {
    hularion.Set.InitializeObject(this, properties, {
        set: null,
        mainFrame: null
    });

    this.collaboratorMap = new Map();
    this.waiter = hularion.constants.CreateWaiter();
};
ScriptFrame.AttachMode = { Inject: "inject", Frame: "frame" };
ScriptFrame.AttachOrder = { Before: "before", After: "after" };
ScriptFrame.AttachScriptReferences = function (frame, references, attach, mode, order) {
    //console.log("ScriptFrame.AttachScriptReferences - ", frame.set.name, frame, references, attach, mode, order);
    hularion.Control.ProcessArray(references, sRef => {
        //console.log("ScriptFrame.AttachScriptReferences - sRef 1 -", sRef);
        if (sRef.attach == null || mode != sRef.attach.toLowerCase()) { return; }
  //      console.log("ScriptFrame.AttachScriptReferences - sRef 2 -", sRef, order, sRef.order);
        //if (order != null && order != sRef.order.toLowerCase()) { return; }
        if (mode != PresentationFrame.AttachMode.Inject && order != sRef.order.toLowerCase()) { return; }
  //      console.log("ScriptFrame.AttachScriptReferences - sRef 3 -", sRef);
        //console.log("ScriptFrame.AttachScriptReferences - sRef__4 -", frame.set.name, "-->", sRef.frame, frame.frames[sRef.frame].frame == null, frame, sRef, attach, frame.frames[sRef.frame].frame);
        if (frame.frames[sRef.frame] == null) {
            console.warn("ScriptFrame.AttachScriptReferences - reference frame not found - ", frame.node.package.key, frame.node.package.name, frame.set.name, "-->", sRef.frame, frame.frames, sRef, frame);
            return;
        }
        var o = {};
        frame.frames[sRef.frame].frame.mainFrame.handle.assign(o, sRef.assign);
        hularion.Control.SetObjectAtReference(attach, sRef.handle, o.out, ".");
    });

}
ScriptFrame.prototype = {
    Setup: function () {
        var t = this;

        //console.log("ScriptFrame.Initialize - ", t);
        ScriptFrame.AttachScriptReferences(t, t.set.scriptFrameHandles, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, ScriptFrame.AttachOrder.Before);
        PresentationFrame.AttachPresenterReferences(t, t.set.presenterReferences, t.mainFrame.attachor.contentWindow, PresentationFrame.AttachMode.Frame, ScriptFrame.AttachOrder.Before);

        hularion.Control.ProcessArray(t.set.attachIndicators, indicator => {
            if (indicator.objectName == "DataTypes") {
                //console.log("ScriptFrame.Initialize attachIndicators - ", t.set.name, t);
            }
            if (indicator.objectName == "Communicator") {
                //console.log("ScriptFrame.Initialize attachIndicators 1 - ", t.set.name, indicator, t, t.mainFrame.handle.spark);
                hularion.Control.SetObjectAtReference(t.mainFrame.attachor.contentWindow, indicator.handle, t.communicator, ".");
                //console.log("ScriptFrame.Initialize attachIndicators 2 - ", t.set.name, indicator, t, t.mainFrame.handle.spark);
            }
        });

        hularion.Control.ProcessArray(t.set.scripts, script => {
            t.mainFrame.handle.loadScript(script.content);
        });


    },

    Start: function () {
        var t = this;
        //console.log("ScriptFrame.Start - ", t.set.name, t);
        ScriptFrame.AttachScriptReferences(t, t.set.scriptFrameHandles, t.mainFrame.attachor.contentWindow, ScriptFrame.AttachMode.Frame, ScriptFrame.AttachOrder.After);
        PresentationFrame.AttachPresenterReferences(t, t.set.presenterReferences, t.mainFrame.attachor.contentWindow, PresentationFrame.AttachMode.Frame, ScriptFrame.AttachOrder.After);

    },

    When: function () {
        var t = this;
        return hularion.Control.Promise(t);
        //return t.waiter.When();
    }

}

