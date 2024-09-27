using HularionExperience.Boot;
using HularionExperience.Registration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Embedded
{
    /// <summary>
    /// An application for embedded HX
    /// </summary>
    public class EmbeddedHularionExperienceApplication
    {

        public HularionExperienceApplication HXApplication { get; private set; }

        public HXScreenController ScreenController { get; private set; }


        public string Url { get; set; }

        public IList<BrowserRegistrationObject> RegistrationObjects { get; private set; } = new List<BrowserRegistrationObject>();

        public IList<BrowserResourceHandler> ResourceHandlers { get; private set; } = new List<BrowserResourceHandler>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="applicationStartup">The HX application to load on startup.</param>
        public EmbeddedHularionExperienceApplication(ApplicationStartup applicationStartup)
        {
            var bootResources = new EmbeddedBootResources();
            ScreenController = new HXScreenController();
            HXApplication = new HularionExperienceApplication(bootResources, ScreenController);
            HXApplication.ApplicationStartup = applicationStartup;

            Url = "http://hularion";
            ResourceHandlers.Add(new BrowserResourceHandler()
            {
                Name = "Resource Configuration Script",
                Url = "http://hularion/HularionKernelBootLoader.js",
                Stream = new MemoryStream(Encoding.ASCII.GetBytes(HXApplication.BootResources.GetHularionKernelBootLoaderScript())),
                MimeType = "text/javascript"
            });
            RegistrationObjects.Add(new BrowserRegistrationObject()
            {
                Name = "resourceManagerAsync",
                Value = HXApplication
            });
            HXApplication.IndexPage = HXApplication.BootResources.GetIndexPage();
            HXApplication.EnableKernelProjectFileStore = true;
            HXApplication.Load();
        }

    }
}
