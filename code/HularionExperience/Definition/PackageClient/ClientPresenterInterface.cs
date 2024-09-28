#region License
/*
MIT License

Copyright (c) 2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.PackageClient
{

    /// <summary>
    /// An interface that a presenter implements.
    /// </summary>
    public class ClientPresenterInterface
    {
        /// <summary>
        /// The name of the interface
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If set, the implementation of the interface passes through to a contained presenter .
        /// </summary>
        public ClientPresenterInterfacePassthrough Passthrough { get; set; }

        /// <summary>
        /// If there is a passthrough, these redirects handle the local implementation.
        /// </summary>
        public List<ClientPresenterInterfaceRedirect> Redirects { get; set; } = new();


        public ClientPresenterInterface()
        {
            
        }

        public ClientPresenterInterface(PresenterInterface presenterInterface)
        {
            if(presenterInterface == null)
            {
                return;
            }
            Name = presenterInterface.Name;
            Passthrough = new ClientPresenterInterfacePassthrough(presenterInterface.Passthrough);
            Redirects = presenterInterface.Redirects.Select(x=>new ClientPresenterInterfaceRedirect(x)).ToList();
        }

    }
}
