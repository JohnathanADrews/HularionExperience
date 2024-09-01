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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder
{
    /// <summary>
    /// Indicates the form of a package source.
    /// </summary>
    public enum PackageSourceForm
    {
        /// <summary>
        /// The source is embedded resource project files from within an assembly.
        /// </summary>
        Assemby,
        /// <summary>
        /// The source is from files in a directory.
        /// </summary>
        Files,
        /// <summary>
        /// The source is a set of project resources. i.e. HularionProjectResources
        /// </summary>
        Resources,
        /// <summary>
        /// The source is a project object. i.e. HularionProject.
        /// </summary>
        Project,
        /// <summary>
        /// The source is a compiled package. i.e. ClientPackage.
        /// </summary>
        Package
    }
}
