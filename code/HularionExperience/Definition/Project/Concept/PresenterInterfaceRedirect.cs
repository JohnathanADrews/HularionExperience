using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.Project.Concept
{
    /// <summary>
    /// A redirect from an interface passthrough to a presenter method
    /// </summary>
    public class PresenterInterfaceRedirect
    {
        /// <summary>
        /// The method to redirect.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// The actual method handling the method call.
        /// </summary>
        public string Receiver { get; set; }

    }
}
