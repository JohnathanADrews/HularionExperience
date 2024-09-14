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
    public class ClientPresenterReference
    {

        /// <summary>
        /// The index of the node this refers to.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// The name of the presenter.
        /// </summary>
        public string PresenterName { get; set; }

        /// <summary>
        /// The name handle to the component in the presenter code object.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The frame that will produce the presenter.  
        /// </summary>
        public string Frame { get; set; }

        public string Attach { get; set; }

        /// <summary>
        /// The components to send to the this presenter.
        /// </summary>
        public List<ClientComponent> Components { get; set; } = new List<ClientComponent>();


        /// <summary>
        /// The style attribute on the presenter.
        /// </summary>
        public string ElementStyle { get; set; }

        /// <summary>
        /// The class attribute on the presenter.
        /// </summary>
        public string ElementClass { get; set; }

        /// <summary>
        /// The parameter to pass to the constructor.
        /// </summary>
        public object ConstructorParameter { get; set; }

        /// <summary>
        /// The parameter to pass to the start method.
        /// </summary>
        public object StartParameter { get; set; }

        /// <summary>
        /// The attributes assigned to this node.
        /// </summary>
        public List<ClientHtmlAttribute> HtmlAttributes { get; set; } = new List<ClientHtmlAttribute>();

        /// <summary>
        /// The event=>method subscriptions to the component.
        /// </summary>
        public List<ClientSubscribeReference> Subscriptions { get; set; } = new List<ClientSubscribeReference>();

        public ClientPresenterReference()
        {

        }
        public ClientPresenterReference(PresenterReference reference)
        {
            Index = reference.Index;
            //packageAlias = reference.PackageAlias;
            //setName = reference.SetName;
            PresenterName = reference.PresenterName;
            Handle = reference.Handle;
            Frame = reference.Frame;
            ElementStyle = reference.Style;
            ElementClass = reference.CssClass;
            ConstructorParameter = reference.ConstructorParameter;
            StartParameter = reference.StartParameter;
            foreach(var subscription in reference.Subscriptions)
            {
                Subscriptions.Add(new ClientSubscribeReference(subscription));
            }
        }

    }
}
