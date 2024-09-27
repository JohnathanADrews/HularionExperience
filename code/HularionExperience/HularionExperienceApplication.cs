#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HularionCore.Pattern.Functional;
using HularionExperience.PackageBuilder;
using HularionExperience.Plugin.HX;
using HularionExperience.Plugin.Packages;
using HularionExperience.Plugin.Styles;
using HularionExperience.Registration;
using HularionPlugin.Route;
using HularionText.Language.Json;
using HularionText.StringCase;
using HularionPlugin.RouteRegistrar;
using HularionExperience.Boot;
using HularionExperience.PackageStores.Embedded;
using HularionExperience.PackageStores.DirectoryStore;
using HularionExperience.PackageStores.ProjectDirectoryStore;
using HularionExperience.PackageStores;
using HularionExperience.PackageStores.Composite;

namespace HularionExperience
{
    /// <summary>
    /// The resources for a Hularion Experience application.
    /// </summary>
    public class HularionExperienceApplication
    {
        /// <summary>
        /// The HX index.html
        /// </summary>
        public string IndexPage { get; set; }

        /// <summary>
        /// The details for the client framework to start the application 
        /// </summary>
        public ApplicationStartup ApplicationStartup { get; set; }

        public HularionRouter Router { get; private set; }
        public HXRouter HXRouter { get; private set; }
        public PackageRouter PackageRouter { get; private set; }
        public StyleCategoryRouter StyleCategoryRouter { get; private set; }

        /// <summary>
        /// iff true, HX will attempt to load the kernel from project files rather than embedded resources.
        /// </summary>
        public bool EnableKernelProjectFileStore { get; set; } = false;

        public IBootResources BootResources { get; }
        public IHXScreenController ScreenController { get; }

        private KernelInitializer kernelInitializer;
        private JsonSerializer jsonSerializer = new JsonSerializer(new StringCaseModifier(StringCaseDefinition.StartLower));
        public PackageLoader PackageManager;
        private IKernelResources kernelResources;

        private Action<string> sendAction = (message)=> { throw new NotImplementedException(String.Format("The send action has not been set. ybJeKPM0XkGb2QvcRZLU0g")); };


        public HularionExperienceApplication()
        {
            this.BootResources = new BootResources("Boot.Resources");
            this.ScreenController = new HXScreenController();

            Start();
        }

        public HularionExperienceApplication(IBootResources bootResources, IHXScreenController screenController)
        {
            this.ScreenController = screenController;
            this.BootResources = bootResources;

            Start();
        }

        private void Start()
        {
            this.kernelResources = new KernelResources();
            Router = new HularionRouter("hularion");

            PackageManager = new PackageLoader(Router);
            PackageRouter = new PackageRouter(PackageManager);
            Router.RegisterRouteProvider(PackageRouter);

            var registrationRouter = new RegistrationRouter(Router, ParameterizedCreator.FromSingle<AnonymousRouteInfo, HularionRoute>(anonymousInfo =>
            {
                return new HularionRoute<object, object>()
                {
                    Route = anonymousInfo.Route,
                    Handler = ParameterizedFacade.FromSingle<RoutedRequest<object>, RoutedResponse<object>>(hRequest =>
                    {
                        sendAction(jsonSerializer.Serialize(hRequest));
                        var hResponse = hRequest.CreateResponse<object>();
                        return hResponse;
                    })
                };
            }));
            Router.RegisterRouteProvider(registrationRouter);


            StyleCategoryRouter = new StyleCategoryRouter();
            Router.RegisterRouteProvider(StyleCategoryRouter);

            HXRouter = new HXRouter(BootResources, ScreenController);
            Router.RegisterRouteProvider(HXRouter);
        }

        public void Load()
        {
            var kernelFiles = kernelResources.GetKernelPackage();
            var packageBuilder = new HularionPackageBuilder();

            var packages = new List<PackageMeta>();

            CompositePackageStore systemPackageStore = new();


#if DEBUG
            if (EnableKernelProjectFileStore)
            {
                var projectProvider = new DevProjectFileProvider();
                List<string> projectDirectories = new();
                projectDirectories.AddRange(projectProvider.LocateHXFromCSharpProject(Assembly.GetExecutingAssembly()));
                var projectLocators = projectDirectories.Distinct().Select(x => new ProjectLocator()
                {
                    Directory = x,
                    SearchMethod = ProjectDirectorySearchMethod.ImmediateDirectory
                }).ToArray();
                var fileProjectStore = new ProjectFilePackageStore();
                fileProjectStore.AddLocators(projectLocators);
                systemPackageStore.AddPackageStore(fileProjectStore);
            }
            var embeddedStore = new EmbeddedPackageStore();
            embeddedStore.AddAssembly(Assembly.GetExecutingAssembly());
            embeddedStore.Initialize();
            systemPackageStore.AddPackageStore(embeddedStore);
#else
            var embeddedStore = new EmbeddedPackageStore();
            embeddedStore.AddAssembly(Assembly.GetExecutingAssembly());
            embeddedStore.Initialize();
            systemPackageStore.AddPackageStore(embeddedStore);
#endif


            var systemPackage = systemPackageStore.GetPackage(new PackageIndicator() { PackageKey = "HularionExperienceKernel", Version = "system" });
            this.kernelInitializer = new KernelInitializer()
            {
                iFrameKernelLoader = kernelResources.GetIFrameKernelLoader(),
                iFrameLoader = kernelResources.GetIFrameLoader(),
                kernelLoader = kernelResources.GetKernelLoader(),
                kernelPackageName = "Hularion Experience",
                kernelPackage = systemPackage.Package.Partial.ClientPackage,
                ApplicationStartup = ApplicationStartup
            };

        }

        /// <summary>
        /// Reloads the application, re-compiling all packages.
        /// </summary>
        public void Refresh()
        {
            Load();
        }


        public object GetHularionConstructorInitializer()
        {
            var result = jsonSerializer.Serialize(kernelInitializer, JsonSerializationSpacing.Expanded);
            return result;
        }

        /// <summary>
        /// Processes requests from the web UI.
        /// </summary>
        /// <param name="request">The requst(s) to process.</param>
        /// <returns>An object containing the response information.</returns>
        public object RequestProcessor(string request)
        {
            var result = Router.ProcessJsonRequestToJsonResult(request);
            return result;
        }

        public void RegisterRouteSender(Action<string> sendAction)
        {
            this.sendAction = sendAction;
        }

    }
}
