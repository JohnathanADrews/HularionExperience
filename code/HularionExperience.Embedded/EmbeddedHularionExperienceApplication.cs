#region License
/*
MIT License

Copyright (c) 2024 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

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
