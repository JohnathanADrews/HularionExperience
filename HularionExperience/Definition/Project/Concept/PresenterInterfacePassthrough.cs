using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.Project.Concept
{
    /// <summary>
    /// Represents a passthrough for an interface, allowing a presenter to pass the implementation onto another presenter instance.
    /// </summary>
    public class PresenterInterfacePassthrough
    {
        /// <summary>
        /// The frame of the passthrough presenter.
        /// </summary>
        public string Frame { get; set; }

        /// <summary>
        /// The handle of the passthrough presenter.
        /// </summary>
        public string Handle {  get; set; }

    }
}
