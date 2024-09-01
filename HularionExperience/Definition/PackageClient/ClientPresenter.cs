#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientPresenter
    {

        public string Key { get; set; }

        public string Name { get; set; }

        public string View { get; set; }

        public string CssClass { get; set; }

        public List<ClientScript> Scripts { get; set; } = new List<ClientScript>();

        public List<ClientPresenterHandle> Handles { get; set; } = new List<ClientPresenterHandle>();

        public Dictionary<string, ClientTemplate> Templates { get; set; } = new Dictionary<string, ClientTemplate>();

        public List<ClientTemplateInstance> TemplateInstances { get; set; } = new List<ClientTemplateInstance>();

        public Dictionary<string, ClientClone> Clones { get; set; } = new Dictionary<string, ClientClone>();

        public List<ClientCloneInstance> CloneInstances { get; set; } = new List<ClientCloneInstance>();

        public List<ClientPresenterReference> Presenters { get; set; } = new List<ClientPresenterReference>();

        public List<ClientComponent> Components { get; set; } = new List<ClientComponent>();

        public Dictionary<string, ClientComponentHandler> ComponentHandlers { get; set; } = new Dictionary<string, ClientComponentHandler>();

        public List<ClientCommunicator> Communicators { get; set; } = new List<ClientCommunicator>();

        public List<ClientDataSource> DataSources { get; set; } = new List<ClientDataSource>();

        public List<ClientScriptReference> ScriptReferences { get; set; } = new List<ClientScriptReference>();

        public List<ClientPresenterFrameHandle> PresenterReferences { get; set; } = new List<ClientPresenterFrameHandle>();

        public List<ClientPresenterProxy> PresenterProxies { get; set; } = new List<ClientPresenterProxy>();

        public Dictionary<string, ClientPresenterPublisher> Publishers { get; set; } = new Dictionary<string, ClientPresenterPublisher>();

        public Dictionary<string, ClientPresenterSubscriber> Subscribers { get; set; } = new Dictionary<string, ClientPresenterSubscriber>();

        public Dictionary<string, ClientPresenterSystemSubscriber> SystemSubscribers { get; set; } = new Dictionary<string, ClientPresenterSystemSubscriber>();

        public string ViewWrapper { get; set; }

        public List<ClientPresenterProxyInterface> ProxyInterfaces { get; set; } = new List<ClientPresenterProxyInterface>();

        public List<ClientPresenterGraphicReference> Graphics { get; set; } = new List<ClientPresenterGraphicReference>();

        public List<ClientPresenterInterface> Interfaces { get; set; } = new();

        public List<ClientPresenterMemberInfo> Members { get; set; } = new List<ClientPresenterMemberInfo>();


        /// <summary>
        /// The event=>method subscriptions to the component.
        /// </summary>
        public List<ClientSubscribeReference> Subscribes { get; set; } = new List<ClientSubscribeReference>();

        public ClientPresenter()
        {

        }

        public ClientPresenter(Presenter presenter)
        {
            Key = presenter.Key;
            Name = presenter.Name;
            View = presenter.View;
            ViewWrapper = presenter.ViewWrapper;
            Scripts = presenter.Scripts.Select(x => new ClientScript() { Content = x }).ToList();
            //styles = presenter.Styles.Select(x => new WebStyle(x)).ToList();
            CssClass = presenter.CssClass;
            Handles = presenter.Handles.Select(x => new ClientPresenterHandle(x)).ToList();
            Templates = presenter.Templates.ToDictionary(x => x.Key, x => new ClientTemplate(x.Value));
            TemplateInstances = presenter.TemplateInstances.Select(x => new ClientTemplateInstance(x)).ToList();
            Clones = presenter.Clones.ToDictionary(x => x.Key, x => new ClientClone(x.Value));
            CloneInstances = presenter.CloneInstances.Select(x => new ClientCloneInstance(x)).ToList();
            Presenters = presenter.Presenters.Select(x => new ClientPresenterReference(x)).ToList();
            ComponentHandlers = presenter.ComponentHandlers.ToDictionary(x=>x.Handler, x => new ClientComponentHandler(x));
            ScriptReferences = presenter.ScriptFrameHandles.Select(x => new ClientScriptReference(x)).ToList();
            PresenterReferences = presenter.PresenterFrameHandles.Select(x => new ClientPresenterFrameHandle(x)).ToList();
            PresenterProxies = presenter.PresenterProxies.Select(x => new ClientPresenterProxy(x)).ToList();
            ProxyInterfaces = presenter.InterfaceMethods.Select(x => new ClientPresenterProxyInterface(x)).ToList();
            Graphics = presenter.GraphicReferences.Select(x => new ClientPresenterGraphicReference(x)).ToList();
            Interfaces = presenter.Interfaces.Select(x=>new ClientPresenterInterface(x)).ToList();
            Members = presenter.Members.Values.Select(x => new ClientPresenterMemberInfo(x)).ToList();
            foreach (var publisher in presenter.Publishers)
            {
                if (Publishers.ContainsKey(publisher.Name)) { Publishers.Remove(publisher.Name); }
                Publishers.Add(publisher.Name, new ClientPresenterPublisher(publisher));
            }
            foreach (var subscriber in presenter.Subscribers)
            {
                if (Subscribers.ContainsKey(subscriber.Name)) { Subscribers.Remove(subscriber.Name); }
                Subscribers.Add(subscriber.Name, new ClientPresenterSubscriber(subscriber));
            }
            foreach (var subscriber in presenter.SystemSubscribers)
            {
                if (SystemSubscribers.ContainsKey(subscriber.Name)) { Subscribers.Remove(subscriber.Name); }
                SystemSubscribers.Add(subscriber.Name, new ClientPresenterSystemSubscriber(subscriber));
            }

            //Handle the component stuff.
            var indexMap = presenter.Components.ToDictionary(x => x.Index, x => x);
            var componentMap = presenter.Components.ToDictionary(x => x, x => new ClientComponent(x));
            var indexedPresenters = Presenters.ToDictionary(x => x.Index, x => x);
            foreach(var component in presenter.Components)
            {
                var webComponent = componentMap[component];
                if (component.ParentIndex == null)
                {
                    if (!component.IsTemplate) { Components.Add(webComponent); }
                }
                else
                {
                    if (!indexMap.ContainsKey(component.ParentIndex)) 
                    {
                        //Parent, is not a component.  It could be a presenter reference.
                        if (indexedPresenters.ContainsKey(component.ParentIndex))
                        {
                            indexedPresenters[component.ParentIndex].Components.Add(webComponent);
                        }
                        continue; 
                    }
                    var parent = indexMap[component.ParentIndex];
                    componentMap[parent].Components.Add(webComponent);
                }
            }

            foreach (var template in presenter.Templates)
            {
                var webTemplate = Templates[template.Key];
                foreach(var component in template.Value.Components)
                {
                    webTemplate.Components.Add(componentMap[component]);
                }
                foreach(var p in webTemplate.Presenters)
                {
                    var tp = template.Value.Presenters.Where(x => x.Index == p.Index).FirstOrDefault();
                    if(tp == null) { continue; }
                    foreach (var component in tp.Components)
                    {
                        p.Components.Add(componentMap[component]);
                        //if(component.ParentIndex == p.index)
                        //{
                        //}
                    }
                }

            }

        }

    }
}
