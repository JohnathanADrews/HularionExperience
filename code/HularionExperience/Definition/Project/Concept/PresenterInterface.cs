using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Definition.Project.Concept
{
    /// <summary>
    /// An interface that a presenter implements.
    /// </summary>
    public class PresenterInterface
    {
        /// <summary>
        /// The name of the interface
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If set, the implementation of the interface passes through to a contained presenter .
        /// </summary>
        public PresenterInterfacePassthrough Passthrough { get; set; }

        /// <summary>
        /// If there is a passthrough, these redirects handle the local implementation.
        /// </summary>
        public List<PresenterInterfaceRedirect> Redirects { get; set; } = new();

    }
}
