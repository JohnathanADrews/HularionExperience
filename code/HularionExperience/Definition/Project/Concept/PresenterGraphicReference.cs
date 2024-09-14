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
    public class PresenterGraphicReference
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        [PackageConceptKey]
        public string Key { get; set; }

        public string Index { get; set; }

        public string Alias { get; set; }

        public string Destination { get; set; } = "src";

        public bool IsThis { get; set; } = false;

        public bool IsHandle { get; set; } = false;

        public string Handle { get; set; }

        public PresenterGraphicReference(string index, string alias, string handle)
        {
            Index = index;
            Alias = String.Format("{0}", alias);
            Handle = handle;
            if (Alias.Contains("=>"))
            {
                var splits = Alias.Split("=>");
                Alias = splits[0];
                if (splits[1].Contains("/"))
                {
                    var innerSplits = splits[1].Split("/");
                    if (innerSplits[0].ToLower() == "handle")
                    {
                        IsHandle = true;
                        Handle = splits[1].Substring(innerSplits[0].Length + 1);
                    }
                }
                else if (splits[1].Contains("."))
                {
                    var innerSplits = splits[1].Split(".");
                    Destination = innerSplits[1];
                    if (innerSplits[0].Trim().ToLower() == "this")
                    {
                        IsThis = true;
                    }
                }
                else if (splits[1].Trim().ToLower() == "handle")
                {
                    IsHandle = true;
                    Handle = handle;
                }
            }
        }
    }
}
