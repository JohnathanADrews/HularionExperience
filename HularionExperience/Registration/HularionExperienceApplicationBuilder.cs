#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience.Boot;
using HularionExperience.PackageBuilder;
using HularionExperience.PackageStores;
using HularionPlugin.Route;
using HularionText.Language.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.Registration
{
    public class HularionExperienceApplicationBuilder
    {

        private UserMode selectedUserMode;

        public IHXScreenController ScreenController { get; set; }

        public IBootResources BootResources { get; set; }

        public IKernelResources KernelResources { get; set; }

        public string BaseDirectory { get; set; }

        public ApplicationStartup ApplicationStartup { get; set; }

        public List<IPackageStore> PackageStores { get; set; } = new ();

        public List<IRouteProvider> RouteProviders { get; set; } = new();

        public HularionExperienceApplicationBuilder()
        {
        }


        public HularionExperienceApplicationBuilder ForDesktop()
        {
            selectedUserMode = UserMode.Desktop;
            return this;
        }

        public HularionExperienceApplicationBuilder ForWeb()
        {
            selectedUserMode = UserMode.Web;
            return this;
        }

        public HularionExperienceApplicationBuilder SetBaseDirectory(string baseDirectory)
        {
            this.BaseDirectory = baseDirectory;
            return this;
        }

        public HularionExperienceApplicationBuilder SetBootResources(IBootResources bootResources)
        {
            this.BootResources = bootResources;
            return this;
        }

        public HularionExperienceApplicationBuilder SetKernelResources(IKernelResources kernelResources)
        {
            this.KernelResources = kernelResources;
            return this;
        }

        public HularionExperienceApplicationBuilder AddPackageSource(IPackageStore store)
        {
            PackageStores.Add(store);
            return this;
        }
        

        public HularionExperienceApplicationBuilder SetApplicationStartup(ApplicationStartup applicationStartup)
        {
            ApplicationStartup = applicationStartup;
            return this;
        }

        public HularionExperienceApplication Build()
        {
            //if (stateStore == null)
            //{
            //    throw new ApplicationException("The state store must be set using SetStateStore. Cl2gxx8OcU2VUhSdfcpPsA.");
            //}
            HularionExperienceApplication application;
            switch (selectedUserMode)
            {
                case UserMode.Desktop:
                    if(BootResources == null)
                    {
                        //BootResources = new DesktopBootResources();
                    }
                    application = new HularionExperienceApplication(BootResources, ScreenController, BaseDirectory);
                    application.Url = "http://hularion";
                    application.ResourceHandlers.Add(new BrowserResourceHandler()
                    {
                        Name = "Resource Configuration Script",
                        Url = "http://hularion/HularionKernelBootLoader.js",
                        Stream = new MemoryStream(Encoding.ASCII.GetBytes(BootResources.GetHularionKernelBootLoaderScript())),
                        MimeType = "text/javascript"
                    });
                    application.RegistrationObjects.Add(new BrowserRegistrationObject()
                    {
                        Name = "resourceManagerAsync",
                        Value = application
                    });
                    application.IndexPage = BootResources.GetIndexPage();
                    foreach(var store in PackageStores)
                    {
                        application.PackageManager.AddPackageStore(store);
                    }
                    application.ApplicationStartup = ApplicationStartup;
                    application.Load();
                    return application;
                //case UserMode.Web:
                //    var webBootResources = new WebBootResources();
                //    application = new HularionExperienceApplication(webBootResources, stateStore, ScreenController, BaseDirectory);
                //    return application;
            }
            throw new Exception("Unhandled UserMode Type. This exception should be unreachable LLaiFLZSS0SzSszQKlXiqA.");
        }



        public enum UserMode
        {
            Desktop,
            Web
        }
    }
}
