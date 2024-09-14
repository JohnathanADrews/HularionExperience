#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Topology;
using HularionExperience.Definition.Project.Concept;
using HularionExperience.PackageBuilder;
using HularionExperience.PackageBuilder.ProjectResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HularionExperience.Definition.PackageClient
{
    public class ClientPackage
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Stage { get; set; }

        public string CreationState { get; set; }

        public ClientApplication DefaultApplication { get; set; }

        public ClientDomeWrapper DomeWrapper { get; set; }

        public List<ClientFrameReference> ScriptFrameReferences { get; set; }

        public List<ClientPackageImport> PackageImports { get; set; } = new List<ClientPackageImport>();

        public Dictionary<string, ClientScriptSet> ImportScriptSets { get; set; } = new Dictionary<string, ClientScriptSet>();

        public Dictionary<string, ClientPackageImport> PackageImportsByAlias { get; set; } = new Dictionary<string, ClientPackageImport>();

        public Dictionary<string, ClientScriptSet> ScriptSets { get; set; } = new Dictionary<string, ClientScriptSet>();

        public Dictionary<string, ClientStyleSet> StyleSets { get; set; } = new Dictionary<string, ClientStyleSet>();

        public Dictionary<string, ClientPresenterSet> PresenterSets { get; set; } = new Dictionary<string, ClientPresenterSet>();

        public Dictionary<string, ClientScriptSet> ScriptImports { get; set; } = new Dictionary<string, ClientScriptSet>();

        public Dictionary<string, ClientApplication> Applications { get; set; } = new Dictionary<string, ClientApplication>();

        public List<ClientGraphic> Graphics { get; set; } = new List<ClientGraphic>();

        public List<ClientGraphicReference> GraphicReferences { get; set; } = new List<ClientGraphicReference>();

        public ClientRouteInformation RouteInformation { get; set; } = new ClientRouteInformation();

        public ClientPackage()
        {
            //stage = HXPackageStage.Package.ToString();
        }

        public ClientPackage(HularionProject project)
        {
            //TODO - Change this to a runtime key 
            Key = project.Key;
            Name = project.Name;
            Version = project.Version;
            //stage = HXPackageStage.Project.ToString();
            //creationState = package.CreationState.ToString();
            PackageImportsByAlias = project.PackageImports.ToDictionary(x=>x.Alias, x=>new ClientPackageImport(x));
            ScriptSets = project.ScriptSets.ToDictionary(x=>x.Name, x => new ClientScriptSet(x, project));
            StyleSets = project.StyleSets.ToDictionary(x => x.Name, x => new ClientStyleSet(x));
            PresenterSets = project.PresenterSets.ToDictionary(x => x.Name, x => new ClientPresenterSet(x, project));
            //scriptImports = project.ScriptImports.ToDictionary(x => x.Alias, x => new ClientScriptImport(x));
            ImportScriptSets = project.ImportScriptSets.ToDictionary(x => x.Name, x => new ClientScriptSet(x, project));
            Applications = project.Applications.ToDictionary(x => x.Key, x => new ClientApplication(project, x));
            Graphics = project.Graphics.Select(x => new ClientGraphic(x.Value)).ToList();
            GraphicReferences = project.GraphicReferences.Select(x => new ClientGraphicReference(x)).ToList();
            DomeWrapper = new ClientDomeWrapper(project.DomeWrapper);
            ScriptFrameReferences = project.ScriptFrameReferences.Select(x => new ClientFrameReference(x)).ToList();
            RouteInformation.RouteProviders = project.RouteProviderDescriptions.Select(x => new ClientRouteProvider(x)).ToList();
            //ProcessComponents(project, this);
        }


        public void ProcessComponents(HularionProject hularionProject, ClientPackage package)
        {
            //var projects = locator.GetAllItems();
            var projects = new HularionProject[] { hularionProject };

            //Needs refactor.
            var transform = new ResourcesToProjectTransform();

            var aliases = new Dictionary<string, Dictionary<string, PackageImport>>();
            var handlers = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, PresenterComponentHandler>>>>();
            var traverser = new TreeTraverser<ClientComponent>();
            var componentPresenters = new Dictionary<ClientComponent, string>();
            foreach (var project in projects)
            {
                aliases.Add(project.Key, new Dictionary<string, PackageImport>());
                aliases[project.Key].Add("this", new PackageImport() { Alias = "this", PackageKey = project.Key });
                foreach (var import in project.PackageImports)
                {
                    aliases[project.Key].Add(import.Alias, import);
                }

                //Setup the handlers.
                handlers.Add(project.Key, new Dictionary<string, Dictionary<string, Dictionary<string, PresenterComponentHandler>>>());
                foreach (var set in project.PresenterSets)
                {
                    handlers[project.Key].Add(set.Name, new Dictionary<string, Dictionary<string, PresenterComponentHandler>>());
                    foreach (var presenter in set.Presenters)
                    {
                        handlers[project.Key][set.Name].Add(presenter.Name, new Dictionary<string, PresenterComponentHandler>());
                        foreach (var handler in presenter.ComponentHandlers)
                        {
                            handlers[project.Key][set.Name][presenter.Name].Add(handler.Handler, handler);
                        }
                    }
                }

                //Set the components
                var components = new List<ClientComponent>();
                foreach (var set in package.PresenterSets)
                {
                    var packageReferences = new Dictionary<string, string>();

                    foreach (var presenter in set.Value.Presenters)
                    {
                        components.AddRange(presenter.Value.Components);
                        foreach (var c in presenter.Value.Components) { componentPresenters.Add(c, presenter.Value.Name); }
                        foreach (var template in presenter.Value.Templates.Values)
                        {
                            components.AddRange(template.Components);
                            foreach (var c in template.Components) { componentPresenters.Add(c, presenter.Value.Name); }
                            foreach (var p in template.Presenters)
                            {
                                components.AddRange(p.Components);
                                foreach (var c in p.Components) { componentPresenters.Add(c, p.PresenterName); }
                            }
                        }
                        foreach (var presenterReference in presenter.Value.Presenters)
                        {
                            components.AddRange(presenterReference.Components);
                            foreach (var c in presenterReference.Components) { componentPresenters.Add(c, presenterReference.PresenterName); }
                        }
                        foreach (var component in components)
                        {
                            var frame = set.Value.PresenterImportFrames.Where(x => x.Frame == component.Frame).FirstOrDefault();
                            if(frame == null) { component.PackageAlias = "this"; }
                            else { component.PackageAlias = frame.PackageAlias; }
                            //if (component.packageAlias == null) { component.packageAlias = "this"; }
                            component.SetName ??= set.Value.Name;
                            if (component.PresenterName == null) { component.SetName = presenter.Value.Name; }
                        }
                    }
                }
                //components contains only the top-level components.
                foreach (var component in components)
                {
                    //Components can have sub-components.
                    var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.LeftRightParent, component, node =>
                    {
                        //set the parent presenter name for each component
                        foreach (var c in node.Components) { componentPresenters.Add(c, node.PresenterName); }
                        return node.Components.ToArray();
                    }, true);
                    foreach (var c in plan)
                    {
                        if (c.Handler == null) { continue; }
                        if (!aliases.ContainsKey(project.Key)) { continue; }
                        if (!aliases[project.Key].ContainsKey(c.PackageAlias)) { continue; }
                        var packageKey = aliases[project.Key][c.PackageAlias].PackageKey;
                        if (!handlers.ContainsKey(packageKey)) { continue; }
                        if (!handlers[packageKey].ContainsKey(c.SetName)) { continue; }
                        if (!componentPresenters.ContainsKey(c)) { continue; }
                        var componentPresenter = componentPresenters[c];
                        if (!handlers[packageKey][c.SetName].ContainsKey(componentPresenter)) { continue; }
                        if (!handlers[packageKey][c.SetName][componentPresenter].ContainsKey(c.Handler)) { continue; }
                        var handler = handlers[packageKey][c.SetName][componentPresenter][c.Handler];
                        //The ClientComponent parameter should be initialized to the source string value.
                        c.Parameter = transform.DecodeParameter(String.Format("{0}", c.Parameter), handler.Encoding, handler.EncodingDelimiter);
                    }
                }
            }
        }


        //public ClientPackageMeta CreatePackageMeta()
        //{
        //    var meta = new ClientPackageMeta();
        //    meta.key = key;
        //    meta.name = name;
        //    meta.version = version;
        //    meta.stage = stage;

        //    meta.applications = applications.Select(x => x.Value.CreateApplicationMeta()).ToList();
        //    if(meta.defaultApplication != null)
        //    {
        //        meta.defaultApplication = defaultApplication.CreateApplicationMeta();
        //    }
        //    meta.scriptSets = scriptSets.Select(x => x.Value.CreateScriptSetMeta()).ToList();
        //    meta.presenterSets = presenterSets.Select(x => x.Value.CreatePresenterSetMeta()).ToList();
        //    meta.packageReferences = packageImports.Select(x => x.CreatePackageReferenceMeta()).ToList();


        //    return meta;
        //}
    }
}
