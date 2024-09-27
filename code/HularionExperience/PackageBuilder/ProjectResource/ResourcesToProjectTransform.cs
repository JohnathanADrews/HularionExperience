#region License
/*
MIT License

Copyright (c) 2023 Johnathan A Drews

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using HularionCore.Pattern.Functional;
using HularionCore.Pattern.Topology;
using HularionExperience.Definition.Package;
using HularionExperience.Definition.Project.Concept;
using HularionExperience.PackageBuilder.ProjectResource.Transforms;
using HularionPlugin.Route;
using HularionText.Language.Css;
using HularionText.Language.Html;
using HularionText.Language.Json;
using HularionText.StringCase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace HularionExperience.PackageBuilder.ProjectResource
{
    /// <summary>
    /// Transforms project resource objects into a project object.
    /// </summary>
    public class ResourcesToProjectTransform
    {
        /// <summary>
        /// Transforms project resource objects into a project object.
        /// </summary>
        public ITransform<HularionProjectResources, HularionProject> FileTransform;

        private CssLanguage cssLanguage = new CssLanguage();
        private JsonLanguage jsonLanguage = new JsonLanguage();

        private const string encodingDelimiter = "delimiter";
        private const string encodingPDelimiter = "pdelimiter";
        private const string encodingJson = "json";
        private const string encodingTypeSingle = "single";
        private const string encodingTypeParams = "params";
        private const string classIndicator = "class";
        private const string styleIndicator = "style";

        private TreeTraverser<HtmlNode> traverser = new TreeTraverser<HtmlNode>();


        private int styleKeyIndex = 0;

        private int componentTemplateId = 0;

        private PresenterTransform presenterTransform;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourcesToProjectTransform()
        {
            FileTransform = new TransformFunction<HularionProjectResources, HularionProject>(file => Transform(file));

            presenterTransform = new PresenterTransform(() => CreateStyleKey(), () => (componentTemplateId++));
        }

        /// <summary>
        /// Transforms project resources object into a project object.
        /// </summary>
        /// <param name="projectResources">The resources of the project.</param>
        /// <returns>A project object.</returns>
        public HularionProject Transform(HularionProjectResources projectResources)
        {
            var project = new HularionProject();
            var html = new HtmlLanguage();
            //For now, assume all files are html since that is the case at present.

            ProcessPackageManifest(html.Parse(projectResources.ProjectFile.Content), project, projectResources.Location);
            foreach (var resource in projectResources.Applications) { ProcessApplicationDocument(html.Parse(resource.Content), project.AddApplication()); }
            foreach (var resource in projectResources.Configurations) { ProccessConfigurationDocument(html.Parse(resource.Content), project); }
            ProccessPresenterSets(html, projectResources, project);

            foreach (var set in projectResources.ScriptSets)
            {
                var ps = project.AddScriptSet();
                ps.Key = set.Key;
                ps.Name = set.Name;
                ps.Scripts = set.Resources;
            }
            foreach (var set in projectResources.ImportScriptSets)
            {
                var ps = project.AddImportScriptSet();
                ps.Key = set.Key;
                ps.Name = set.Name;
                ps.Scripts = set.Resources;
            }
            //foreach (var set in file.StyleSets)
            //{
            //    var ps = package.AddStyleSet();
            //    ps.Key = set.Key;
            //    ps.Name = set.Name;
            //    ps.Styles = set.Resources;
            //}
            //foreach (var set in file.ImportStyleSets)
            //{
            //    var ps = package.AddImportStyleSet();
            //    ps.Key = set.Key;
            //    ps.Name = set.Name;
            //    ps.Styles = set.Resources;
            //}
            //package.CreateWebPackage();

            //ProcessComponents(project);

            foreach (var graphic in projectResources.Graphics)
            {
                foreach (var resource in graphic.Resources)
                {
                    var projectGraphic = new Graphic();
                    projectGraphic.ContentType = Graphic.GraphicContentType.Text;
                    projectGraphic.Key = resource.Key;
                    projectGraphic.Name = resource.Name;
                    projectGraphic.Content = resource.Content;
                    project.Graphics.Add(resource.Key, projectGraphic);
                }
            }

            ProcessPluginFiles(project, projectResources.Plugin);

            return project;
        }

        public IEnumerable<HularionProject> Transform(IEnumerable<HularionProjectResources> packageFiles)
        {
            var packages = new List<HularionProject>();
            foreach (var file in packageFiles) { packages.Add(Transform(file)); }
            return packages;
        }

        private void ProcessPluginFiles(HularionProject project, IEnumerable<ResourceSet> plugin)
        {

            var routeDescriptions = new Dictionary<string, RouteProviderDescription>();
            foreach (var routeProvider in project.RouteProviderDescriptions)
            {
                routeDescriptions.Add(routeProvider.RouteProviderKey, routeProvider);
            }

            foreach (var set in plugin)
            {
                foreach (var resource in set.Resources)
                {
                    var serverFile = new ProjectServerFile(resource);
                    project.ServerFiles.Add(serverFile);
                    try
                    {
                        var assembly = Assembly.Load(resource.Blob);
                        if (assembly.FullName.StartsWith("HularionPlugin,"))
                        {
                            continue;
                        }
                        var routeType = typeof(IRouteProvider);
                        RouteProviderFile routeFile = null;
                        foreach (var type in assembly.GetTypes())
                        {
                            foreach (var ti in type.GetInterfaces())
                            {
                                if (ti.FullName == routeType.FullName)
                                {
                                    if (routeFile == null)
                                    {
                                        routeFile = new RouteProviderFile() { FilePath = serverFile.Filename };
                                        project.ServerRouteProviders.Add(routeFile);
                                    }
                                    routeFile.ProviderTypes.Add(type.FullName);

                                    if (routeType.IsAssignableFrom(type))
                                    {
                                        var instance = (IRouteProvider)Activator.CreateInstance(type);
                                        if (routeDescriptions.ContainsKey(instance.Key))
                                        {
                                            foreach (var route in instance.Routes)
                                            {
                                                if (!string.IsNullOrWhiteSpace(route.Method))
                                                {
                                                    var routeMethod = new RouteMethod()
                                                    {
                                                        Route = route.Route,
                                                        Name = route.Name,
                                                        Method = route.Method,
                                                        Description = route.Usage
                                                    };
                                                    routeDescriptions[instance.Key].Methods.Add(routeMethod);
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        //Probably not an assembly. Just looking for relevant interface implementations.
                    }
                }
            }
        }

        private void ProcessPackageManifest(HtmlDocument document, HularionProject package, string projectLocation)
        {
            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, document.Root, node => node.Nodes.ToArray(), true);
            var nodes = document.GetAttibuteNodes(HtmlPackageAttribute.HXPackage.Attribute);
            if (nodes.Count() > 0)
            {
                var node = nodes.First();
                package.Key = node.GetFirstAvailableAttributeValue(HtmlPackageAttribute.PackageKey.Attribute, HtmlPackageAttribute.ObjectKey.Attribute);
                package.ProductKey = node.GetAttributeValue(HtmlPackageAttribute.ProductKey.Attribute);
                package.Version = node.GetAttributeValue(HtmlPackageAttribute.Version.Attribute);
                package.Name = node.GetFirstAvailableAttributeValue(HtmlPackageAttribute.PackageName.Attribute, HtmlPackageAttribute.ObjectName.Attribute);
                package.Description = node.GetFirstAvailableAttributeValue(HtmlPackageAttribute.ObjectDescription.Attribute);
                package.Brand = node.GetAttributeValue(HtmlPackageAttribute.Brand.Attribute);
            }
            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.PackageImport.Attribute);
            foreach (var node in nodes)
            {
                var import = package.AddPackageImport();
                import.PackageKey = node.GetAttributeValue(HtmlPackageAttribute.PackageImport.Attribute, returnEmptyStringIfNull: true);
                if (import.PackageKey.Contains("=>"))
                {
                    var splits = import.PackageKey.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Length >= 2)
                    {
                        import.PackageKey = splits[0];
                        import.Alias = splits[1];
                    }
                }
                if (import.PackageKey.Contains("@"))
                {
                    var splits = import.PackageKey.Split("@", StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Length >= 2)
                    {
                        import.PackageKey = splits[0];
                        import.Version = splits[1];
                    }
                }
                import.Project = GetProjectImportLocation(projectLocation, node.GetAttributeValue(HtmlPackageAttribute.Project.Attribute));
                if (node.HasAttribute(HtmlPackageAttribute.Alias.Attribute)) { import.Alias = node.GetAttributeValue(HtmlPackageAttribute.Alias.Attribute); }
                if (node.HasAttribute(HtmlPackageAttribute.Version.Attribute)) { import.Version = node.GetAttributeValue(HtmlPackageAttribute.Version.Attribute); }
                import.RequestorKey = package.Key;
                import.RequestorName = package.Name;

                foreach (var n in node.Nodes)
                {
                    var attribute = n.GetAttributes(HtmlPackageAttribute.ScriptSet.Attribute).FirstOrDefault();
                    if (attribute == null) { continue; }
                    var scriptImport = new ScriptImport() { Alias = attribute.Value, PackageAlias = import.Alias };
                    attribute = n.GetAttributes(HtmlPackageAttribute.ScriptSet.Attribute).FirstOrDefault();
                    if (attribute == null) { continue; }
                    scriptImport.SetName = attribute.Value;
                    package.ScriptImports.Add(scriptImport);
                }
            }
            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.DomeWrapper.Attribute);
            if (nodes.Count() > 0)
            {
                package.DomeWrapper = new DomeWrapper()
                {
                    EvalCode = nodes.First().GetAttributeValue(HtmlPackageAttribute.DomeWrapper.Attribute),
                    EvalFrameKey = nodes.First().GetAttributeValue(HtmlPackageAttribute.ScriptFrame.Attribute)
                };
            }
            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.ImportSet.Attribute);
            foreach (var node in nodes)
            {
                var frame = new FrameReference()
                {
                    Type = FrameReferenceType.Script,
                    PackageAlias = node.GetAttributeValue(HtmlConfigurationAttribute.ImportScript.Attribute),
                    Frame = node.GetAttributeValue(HtmlConfigurationAttribute.Frame.Attribute),
                    SetName = node.GetAttributeValue(HtmlConfigurationAttribute.ImportSet.Attribute)
                };
                package.ScriptFrameReferences.Add(frame);
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.Graphic.Attribute).Where(x => x.HasAttribute(HtmlPackageAttribute.Alias.Attribute));
            foreach (var node in nodes)
            {
                var graphic = new GraphicReference(node.GetAttributeValue(HtmlPackageAttribute.Graphic.Attribute), node.GetAttributeValue(HtmlPackageAttribute.Alias.Attribute));
                package.GraphicReferences.Add(graphic);
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.Contributor.Attribute);
            var collaborators = new Dictionary<HtmlNode, ProjectContributor>();
            foreach (var node in nodes)
            {
                var collaborator = new ProjectContributor()
                {
                    Name = node.GetAttributeValue(HtmlPackageAttribute.Contributor.Attribute),
                    Role = node.GetAttributeValue(HtmlPackageAttribute.Role.Attribute)
                };
                package.Collaborators.Add(collaborator);
                collaborators.Add(node, collaborator);
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.Link.Attribute);
            foreach (var node in nodes)
            {
                var link = new ProjectLink()
                {
                    Url = node.GetAttributeValue(HtmlPackageAttribute.Link.Attribute),
                    Name = node.GetAttributeValue(HtmlPackageAttribute.ObjectName.Attribute),
                    Description = node.CoalesceAttributeValueOrNodeText(HtmlPackageAttribute.ObjectDescription.Attribute)
                };
                if (node.Parent != null && collaborators.ContainsKey(node.Parent))
                {
                    collaborators[node.Parent].Links.Add(link);
                }
                else
                {
                    package.Links.Add(link);
                }
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.License.Attribute);
            if (nodes.Count() > 0)
            {
                var node = nodes.First();
                package.License = node.CoalesceAttributeValueOrNodeText(new string[] { HtmlPackageAttribute.License.Attribute }, attributeFirst: false);
                if (node.HasAttribute(HtmlPackageAttribute.LicenseRequired.Attribute))
                {
                    bool required = false;
                    bool.TryParse(node.GetAttributeValue(HtmlPackageAttribute.LicenseRequired.Attribute), out required);
                    package.LicenseAgreementIsRequired = required;
                }
                else
                {
                    package.LicenseAgreementIsRequired = false;
                }
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.Tags.Attribute);
            foreach (var node in nodes)
            {
                package.Tags.AddRange(string.Format("{0}", node.CoalesceAttributeValueOrNodeText(HtmlPackageAttribute.Tags.Attribute)).Split(new char[] { ' ', '\t', '\r', '\n', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries));
            }

            nodes = document.GetAttibuteNodes(HtmlPackageAttribute.ServerRouter.Attribute);
            foreach (var node in nodes)
            {
                var routeProvider = new RouteProviderDescription()
                {
                    RouteProviderKey = node.GetAttributeValue(HtmlPackageAttribute.ServerRouter.Attribute),
                    MethodGroup = node.GetAttributeValue(HtmlPackageAttribute.ServerRouter.Attribute)
                };
                if (routeProvider.RouteProviderKey.Contains("=>"))
                {
                    var splits = routeProvider.RouteProviderKey.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                    if (splits.Length > 0)
                    {
                        routeProvider.RouteProviderKey = splits[0];
                    }
                    if (splits.Length > 1)
                    {
                        routeProvider.MethodGroup = splits[1];
                    }
                }
                routeProvider.Handle = node.GetAttributeValue(HtmlPackageAttribute.Handle.Attribute);
                routeProvider.HandleType = node.GetAttributeValue(HtmlPackageAttribute.Attach.Attribute);

                if(String.Format("{0}", node.GetAttributeValue(HtmlPackageAttribute.SystemRouter.Attribute)).ToLower() == "true")
                {
                    routeProvider.IsSystemRouter = true;
                }

                package.RouteProviderDescriptions.Add(routeProvider);
            }

        }

        private void ProcessApplicationDocument(HtmlDocument document, Application application)
        {

            var appNode = document.GetAttibuteNodes(HtmlApplicationAttribute.Application.Attribute).FirstOrDefault();
            if(appNode == null)
            {
                return;
            }

            application.Key = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.ApplicationKey.Attribute, HtmlApplicationAttribute.ObjectKey.Attribute });
            application.Name = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.ApplicationName.Attribute, HtmlApplicationAttribute.ObjectName.Attribute });
            application.Description = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.ObjectDescription.Attribute });
            application.PresenterSet = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.PresenterSet.Attribute });
            application.Presenter = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.ApplicationPresenter.Attribute });
            application.PresenterConfiguration = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.PresenterConfiguration.Attribute });

            application.IsDefault = false;
            var defaultValue = appNode.GetFirstAvailableAttributeValue(new string[] { HtmlApplicationAttribute.ApplicationIsDefault.Attribute });
            if (string.IsNullOrWhiteSpace(defaultValue)) { return; }
            bool isDefault = false;
            bool.TryParse(defaultValue, out isDefault);
            application.IsDefault = isDefault;

            if (application.PresenterSet.Contains("/"))
            {
                var splits = application.PresenterSet.Split('/');
                if (splits.Length >= 2)
                {
                    application.PresenterSet = splits[0];
                    application.Presenter = splits[1];
                }
            }

            var linkNodes = document.GetAttibuteNodes(HtmlApplicationAttribute.Link.Attribute).ToList();
            foreach (var linkNode in linkNodes)
            {
                var link = new ProjectLink()
                {
                    Url = linkNode.GetAttributeValue(HtmlPackageAttribute.Link.Attribute),
                    Name = linkNode.GetAttributeValue(HtmlPackageAttribute.ObjectName.Attribute),
                    Description = linkNode.CoalesceAttributeValueOrNodeText(HtmlPackageAttribute.ObjectDescription.Attribute)
                };
                application.Links.Add(link);
            }

            
            var traverser = new TreeTraverser<HtmlNode>();
            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, appNode, node => node.Nodes.ToArray(), true);

            foreach (var node in plan)
            {
                if (node.HasAttribute(HtmlApplicationAttribute.SelectedStyle.Attribute))
                {
                    var value = node.GetAttributeValue(HtmlApplicationAttribute.SelectedStyle.Attribute);
                    if (!value.Contains("=>"))
                    {
                        continue;
                    }
                    var parts = value.Split("=>", StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2)
                    {
                        continue;
                    }
                    application.StyleCategories.Add(new StyleCategorySelection()
                    {
                        CategoryName = parts[0],
                        SelectedStyle = parts[1]
                    });

                }
            }




        }

        private void ProccessConfigurationDocument(HtmlDocument document, HularionProject package)
        {
            var node = document.GetAttibuteNodes(HtmlConfigurationAttribute.PresenterConfiguration.Attribute).FirstOrDefault();
            if (node != null)
            {
                ProcessPresenterConfiguration(document, package);
            }
            node = document.GetAttibuteNodes(HtmlConfigurationAttribute.StyleConfiguration.Attribute).FirstOrDefault();
            if (node != null)
            {
                var configuration = package.AddStyleConfiguration();
                configuration.Name = node.Attributes.Where(x => x.Name == HtmlConfigurationAttribute.StyleConfiguration.Attribute).First().Value;
            }
            node = document.GetAttibuteNodes(HtmlConfigurationAttribute.ScriptConfiguration.Attribute).FirstOrDefault();
            if (node != null)
            {
                ProcessScriptConfiguration(document, package);
            }
        }

        private void ProcessPresenterConfiguration(HtmlDocument document, HularionProject package)
        {
            var configuration = package.AddPresenterConfiguration();
            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, document.Root, node => node.Nodes.ToArray(), true);

            var styleNodes = document.GetNodesOfElementType(HtmlElementType.Style);
            var styleText = new StringBuilder();
            foreach (var node in styleNodes)
            {
                styleText.Append(node.EmbeddedLanguageContent);
            }
            configuration.CssStyles = styleText.ToString();

            //configuration.Collaborators = document.GetAttibuteNodes(HtmlConfigurationAttribute.Collaborator.Attribute).Select(x => x.GetAttributeValue(HtmlConfigurationAttribute.Collaborator.Attribute)).ToList();

            configuration.CommunicatorHandles = document.GetAttibuteNodes(HtmlConfigurationAttribute.Communicator.Attribute).Select(x => x.GetAttributeValue(HtmlConfigurationAttribute.Communicator.Attribute)).ToList();

            foreach (var node in plan)
            {
                if (node == document.Root || node.Element.Type == HtmlElementType.FreeText) { continue; }

                if (node.HasAttribute(HtmlConfigurationAttribute.PresenterConfiguration.Attribute))
                {
                    configuration.Name = node.GetAttributeValue(HtmlConfigurationAttribute.PresenterConfiguration.Attribute);
                    configuration.PresenterSet = node.GetAttributeValue(HtmlConfigurationAttribute.PresenterSet.Attribute);
                    continue;
                }

                //if (node.HasAttribute(HtmlConfigurationAttribute.DomeWrapper.Attribute))
                //{
                //    configuration.ViewWrapper = node.GetAttributeValue(HtmlConfigurationAttribute.DomeWrapper.Attribute);
                //}

                if (node.HasAttribute(HtmlConfigurationAttribute.ImportScript.Attribute))
                {
                    SetImportScriptFrame(node, frame => configuration.ScriptImports.Add(frame));
                    continue;
                }

                if (node.HasAttribute(HtmlConfigurationAttribute.ScriptFrame.Attribute))
                {
                    SetScriptFrameHandle(node, frame => configuration.ScriptFrameHandles.Add(frame));
                    continue;
                }

                if (node.HasAttribute(HtmlConfigurationAttribute.ImportPresenter.Attribute))
                {
                    var import = new FrameReference()
                    {
                        Type = FrameReferenceType.Presenter,
                        PackageAlias = node.GetAttributeValue(HtmlConfigurationAttribute.ImportPresenter.Attribute),
                        SetName = node.GetAttributeValue(HtmlConfigurationAttribute.ImportSet.Attribute),
                        Frame = node.GetAttributeValue(HtmlConfigurationAttribute.Frame.Attribute),
                        Identifier = node.GetAttributeValue(HtmlConfigurationAttribute.Identifier.Attribute, trimResult: true)
                    };
                    configuration.PresenterSetImports.Add(import);
                    continue;
                }

                if (node.HasAttribute(HtmlConfigurationAttribute.PresenterFrame.Attribute))
                {
                    var presenter = new PresenterFrameHandle()
                    {
                        Frame = node.GetAttributeValue(HtmlConfigurationAttribute.PresenterFrame.Attribute),
                        Handle = node.GetAttributeValue(HtmlConfigurationAttribute.Handle.Attribute)
                    };
                    if (node.HasAttribute(HtmlConfigurationAttribute.Attach.Attribute))
                    {
                        presenter.Attach = node.GetAttributeValue(HtmlConfigurationAttribute.Attach.Attribute).Trim().ToLower() == ScriptFrameAttachMode.Frame.ToString().ToLower() ? ScriptFrameAttachMode.Frame : ScriptFrameAttachMode.Inject;
                    }
                    if (node.HasAttribute(HtmlConfigurationAttribute.Order.Attribute))
                    {
                        if (node.GetAttributeValue(HtmlConfigurationAttribute.Order.Attribute).ToLower() == ScriptFrameHandleOrder.Before.ToString().ToLower()) { presenter.Order = ScriptFrameHandleOrder.Before; };
                    }
                    configuration.PresenterFrameHandles.Add(presenter);
                    continue;
                }


                if (node.HasAttribute(HtmlConfigurationAttribute.StyleCategoryDefaultKey.Attribute) && node.HasAttribute(HtmlConfigurationAttribute.StyleCategoryDefaultValue.Attribute))
                {
                    var key = node.GetAttributeValue(HtmlConfigurationAttribute.StyleCategoryDefaultKey.Attribute);
                    if (configuration.StyleCategoryDefaults.ContainsKey(key)) { configuration.StyleCategoryDefaults.Remove(key); }
                    configuration.StyleCategoryDefaults.Add(key, node.GetAttributeValue(HtmlConfigurationAttribute.StyleCategoryDefaultValue.Attribute));
                    continue;
                }


                if (node.HasAttribute(HtmlConfigurationAttribute.GraphicName.Attribute) && node.HasAttribute(HtmlConfigurationAttribute.GraphicAlias.Attribute))
                {
                    configuration.GraphicReferences.Add(new GraphicReference(node.GetAttributeValue(HtmlConfigurationAttribute.GraphicName.Attribute), node.GetAttributeValue(HtmlConfigurationAttribute.GraphicAlias.Attribute)));
                    SetScriptFrameHandle(node, frame => configuration.ScriptFrameHandles.Add(frame));
                    continue;
                }

            }

            //if (String.IsNullOrWhiteSpace(configuration.ViewWrapper))
            //{
            //    configuration.DomeWrapper = new DomeWrapper { EvalCode = package.DomeWrapper };
            //}
        }

        private void ProcessScriptConfiguration(HtmlDocument document, HularionProject package)
        {
            var configuration = package.AddScriptConfiguration();

            configuration.CommunicatorHandles = document.GetAttibuteNodes(HtmlConfigurationAttribute.Communicator.Attribute).Select(x => x.GetAttributeValue(HtmlConfigurationAttribute.Communicator.Attribute)).ToList();

            var node = document.GetAttibuteNodes(HtmlConfigurationAttribute.ScriptConfiguration.Attribute).FirstOrDefault();
            if (node != null)
            {
                configuration.Name = node.GetAttributeValue(HtmlConfigurationAttribute.ScriptConfiguration.Attribute);
                configuration.ScriptSet = node.GetAttributeValue(HtmlConfigurationAttribute.ScriptSet.Attribute);
            }
            SetImportScriptFrames(document, frame => configuration.Frames.Add(frame));
            SetScriptFrameHandles(document, handle => configuration.ScriptFrameHandles.Add(handle));

            IEnumerable<HtmlNode> nodes;

            nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.ReceivePresenterFrame.Attribute);
            foreach (var n in nodes)
            {
                var receiver = new FrameReceiver();
                receiver.Name = n.GetAttributeValue(HtmlConfigurationAttribute.Frame.Attribute);
                receiver.Reference = n.GetAttributeValue(HtmlConfigurationAttribute.ReceivePresenterFrame.Attribute);
                configuration.ReceivedFrames.Add(receiver);
            }

            nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.AttachObject.Attribute);
            foreach (var n in nodes)
            {
                var indicator = new AttachObjectIndicator()
                {
                    ObjectName = n.GetAttributeValue(HtmlConfigurationAttribute.AttachObject.Attribute),
                    Handle = n.GetAttributeValue(HtmlConfigurationAttribute.Handle.Attribute),
                    PackageKey = n.GetAttributeValue(HtmlConfigurationAttribute.PackageKey.Attribute)
                };
                configuration.AttachIndicators.Add(indicator);
            }

            nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.ImportPresenter.Attribute);
            foreach (var n in nodes)
            {
                var import = new FrameReference()
                {
                    Type = FrameReferenceType.Presenter,
                    PackageAlias = n.GetAttributeValue(HtmlConfigurationAttribute.ImportPresenter.Attribute),
                    SetName = n.GetAttributeValue(HtmlConfigurationAttribute.ImportSet.Attribute),
                    Frame = n.GetAttributeValue(HtmlConfigurationAttribute.Frame.Attribute),
                    Identifier = n.GetAttributeValue(HtmlConfigurationAttribute.Identifier.Attribute, trimResult: true)
                };
                configuration.PresenterSetImports.Add(import);
            }

            nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.PresenterFrame.Attribute);
            foreach (var n in nodes)
            {
                var presenter = new PresenterFrameHandle()
                {
                    Frame = n.GetAttributeValue(HtmlConfigurationAttribute.PresenterFrame.Attribute),
                    Handle = n.GetAttributeValue(HtmlConfigurationAttribute.Handle.Attribute)
                };
                if (n.HasAttribute(HtmlConfigurationAttribute.Attach.Attribute))
                {
                    presenter.Attach = n.GetAttributeValue(HtmlConfigurationAttribute.Attach.Attribute).Trim().ToLower() == ScriptFrameAttachMode.Frame.ToString().ToLower() ? ScriptFrameAttachMode.Frame : ScriptFrameAttachMode.Inject;
                }
                if (n.HasAttribute(HtmlConfigurationAttribute.Order.Attribute))
                {
                    if (n.GetAttributeValue(HtmlConfigurationAttribute.Order.Attribute).ToLower() == ScriptFrameHandleOrder.Before.ToString().ToLower()) { presenter.Order = ScriptFrameHandleOrder.Before; };
                }
                configuration.PresenterFrameHandles.Add(presenter);
                continue;
            }

            //configuration.Collaborators = document.GetAttibuteNodes(HtmlConfigurationAttribute.Collaborator.Attribute).Select(x=>x.GetAttributeValue(HtmlConfigurationAttribute.Collaborator.Attribute)).ToList();

        }

        private void SetImportScriptFrames(HtmlDocument document, Action<FrameReference> assigner)
        {
            var nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.ImportScript.Attribute);
            foreach (var node in nodes)
            {
                SetImportScriptFrame(node, assigner);
            }
        }

        private void SetImportScriptFrame(HtmlNode node, Action<FrameReference> assigner)
        {
            var frame = new FrameReference() { Type = FrameReferenceType.Script, PackageAlias = node.GetAttributeValue(HtmlConfigurationAttribute.ImportScript.Attribute) };
            frame.Frame = node.GetAttributeValue(HtmlConfigurationAttribute.Frame.Attribute);
            frame.SetName = node.GetAttributeValue(HtmlConfigurationAttribute.ImportSet.Attribute);
            frame.Type = FrameReferenceType.Script;
            frame.ProvideFrame = node.GetAttributeValue(HtmlConfigurationAttribute.ProvideFrame.Attribute);
            frame.Identifier = node.GetAttributeValue(HtmlConfigurationAttribute.Identifier.Attribute, trimResult: true);
            assigner(frame);
        }

        private void SetScriptFrameHandles(HtmlDocument document, Action<ScriptFrameHandle> assigner)
        {
            var nodes = document.GetAttibuteNodes(HtmlConfigurationAttribute.ScriptFrame.Attribute, HtmlConfigurationAttribute.Handle.Attribute);
            foreach (var node in nodes)
            {
                SetScriptFrameHandle(node, assigner);
            }
        }

        private void SetScriptFrameHandle(HtmlNode node, Action<ScriptFrameHandle> assigner)
        {
            var frame = new ScriptFrameHandle()
            {
                Frame = node.GetAttributeValue(HtmlConfigurationAttribute.ScriptFrame.Attribute),
                Assign = node.GetAttributeValue(HtmlConfigurationAttribute.Assign.Attribute),
                Handle = node.GetAttributeValue(HtmlConfigurationAttribute.Handle.Attribute),
                Mode = ScriptFrameHandleMode.Presenter
            };
            if (node.HasAttribute(HtmlConfigurationAttribute.Order.Attribute))
            {
                if (node.GetAttributeValue(HtmlConfigurationAttribute.Order.Attribute).ToLower() == ScriptFrameHandleOrder.Before.ToString().ToLower()) { frame.Order = ScriptFrameHandleOrder.Before; };
            }
            if (node.HasAttribute(HtmlConfigurationAttribute.Execute.Attribute))
            {
                frame.Assign = node.GetAttributeValue(HtmlConfigurationAttribute.Execute.Attribute);
            }
            if (node.HasAttribute(HtmlConfigurationAttribute.Attach.Attribute))
            {
                frame.Attach = node.GetAttributeValue(HtmlConfigurationAttribute.Attach.Attribute).Trim().ToLower() == ScriptFrameAttachMode.Frame.ToString().ToLower() ? ScriptFrameAttachMode.Frame : ScriptFrameAttachMode.Inject;
            }

            assigner(frame);
        }

        private void ProccessPresenterSets(HtmlLanguage html, HularionProjectResources file, HularionProject package)
        {
            //var indexAttribute = new HtmlAttribute() { Name = "h-index", QuoteType= HtmlAttribute.InnerQuoteType.Double };
            var hularionPrefix = "h-";
            foreach (var set in file.PresenterSets)
            {
                var presenterSet = package.AddPresenterSet();
                presenterSet.Key = set.Key;
                presenterSet.Name = set.Name;
                foreach (var resource in set.Resources)
                {
                    var presenter = presenterTransform.Transform(package.Key, set.Name, resource);
                    presenterSet.Presenters.Add(presenter);
                }

            }
        }


        public object DecodeParameter(string parameter, string encoding, string delimiter)
        {
            object result = parameter;
            encoding = string.Format("{0}", encoding).ToLower();
            if (encoding == encodingJson)
            {
                result = jsonLanguage.Parse(parameter, new StringCaseModifier(StringCaseDefinition.AllLower)).MakeAnonymous();
            }
            if (encoding.StartsWith(encodingDelimiter) || encoding.StartsWith(encodingPDelimiter))
            {
                result = string.Format("{0}", parameter).Split(new string[] { delimiter }, StringSplitOptions.None);
            }
            return result;
        }


        private string CreateStyleKey()
        {
            return string.Format("style{0}", styleKeyIndex++);
        }

        public IEnumerable<HtmlAttribute> GetNonHXAttributes(HtmlNode node)
        {
            return node.Attributes.Where(x => !x.Name.StartsWith("h-")).ToList();
        }

        public void ScrubAttributes(HtmlNode node)
        {
            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, node, n => n.Nodes.ToArray(), true);
            foreach(var pnode in plan)
            {
                pnode.Attributes = pnode.Attributes.Where(x => !x.Name.StartsWith("h-") || x.Name == HtmlPresenterAttribute.Index.Attribute).ToList();
            }

        }


        public string GetProjectImportLocation(string projectLocation, string reference)
        {

            if(string.IsNullOrWhiteSpace(projectLocation))
            {
                return reference;
            }
            if(string.IsNullOrWhiteSpace(reference))
            {
                return reference;
            }

            reference = reference.Trim(new char[] { ' ', '\t', '\n', '\r' });

            //Up directory.
            if (reference.StartsWith(".."))
            {
                reference = reference.Trim(new char[] { '\\' });
                var importSplits = reference.Split(new char[] { '\\' });
                var upCount = 0;
                for (var i = 0; i < importSplits.Length; i++)
                {
                    if (importSplits[i] == "..")
                    {
                        upCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                var locationTrim = projectLocation.Trim(new char[] { ' ', '\t', '\n', '\r', '\\' });
                var upCounted = 0;
                var upIndex = locationTrim.Length - 1;
                for (var i = locationTrim.Length - 1; i >= 0 && (upCount > upCounted); i--)
                {
                    if (locationTrim[i] == '\\')
                    {
                        upCounted++;
                        upIndex = i;
                    }
                }
                var prefix = locationTrim.Substring(0, upIndex);
                var compiled = new StringBuilder();
                compiled.Append(prefix);
                for(var i=upCount; i< importSplits.Length; i++)
                {
                    compiled.Append('\\');
                    compiled.Append(importSplits[i]);
                }
                var result = compiled.ToString();
                return result;
            }


            return reference;

            //UNC path
            //http

            if (reference.StartsWith(@"\\"))
            {
            }


            
        }

    }
}
