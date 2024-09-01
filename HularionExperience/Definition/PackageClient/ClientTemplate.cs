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
    public class ClientTemplate
    {

        public string Name { get; set; }

        public string View { get; set; }

        public string Method { get; set; }

        public string MethodSplit { get; set; }

        public string Wrapper { get; set; }

        public List<ClientPresenterHandle> Handles { get; set; } = new List<ClientPresenterHandle>();

        public List<ClientPresenterReference> Presenters { get; set; } = new List<ClientPresenterReference>();

        public List<ClientComponent> Components { get; set; } = new List<ClientComponent>();


        public List<ClientTemplateInstance> TemplateInstances { get; set; } = new List<ClientTemplateInstance>();

        public List<ClientCloneInstance> CloneInstances { get; set; } = new List<ClientCloneInstance>();


        /// <summary>
        /// The style attribute on the component.
        /// </summary>
        public string ElementStyle { get; set; }

        /// <summary>
        /// The class attribute on the component.
        /// </summary>
        public string ElementClass { get; set; }

        /// <summary>
        /// The attributes assigned to this node. They will be added to each instance unless overriden by that instance.
        /// </summary>
        public List<ClientHtmlAttribute> HtmlAttributes { get; set; } = new List<ClientHtmlAttribute>();

        public ClientTemplate()
        {

        }
        public ClientTemplate(PresenterTemplate template)
        {
            Name = template.Name;
            View = template.View;
            Method = template.Method;
            MethodSplit = template.EncodingType;
            Handles = template.Handles.Select(x => new ClientPresenterHandle(x)).ToList();
            Presenters = template.Presenters.Select(x => new ClientPresenterReference(x)).ToList();
            TemplateInstances = template.TemplateInstances.Select(x => new ClientTemplateInstance(x)).ToList();
            CloneInstances = template.CloneInstances.Select(x => new ClientCloneInstance(x)).ToList();
        }
    }
}
