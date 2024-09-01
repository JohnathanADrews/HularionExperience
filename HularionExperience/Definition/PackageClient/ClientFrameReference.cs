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
    public class ClientFrameReference
    {
        public string PackageAlias { get; set; }

        public string SetName { get; set; }

        public string Frame { get; set; }

        public string Type { get; set; }

        public string ProvideFrame { get; set; }

        public string Identifier { get; set; }

        public ClientFrameReference()
        {

        }

        public ClientFrameReference(FrameReference reference)
        {
            PackageAlias = reference.PackageAlias;
            SetName = reference.SetName;
            Frame = reference.Frame;
            Type = String.Format("{0}", reference.Type);
            ProvideFrame = reference.ProvideFrame;
            Identifier = reference.Identifier;

            if (String.IsNullOrWhiteSpace(PackageAlias)) { PackageAlias = "this"; }
            if (String.IsNullOrWhiteSpace(SetName)) { SetName = "this"; }
        }

    }
}
