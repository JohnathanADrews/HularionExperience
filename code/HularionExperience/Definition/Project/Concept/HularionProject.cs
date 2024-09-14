#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionExperience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HularionExperience.Definition.Project.Concept
{
    [PackageConcept]
    public class HularionProject : IItemLocator
    {
        /// <summary>
        /// The key of this object.
        /// </summary>
        [PackageConceptKey]
        public string Key { get; set; }

        public string Version { get; set; }

        public string ProductKey { get; set; }

        public string Brand { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }

        public string BuildKey { get; set; }

        public string Description { get; set; }

        public string License { get; set; }

        public bool LicenseAgreementIsRequired { get; set; } = false;

        public List<ProjectLink> Links { get; set; } = new List<ProjectLink>();

        public List<ProjectContributor> Collaborators { get; set; } = new List<ProjectContributor>();

        public List<string> Tags { get; set; } = new List<string>();


        public List<PackageImport> PackageImports = new();

        public List<Application> Applications { get; set; } = new List<Application>();

        public List<ScriptSet> ScriptSets { get; set; } = new List<ScriptSet>();

        public List<ScriptSet> ImportScriptSets { get; set; } = new List<ScriptSet>();

        public List<StyleSet> StyleSets { get; set; } = new List<StyleSet>();

        public List<StyleSet> ImportStyleSets { get; set; } = new List<StyleSet>();

        public List<PresenterSet> PresenterSets { get; set; } = new List<PresenterSet>();

        /// <summary>
        /// References to script frames for whole package use, such as DOME wrapper.
        /// </summary>
        public List<FrameReference> ScriptFrameReferences { get; set; } = new List<FrameReference>();

        /// <summary>
        /// These reference a package alias and a set within that package.  This is associated to a script import alias.
        /// </summary>
        public List<ScriptImport> ScriptImports { get; set; } = new List<ScriptImport>();

        public List<PresenterConfiguration> PresenterConfigurations { get; set; } = new List<PresenterConfiguration>();

        public List<StyleConfiguration> StyleConfigurations { get; set; } = new List<StyleConfiguration>();

        public List<ScriptConfiguration> ScriptConfigurations { get; set; } = new List<ScriptConfiguration>();

        //public PluginPackage Plugins { get; set; }

        //public WebPackage WebPackage { get; set; } = new WebPackage();

        public Dictionary<string, HashSet<string>> ThemeStyleKeys { get; set; } = new Dictionary<string, HashSet<string>>();

        public Dictionary<string, HashSet<string>> ThemeStructureKeys { get; set; } = new Dictionary<string, HashSet<string>>();

        public Dictionary<string, Graphic> Graphics { get; set; } = new Dictionary<string, Graphic>();

        public List<GraphicReference> GraphicReferences { get; set; } = new List<GraphicReference>();

        public List<ProjectServerFile> ServerFiles { get; set; } = new List<ProjectServerFile>();

        public List<RouteProviderFile> ServerRouteProviders = new();

        public List<RouteProviderDescription> RouteProviderDescriptions = new();

        public DomeWrapper DomeWrapper { get; set; }


        public IEnumerable<ItemLocationIndicator> GetItemLocationIndicator()
        {
            var type = typeof(HularionProject);
            var result = new List<ItemLocationIndicator>();
            result.Add(new ItemLocationIndicator() { Property = type.GetProperty("Key"), IsDefinitive = true, Priority = 256, Comparer = (x, y) => string.Format("{0}", x.Value) == ((HularionProject)y).Key });
            result.Add(new ItemLocationIndicator() { Property = type.GetProperty("BuildKey"), IsDefinitive = true, Priority = 128, Comparer = (x, y) => string.Format("{0}", x.Value) == ((HularionProject)y).BuildKey });
            result.Add(new ItemLocationIndicator() { Property = type.GetProperty("ProductKey"), Priority = 0, Comparer = (x, y) => string.Format("{0}", x.Value) == ((HularionProject)y).ProductKey });
            result.Add(new ItemLocationIndicator() { Property = type.GetProperty("Version"), Priority = 16, Comparer = (x, y) => string.Format("{0}", x.Value).ToLower() == string.Format("{0}", ((HularionProject)y).Version).ToLower() });
            result.Add(new ItemLocationIndicator() { Property = type.GetProperty("Name"), Priority = 0, Comparer = (x, y) => string.Format("{0}", x.Value).ToLower() == string.Format("{0}", ((HularionProject)y).Name).ToLower() });
            return result;
        }

        public IEnumerable<ItemLocationValue> GetLocationIndicatorValues()
        {
            var type = typeof(HularionProject);
            return new ItemLocationValue[] {
                new ItemLocationValue(){ Property = type.GetProperty("Key"), Value = Key},
                new ItemLocationValue(){ Property = type.GetProperty("BuildKey"), Value = BuildKey},
                new ItemLocationValue(){ Property = type.GetProperty("ProductKey"), Value = ProductKey},
                new ItemLocationValue(){ Property = type.GetProperty("Name"), Value = Name},
                new ItemLocationValue(){ Property = type.GetProperty("Version"), Value = Version}
            };
        }

        public PackageImport AddPackageImport()
        {
            var import = new PackageImport();
            PackageImports.Add(import);
            return import;
        }

        public Application AddApplication()
        {
            var application = new Application();
            Applications.Add(application);
            return application;
        }

        public ScriptSet AddScriptSet()
        {
            var set = new ScriptSet();
            ScriptSets.Add(set);
            return set;
        }

        public ScriptSet AddImportScriptSet()
        {
            var set = new ScriptSet();
            ImportScriptSets.Add(set);
            return set;
        }

        public StyleSet AddStyleSet()
        {
            var set = new StyleSet();
            StyleSets.Add(set);
            return set;
        }

        public StyleSet AddImportStyleSet()
        {
            var set = new StyleSet();
            ImportStyleSets.Add(set);
            return set;
        }

        public PresenterSet AddPresenterSet()
        {
            var set = new PresenterSet();
            PresenterSets.Add(set);
            return set;
        }

        public PresenterConfiguration AddPresenterConfiguration()
        {
            var configuration = new PresenterConfiguration();
            PresenterConfigurations.Add(configuration);
            return configuration;
        }

        public StyleConfiguration AddStyleConfiguration()
        {
            var configuration = new StyleConfiguration();
            StyleConfigurations.Add(configuration);
            return configuration;
        }

        public ScriptConfiguration AddScriptConfiguration()
        {
            var configuration = new ScriptConfiguration();
            ScriptConfigurations.Add(configuration);
            return configuration;
        }

        //public string GetStyleKey(string key)
        //{

        //}

        //public WebPackage CreateWebPackage()
        //{
        //    WebPackage = new WebPackage(this);
        //    return WebPackage;
        //}



    }


}
