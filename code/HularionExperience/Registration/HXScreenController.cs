using HularionCore.Pattern.Functional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Registration
{
    /// <summary>
    /// A default HX screen controller.
    /// </summary>
    public class HXScreenController : IHXScreenController
    {
        /// <summary>
        /// Sets the Reload handler. Using StandardHandler allows it to be set to a func.
        /// </summary>
        public IHandler ReloadHandler { get; set; } = new StandardHandler() { Handler = () => { } };

        /// <summary>
        /// Sets the Show Dev Tools handler. Using StandardHandler allows it to be set to a func.
        /// </summary>
        public IHandler ShowDevToolsHandler { get; set; } = new StandardHandler() { Handler = () => { } };

        public void ReloadPlayer()
        {
            ReloadHandler.Handler();
        }

        public void ShowDevTools()
        {
            ShowDevToolsHandler.Handler();
        }
    }
}
