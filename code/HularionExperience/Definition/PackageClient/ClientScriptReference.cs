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
    public class ClientScriptReference
    {
        /// <summary>
        /// The script text that will retrieve the handle.
        /// </summary>
        public string Assign { get; set; }

        public string Attach { get; set; }

        /// <summary>
        /// The name of the object the presenter's will receive that will reference the Assign return value.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// The name of the frame in which the script was loaded.  The same script can be loaded into multiple frames.
        /// </summary>
        public string Frame { get; set; }

        public string Mode { get; set; }

        public string Order { get; set; }



        public ClientScriptReference()
        {

        }

        public ClientScriptReference(ScriptFrameHandle frameHandle)
        {
            Assign = frameHandle.Assign;
            Handle = frameHandle.Handle;
            Frame = frameHandle.Frame;
            Mode = frameHandle.Mode.ToString();
            Order = frameHandle.Order.ToString();
            Attach = frameHandle.Attach.ToString();
        }

    }
}
