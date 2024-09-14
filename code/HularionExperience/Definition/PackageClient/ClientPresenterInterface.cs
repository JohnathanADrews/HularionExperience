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
