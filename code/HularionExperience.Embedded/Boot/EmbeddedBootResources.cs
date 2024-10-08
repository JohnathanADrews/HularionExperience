﻿#region License
/*
MIT License

Copyright (c) 2023-2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion


using HularionExperience.Boot;
using HularionExperience.Registration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Embedded
{
    /// <summary>
    /// The boot resources for an embedded application.
    /// </summary>
    public class EmbeddedBootResources : BootResources
    {

        /// <summary>
        /// This will replace the title of the application in Index.html.
        /// </summary>
        public string ApplicationTitle { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmbeddedBootResources() 
            : base("Embedded.Boot", Assembly.GetExecutingAssembly())
        {

        }

        /// <summary>
        /// Gets the index page for an embedded application.
        /// </summary>
        /// <returns>The index page for an embedded application.</returns>
        public override string GetIndexPage()
        {
            var index = GetResource("Index.html");
            index = index.Replace("$$IndexTitle$$", ApplicationTitle);
            return index;
        }
    }
}
