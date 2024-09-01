#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace HularionExperience.Definition.Project.Concept
{
    [PackageConcept]
    public class Presenter
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        [PackageConceptKey]
        public string Key { get; set; }

        public string Name { get; set; }

        public string View { get; set; }

        public string CssClass { get; set; }

        public DomeWrapper DomeWrapper { get; set; }

        public List<string> Scripts { get; set; } = new List<string>();

        public List<PresenterStyle> Styles { get; set; } = new List<PresenterStyle>();

        public List<PresenterHandle> Handles { get; set; } = new List<PresenterHandle>();

        public Dictionary<string, PresenterTemplate> Templates { get; set; } = new Dictionary<string, PresenterTemplate>();

        public Dictionary<string, PresenterClone> Clones { get; set; } = new Dictionary<string, PresenterClone>();

        public List<PresenterComponent> Components { get; set; } = new List<PresenterComponent>();

        public List<PresenterComponentHandler> ComponentHandlers { get; set; } = new List<PresenterComponentHandler>();

        public List<PresenterReference> Presenters { get; set; } = new List<PresenterReference>();

        public List<TemplateInstance> TemplateInstances { get; set; } = new List<TemplateInstance>();

        public List<CloneInstance> CloneInstances { get; set; } = new List<CloneInstance>();

        public List<ScriptFrameHandle> ScriptFrameHandles { get; set; } = new List<ScriptFrameHandle>();

        public List<PresenterFrameHandle> PresenterFrameHandles { get; set; } = new List<PresenterFrameHandle>();

        public List<PresenterProxy> PresenterProxies { get; set; } = new List<PresenterProxy>();

        public string ViewWrapper { get; set; }

        public List<PresenterPublisher> Publishers = new List<PresenterPublisher>();

        public List<PresenterSubscriber> Subscribers = new List<PresenterSubscriber>();

        public List<PresenterSystemSubscriber> SystemSubscribers = new List<PresenterSystemSubscriber>();

        public List<PresenterProxyInterface> InterfaceMethods = new List<PresenterProxyInterface>();

        public List<PresenterGraphicReference> GraphicReferences = new List<PresenterGraphicReference>();

        public List<PresenterInterface> Interfaces { get; set; } = new();

        public Dictionary<string, PresenterMemberInfo> Members = new Dictionary<string, PresenterMemberInfo>();

    }
}
