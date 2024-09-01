#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.PackageBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace HularionExperience.Definition.Project.Concept
{
    [PackageConcept]
    public class Graphic
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        [PackageConceptKey]
        public string Key { get; set; }

        public string Set { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public GraphicContentType ContentType { get; set; }

        public Graphic()
        {

        }

        public Graphic(ResourceSet set, ResourceFile file)
        {
            Set = set.Name;
            Name = file.Filename;
            if (file.Type == PackageFileType.Byte)
            {
                Content = string.Format("data:image/{0};base64,{1}", file.FileExtension, Convert.ToBase64String(file.Blob));
                ContentType = GraphicContentType.Binary;
            }
            else
            {
                Content = file.Content;
                ContentType = GraphicContentType.Text;
            }
        }

        public enum GraphicContentType
        {
            Text,
            Binary
        }

    }
}
