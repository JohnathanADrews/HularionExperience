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
    public class ClientComponentHandler
    {
        /// <summary>
        /// The name of the handler.  This allows a source to send a view to a particular handler.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The method to call after Start to pass Parameter to.  If not set, parameter is passed to Start.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Determines whether an array parameter will be accepted as an array ("single") or whether the array will be split over a parameter set ("params").  Does not apply to other object types.
        /// </summary>
        public string MethodSplit { get; set; }

        public ClientComponentHandler()
        {

        }

        public ClientComponentHandler(PresenterComponentHandler handler)
        {
            Name = handler.Handler;
            Method = handler.Method;
            MethodSplit = handler.EncodingType;
        }
    }
}
