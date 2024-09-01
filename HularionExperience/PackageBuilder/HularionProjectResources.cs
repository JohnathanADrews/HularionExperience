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

namespace HularionExperience.PackageBuilder
{
    public class HularionProjectResources
    {
        public string Key { get; set; }

        public string Location { get; set; }

        public ResourceFile ProjectFile { get; set; }

        public List<ResourceFile> Applications { get; set; } = new List<ResourceFile>();

        public List<ResourceFile> Configurations { get; set; } = new List<ResourceFile>();

        public List<ResourceSet> ScriptSets { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> StyleSets { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> PresenterSets { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> ImportScriptSets { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> ImportStyleSets { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> Graphics { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> Fonts { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> Plugin { get; set; } = new List<ResourceSet>();

        public List<ResourceSet> Content { get; set; } = new List<ResourceSet>();

    }
}
