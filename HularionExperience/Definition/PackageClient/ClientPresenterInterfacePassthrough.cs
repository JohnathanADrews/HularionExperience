using HularionExperience.Definition.Project.Concept;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.PackageClient
{
    /// <summary>
    /// Represents a passthrough for an interface, allowing a presenter to pass the implementation onto another presenter instance.
    /// </summary>
    public class ClientPresenterInterfacePassthrough
    {
        /// <summary>
        /// The frame of the passthrough presenter.
        /// </summary>
        public string Frame { get; set; }

        /// <summary>
        /// The handle of the passthrough presenter.
        /// </summary>
        public string Handle { get; set; }

        public ClientPresenterInterfacePassthrough()
        {
            
        }

        public ClientPresenterInterfacePassthrough(PresenterInterfacePassthrough passthrough)
        {
            if(passthrough == null)
            {
                return;
            }
            Frame = passthrough.Frame;
            Handle = passthrough.Handle;
        }

    }
}
