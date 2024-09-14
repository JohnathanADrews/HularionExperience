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
    public class TemplateInstance
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The index of the html element this corresponds to.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// The name of the template this is an instance of.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The handle to this instance in the presenter.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The encoded parameter string.
        /// </summary>
        public string Parameter { get; set; }

        /// <summary>
        /// The decoded parameter to send to the template handler.
        /// </summary>
        public object DecodedParameter { get; set; }

        /// <summary>
        /// The CSS class set on the template.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// The HTML style set on the template.
        /// </summary>
        public string Style { get; set; }

    }
}
