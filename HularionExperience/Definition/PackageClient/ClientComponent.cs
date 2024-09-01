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
using System.Text;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientComponent
    {
        /// <summary>
        /// The frame of the component.
        /// </summary>
        public string Frame { get; set; }

        /// <summary>
        /// The alias of the component package.
        /// </summary>
        public string PackageAlias { get; set; }

        /// <summary>
        /// The presenter set of the component.
        /// </summary>
        public string SetName { get; set; }

        /// <summary>
        /// The name of the component presenter.
        /// </summary>
        public string PresenterName { get; set; }

        /// <summary>
        /// The parameter to send to the component.
        /// </summary>
        public object Parameter { get; set; }

        /// <summary>
        /// The parameter to pass to the constructor.
        /// </summary>
        public object ConstructorParameter { get; set; }

        /// <summary>
        /// The parameter to pass to the start method.
        /// </summary>
        public object StartParameter { get; set; }

        /// <summary>
        /// The name handle to the component in the presenter code object.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The handler of the target presenter that will handle this component.
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// The event=>method subscriptions to the component.
        /// </summary>
        public List<ClientSubscribeReference> Subscriptions { get; set; } = new List<ClientSubscribeReference>();

        /// <summary>
        /// The components added to this component.
        /// </summary>
        public List<ClientComponent> Components { get; set; } = new List<ClientComponent>();


        /// <summary>
        /// The style attribute on the component.
        /// </summary>
        public string ElementStyle { get; set; }

        /// <summary>
        /// The class attribute on the component.
        /// </summary>
        public string ElementClass { get; set; }

        /// <summary>
        /// Iff true, the component will be DOM element instead of a new presenter.
        /// </summary>
        public bool AsElement { get; set; } = false;

        /// <summary>
        /// The attributes assigned to this node.
        /// </summary>
        public List<ClientHtmlAttribute> HtmlAttributes { get; set; } = new List<ClientHtmlAttribute>();

        public ClientComponent()
        {

        }
        public ClientComponent(PresenterComponent component)
        {
            Frame = component.Frame;
            PackageAlias = component.PackageAlias;
            SetName = component.SetName;
            PresenterName = component.PresenterName;
            Parameter = component.Parameter;
            ConstructorParameter = component.ConstructorParameter;
            StartParameter = component.StartParameter;
            Handle = component.Handle;
            Handler = component.TargetHandler;
            ElementClass = component.CssClass;
            ElementStyle = component.Style;
            AsElement = component.AsElement;

            foreach(var subscription in component.Subscriptions)
            {
                Subscriptions.Add(new ClientSubscribeReference(subscription));
            }
        }

    }
}
