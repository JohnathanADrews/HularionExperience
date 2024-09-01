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
    public class PresenterTemplate
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        public string Key { get; set; }

        public string Name { get; set; }

        public string View { get; set; }

        public string Method { get; set; }

        public string Encoding { get; set; }

        public string EncodingType { get; set; }

        public string EncodingDelimiter { get; set; }

        public List<PresenterHandle> Handles { get; set; } = new List<PresenterHandle>();

        public List<PresenterReference> Presenters = new List<PresenterReference>();

        public List<PresenterComponent> Components = new List<PresenterComponent>();

        public List<TemplateInstance> TemplateInstances { get; set; } = new List<TemplateInstance>();

        public List<CloneInstance> CloneInstances { get; set; } = new List<CloneInstance>();

    }
}
