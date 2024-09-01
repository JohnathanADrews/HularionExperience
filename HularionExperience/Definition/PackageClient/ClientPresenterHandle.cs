﻿#region License
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
    /// <summary>
    /// A handle to an html element.
    /// </summary>
    public class ClientPresenterHandle
    {

        /// <summary>
        /// The index of the node this refers to.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// The name of the handle that will be assigned ot the presenter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If set, this indicates a wrapper type for the html element.
        /// </summary>
        public string Wrapper { get; set; }

        public ClientPresenterHandle()
        {

        }
        public ClientPresenterHandle(PresenterHandle handle)
        {
            Index = handle.Index;
            Name = handle.Name;
            Wrapper = handle.Wrapper;
        }

    }
}
