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
    /// <summary>
    /// In a presenter, an element with a "h-component" attribute will indicate that the element is to be replaced with an instance of the indicated presenter.
    /// Those details are described here.
    /// </summary>
    [PackageConcept]
    public class PresenterComponent
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        [PackageConceptKey]
        public string Key { get; set; }

        /// <summary>
        /// The index of the node this refers to.
        /// </summary>
        public string Index { get; set; }

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
        public string Parameter { get; set; }

        /// <summary>
        /// The decoded parameter to send to the component.
        /// </summary>
        public object DecodedParameter { get; set; }

        /// <summary>
        /// The name handle to the component in the presenter code object.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The handler of the target presenter that will handle this component.
        /// </summary>
        public string TargetHandler { get; set; }

        /// <summary>
        /// The index of the parent component.
        /// </summary>
        public string ParentIndex { get; set; }

        /// <summary>
        /// True iff this component exists within a template.
        /// </summary>
        public bool IsTemplate { get; set; } = false;

        /// <summary>
        /// The CSS class set on the component.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// The HTML style set on the component.
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// The parameter to pass to the constructor.
        /// </summary>
        public string ConstructorParameter { get; set; }

        /// <summary>
        /// The parameter to pass to the start method.
        /// </summary>
        public string StartParameter { get; set; }

        /// <summary>
        /// Iff true, the component will be DOM element instead of a new presenter.
        /// </summary>
        public bool AsElement { get; set; } = false;

        /// <summary>
        /// Components that are not presenters get treated like templates, and each gets its own template id.
        /// </summary>
        public int ComponentTemplateId { get; set; }

        /// <summary>
        /// The event=>method subscriptions to the component.
        /// </summary>
        public List<Subscriptioneference> Subscriptions { get; set; } = new List<Subscriptioneference>();

    }
}
