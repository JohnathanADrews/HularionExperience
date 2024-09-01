using HularionCore.Pattern.Topology;
using HularionExperience.Definition;
using HularionExperience.Definition.Project.Concept;
using HularionText.Language.Css;
using HularionText.Language.Html;
using HularionText.Language.Json;
using HularionText.StringCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HularionExperience.PackageBuilder.ProjectResource.Transforms
{
    /// <summary>
    /// Transoforms presenter resource files into Presenter objects.
    /// </summary>
    public class PresenterTransform
    {
        private static TreeTraverser<HtmlNode> traverser = new TreeTraverser<HtmlNode>();
        private static JsonLanguage jsonLanguage = new();
        private static CssLanguage cssLanguage = new();
        private static HtmlLanguage html = new();

        private const string hularionPrefix = "h-";
        private const string classIndicator = "class";
        private const string styleIndicator = "style";
        private const string encodingDelimiter = "delimiter";
        private const string encodingPDelimiter = "pdelimiter";
        private const string encodingTypeSingle = "single";
        private const string encodingTypeParams = "params";
        private const string encodingJson = "json";


        public Func<string> createStyleKey;
        public Func<int> nextComponentTemplateId;


        public PresenterTransform(Func<string> createStyleKey, Func<int> nextComponentTemplateId)
        {
            this.createStyleKey = createStyleKey;
            this.nextComponentTemplateId = nextComponentTemplateId;
        }


        public Presenter Transform(string packageKey, string setName, ResourceFile resource)
        {
            var viewWrap = true;
            var scopedPrefix = string.Format("{0}-{1}-{2} > ", packageKey, setName, resource.Name);
            var unscopedPrefix = string.Format("{0}-{1}-{2}", packageKey, setName, resource.Name);
            var presenter = new Presenter();
            presenter.Key = resource.Key;
            presenter.Name = resource.Name;
            presenter.CssClass = string.Format("{0}-{1}-{2} ", packageKey, setName, resource.Name);
            var document = html.Parse(resource.Content);
            var scriptNodes = document.GetNodesOfElementType(HtmlElementType.Script);
            document.RemoveNodes(scriptNodes.ToArray());
            presenter.Scripts.AddRange(scriptNodes.Select(x => x.EmbeddedLanguageContent).ToList());
            var styleNodes = document.GetNodesOfElementType(HtmlElementType.Style);
            document.RemoveNodes(styleNodes.ToArray());
            presenter.Styles.AddRange(styleNodes.Select(styleNode =>
            {
                var cssDoc = cssLanguage.Parse(styleNode.EmbeddedLanguageContent);
                var scopeAttribute = styleNode.GetAttributes(HtmlPresenterAttribute.StyleScoped.Attribute).FirstOrDefault();
                var stylePrefix = "." + (scopeAttribute == null || scopeAttribute.Value != "true" ? unscopedPrefix + " " : scopedPrefix);
                foreach (var block in cssDoc.Blocks)
                {
                    bool thisFound = false;
                    for (var i = 0; i < block.SelectorParts.Count; i++)
                    {
                        if (block.SelectorParts[i] == "this")
                        {
                            block.SelectorParts[i] = unscopedPrefix;
                            thisFound = true;
                            break;
                        }
                    }
                    if (!thisFound)
                    {
                        block.AddSelectorPrefix(stylePrefix);
                    }
                }
                var style = new PresenterStyle()
                {
                    Key = createStyleKey(),
                    CategoryKey = styleNode.GetAttributeValue(HtmlPresenterAttribute.StyleCategoryKey.Attribute),
                    CategoryValue = styleNode.GetAttributeValue(HtmlPresenterAttribute.StyleCategoryValue.Attribute),
                    Content = cssDoc.ToDocumentString(),
                    SetName = setName,
                    PresenterName = presenter.Name
                };
                return style;
                //return cssDoc.ToDocumentString();
            }).ToList());

            //var nodes = document.GetAttibuteNodes(HtmlPresenterAttribute.PackageImport.Attribute);
            //document.RemoveNodes(nodes.ToArray());
            var remove = new HashSet<HtmlNode>();

            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, document.Root, node => node.Nodes.ToArray(), true);
            var index = 1;
            var templates = new HashSet<HtmlNode>();
            var templatePresenters = new Dictionary<HtmlNode, PresenterReference>();
            var templateComponents = new Dictionary<HtmlNode, PresenterTemplate>();
            var templateInstances = new Dictionary<HtmlNode, TemplateInstance>();
            var presenters = new Dictionary<HtmlNode, PresenterReference>();
            var components = new HashSet<HtmlNode>();
            var componentHandlers = new List<HtmlNode>();
            var scriptImports = new List<HtmlNode>();
            var presenterImports = new List<HtmlNode>();
            var communicators = new List<HtmlNode>();
            var dataSources = new List<HtmlNode>();
            var viewHandles = new List<HtmlNode>();
            var unknown = new List<HtmlNode>();
            var clones = new HashSet<HtmlNode>();
            var cloneInstances = new Dictionary<HtmlNode, CloneInstance>();
            var proxies = new HashSet<HtmlNode>();
            var publishers = new HashSet<HtmlNode>();
            var subscribers = new HashSet<HtmlNode>();
            var systemSubscribers = new HashSet<HtmlNode>();
            var graphics = new HashSet<HtmlNode>();
            var implements = new HashSet<HtmlNode>();
            HtmlAttribute attribute;

            foreach (var node in plan)
            {

                if (node == document.Root || node.Element.Type == HtmlElementType.FreeText) { continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Templates.Attribute).FirstOrDefault();
                if (attribute != null) { remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.ScriptFrame.Attribute).FirstOrDefault();
                if (attribute != null) { remove.Add(node); scriptImports.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.PresenterFrame.Attribute).FirstOrDefault();
                if (attribute != null) { remove.Add(node); presenterImports.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Communicator.Attribute).FirstOrDefault();
                if (attribute != null) { remove.Add(node); communicators.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.DataSource.Attribute).FirstOrDefault();
                if (attribute != null) { remove.Add(node); dataSources.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.ComponentHandler.Attribute).FirstOrDefault();
                if (attribute != null) { componentHandlers.Add(node); remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Proxy.Attribute).FirstOrDefault();
                if (attribute != null) { proxies.Add(node); remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Publisher.Attribute).FirstOrDefault();
                if (attribute != null) { publishers.Add(node); remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Subscriber.Attribute).FirstOrDefault();
                if (attribute != null) { subscribers.Add(node); remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.SystemSubscriber.Attribute).FirstOrDefault();
                if (attribute != null) { systemSubscribers.Add(node); remove.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.DomeWrapper.Attribute).FirstOrDefault();
                if (attribute != null) { presenter.ViewWrapper = node.GetAttributeValue(HtmlPresenterAttribute.DomeWrapper.Attribute); remove.Add(node); continue; }

                //Set the node index.
                node.Attributes.Add(new HtmlAttribute() { Name = HtmlPresenterAttribute.Index.Attribute, Value = string.Format("{0}", index++) });

                attribute = node.GetAttributes(HtmlPresenterAttribute.Template.Attribute).FirstOrDefault();
                if (attribute != null) { node.Parent.RemoveNodes(node); templates.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Clone.Attribute).FirstOrDefault();
                if (attribute != null) { node.Parent.RemoveNodes(node); clones.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Presenter.Attribute).FirstOrDefault();
                if (attribute != null) { presenters.Add(node, AssignPresenterReference(node)); continue; }
                //if (attribute != null) { presenters.Add(node, new PresenterReference()); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Component.Attribute).FirstOrDefault();
                if (attribute != null) { components.Add(node); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Access.Attribute).FirstOrDefault();
                if (attribute != null) { GetMemberAccess(node, presenter); continue; }

                attribute = node.GetAttributes(HtmlPresenterAttribute.TemplateInstance.Attribute).FirstOrDefault();
                if (attribute != null)
                {
                    var ancestors = node.GetAncestors();
                    bool failed = false;
                    foreach (var ancestor in ancestors)
                    {
                        if (ancestor.HasAttribute(HtmlPresenterAttribute.Clone.Attribute)
                            || ancestor.HasAttribute(HtmlPresenterAttribute.Template.Attribute))
                        {
                            failed = true;
                            break;
                        }
                    }
                    if (!failed)
                    {
                        templateInstances.Add(node, new TemplateInstance());
                    }
                    continue;
                }

                attribute = node.GetAttributes(HtmlPresenterAttribute.CloneInstance.Attribute).FirstOrDefault();
                if (attribute != null)
                {
                    var ancestors = node.GetAncestors();
                    bool failed = false;
                    foreach (var ancestor in ancestors)
                    {
                        if (ancestor.HasAttribute(HtmlPresenterAttribute.Clone.Attribute)
                            || ancestor.HasAttribute(HtmlPresenterAttribute.Template.Attribute))
                        {
                            failed = true;
                            break;
                        }
                    }
                    if (!failed)
                    {
                        cloneInstances.Add(node, new CloneInstance());
                    }
                    continue;
                }

                attribute = node.GetAttributes(HtmlPresenterAttribute.Graphic.Attribute).FirstOrDefault();
                if (node.HasAttribute(HtmlPresenterAttribute.Graphic.Attribute))
                {
                    var graphic = new PresenterGraphicReference(node.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute), node.GetAttributeValue(HtmlPresenterAttribute.Graphic.Attribute), node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute));
                    presenter.GraphicReferences.Add(graphic);
                    if (graphic.IsThis || graphic.IsHandle)
                    {
                        remove.Add(node);
                        continue;
                    }
                }


                attribute = node.GetAttributes(HtmlPresenterAttribute.Handle.Attribute).FirstOrDefault();
                if (attribute != null) { viewHandles.Add(node); continue; }


                attribute = node.GetAttributesHavingPrefix(hularionPrefix).FirstOrDefault();
                if (attribute != null) { unknown.Add(node); }

                if (node.Element != null && node.Element.Type == HtmlElementType.Unknown)
                {
                    node.Element.Type = HtmlElementType.Div;
                }

                if (node.Element.Type == HtmlElementType.Unknown) { remove.Add(node); }

            }

            foreach (var node in templates)
            {
                CreatePresenterTemplate(node, presenter, templatePresenters, components, templateComponents);
            }

            presenter.TemplateInstances = templateInstances.Select(x => CreateTemplateInstance(x.Key, presenter).Resource).ToList();



            foreach (var node in clones)
            {
                var item = CreatePresenterClone(node);
                presenter.Clones[item.Resource.Name] = item.Resource;
            }

            presenter.CloneInstances = cloneInstances.Keys.Select(x => CreateCloneInstance(x).Resource).ToList();

            foreach (var np in presenters)
            {
                HtmlNode n = np.Key;
                var isTemplate = false;
                while (n != document.Root)
                {
                    if (templates.Contains(n)) { isTemplate = true; break; };
                    n = n.Parent;
                }
                if (isTemplate) { break; }
                presenter.Presenters.Add(np.Value);
                //np.Value.PresenterName = np.Key.GetAttributes(HtmlPresenterAttribute.Presenter.Attribute).FirstOrDefault().Value;
                //attribute = np.Key.GetAttributes(HtmlPresenterAttribute.Frame.Attribute).FirstOrDefault();
                np.Value.ConstructorParameter = np.Key.GetAttributeValue(HtmlPresenterAttribute.ConstructorParameter.Attribute);
                np.Value.StartParameter = np.Key.GetAttributeValue(HtmlPresenterAttribute.StartParameter.Attribute);
                //if (attribute != null) { np.Value.Frame = attribute.Value; }
                //attribute = np.Key.GetAttributes(HtmlPresenterAttribute.SetName.Attribute).FirstOrDefault();
                //if (attribute != null) { np.Value.PackageAlias = attribute.Value; }
                //attribute = np.Key.GetAttributes(HtmlPresenterAttribute.SetName.Attribute).FirstOrDefault();
                //if (attribute != null) { np.Value.SetName = attribute.Value; }
                attribute = np.Key.GetAttributes(HtmlPresenterAttribute.Handle.Attribute).FirstOrDefault();
                if (attribute != null) { np.Value.Handle = attribute.Value; }
                attribute = np.Key.GetAttributes(HtmlPresenterAttribute.Index.Attribute).FirstOrDefault();
                np.Value.Index = attribute.Value;
                np.Value.CssClass = np.Key.GetAttributeValue(classIndicator);
                if (np.Value.CssClass == "breakOnClass")
                {

                }
                np.Value.Style = np.Key.GetAttributeValue(styleIndicator);
                //np.Key.Attributes.Clear();
                ScrubNodeAttributes(np.Key);
                np.Key.Attributes.Add(attribute);
            }
            var componentMap = new Dictionary<HtmlNode, PresenterComponent>();
            foreach (var node in components)
            {
                var component = new PresenterComponent();
                componentMap.Add(node, component);
                component.PresenterName = node.GetAttributes(HtmlPresenterAttribute.Component.Attribute).FirstOrDefault().Value;
                if (component.PresenterName.Contains("/"))
                {
                    var splits = component.PresenterName.Split("/");
                    component.PresenterName = splits[1].Trim();
                    component.Frame = splits[0].Trim();
                }
                if (node.HasAttribute(HtmlPresenterAttribute.Frame.Attribute))
                {
                    component.Frame = node.GetAttributeValue(HtmlPresenterAttribute.Frame.Attribute);
                }
                if (component.PresenterName.Contains("=>"))
                {
                    var splits = component.PresenterName.Split("=>");
                    component.PresenterName = splits[0].Trim();
                    if (component.PresenterName == "#")
                    {
                        component.AsElement = true;
                        component.ComponentTemplateId = nextComponentTemplateId();
                    }
                    component.TargetHandler = splits[1].Trim();
                }
                if (node.HasAttribute(HtmlPresenterAttribute.Handler.Attribute))
                {
                    component.TargetHandler = node.GetAttributeValue(HtmlPresenterAttribute.Handler.Attribute);
                }
                //if (string.IsNullOrWhiteSpace(component.Frame))
                //{
                //    component.Frame = "this";
                //}

                component.Subscriptions.AddRange(Subscriptioneference.Parse(node.GetAttributeValue(HtmlPresenterAttribute.Subscribe.Attribute)));

                //attribute = node.GetAttributes(HtmlPresenterAttribute.PackageAlias.Attribute).FirstOrDefault();
                //if (attribute != null) { component.PackageAlias = attribute.Value; }
                attribute = node.GetAttributes(HtmlPresenterAttribute.SetName.Attribute).FirstOrDefault();
                if (attribute != null) { component.SetName = attribute.Value; }
                attribute = node.GetAttributes(HtmlPresenterAttribute.Handle.Attribute).FirstOrDefault();
                if (attribute != null) { component.Handle = attribute.Value; }
                //attribute = node.GetAttributes(HtmlPresenterAttribute.Handler.Attribute).FirstOrDefault();
                //if (attribute != null) { component.TargetHandler = attribute.Value; }
                attribute = node.GetAttributes(HtmlPresenterAttribute.Index.Attribute).FirstOrDefault();
                if (attribute != null) { component.Index = attribute.Value; }


                component.Parameter = node.GetAttributeValue(HtmlPresenterAttribute.ComponentParameter.Attribute);
                component.ConstructorParameter = node.GetAttributeValue(HtmlPresenterAttribute.ConstructorParameter.Attribute);
                component.StartParameter = node.GetAttributeValue(HtmlPresenterAttribute.StartParameter.Attribute);

                if (templatePresenters.ContainsKey(node.Parent))
                {
                    component.IsTemplate = true;
                    templatePresenters[node.Parent].Components.Add(component);
                    //continue;
                }
                if (templateComponents.ContainsKey(node))
                {
                    component.IsTemplate = true;
                    templateComponents[node].Components.Add(component);
                    //continue;
                }
                if (componentMap.ContainsKey(node.Parent)) { component.ParentIndex = componentMap[node.Parent].Index; }
                if (presenters.ContainsKey(node.Parent))
                {
                    presenters[node.Parent].Components.Add(component);
                    component.ParentIndex = presenters[node.Parent].Index;
                }
                component.CssClass = node.GetAttributeValue(classIndicator);
                component.Style = node.GetAttributeValue(styleIndicator);
                remove.Add(node);


                presenter.Components.Add(component);
            }
            foreach (var node in componentHandlers)
            {
                var handler = new PresenterComponentHandler();

                handler.Handler = node.GetAttributes(HtmlPresenterAttribute.ComponentHandler.Attribute).FirstOrDefault().Value;
                //attribute = node.GetAttributes(HtmlPresenterAttribute.PackageAlias.Attribute).FirstOrDefault();
                //if (attribute != null) { handler.PackageAlias = attribute.Value; }
                //attribute = node.GetAttributes(HtmlPresenterAttribute.SetName.Attribute).FirstOrDefault();
                //if (attribute != null) { handler.SetName = attribute.Value; }
                //attribute = node.GetAttributes(HtmlPresenterAttribute.ComponentPresenter.Attribute).FirstOrDefault();
                //if (attribute != null) { handler.PresenterName = attribute.Value; }
                attribute = node.GetAttributes(HtmlPresenterAttribute.Method.Attribute).FirstOrDefault();
                if (attribute != null) { handler.Method = attribute.Value; }
                attribute = node.GetAttributes(HtmlPresenterAttribute.Encoding.Attribute).FirstOrDefault();
                if (attribute != null)
                {
                    handler.Encoding = attribute.Value;
                    handler.EncodingDelimiter = GetDelimiter(handler.Encoding);
                    handler.EncodingType = GetEncodingType(handler.Encoding);
                }

                presenter.ComponentHandlers.Add(handler);
            }
            foreach (var node in proxies)
            {
                if (!node.HasAttribute(HtmlPresenterAttribute.Proxy.Attribute)) { continue; }
                if (node.HasAttribute(HtmlPresenterAttribute.Interface.Attribute) || node.HasAttribute(HtmlPresenterAttribute.StaticInterface.Attribute))
                {
                    var pi = new PresenterProxyInterface()
                    {
                        Proxy = node.GetAttributeValue(HtmlPresenterAttribute.Proxy.Attribute),
                        Method = node.GetAttributeValue(HtmlPresenterAttribute.Method.Attribute)
                    };
                    if (string.IsNullOrWhiteSpace(pi.Method) && pi.Proxy.Contains("=>"))
                    {
                        var splits = pi.Proxy.Split("=>");
                        pi.Proxy = splits[0];
                        pi.Method = splits[1];
                    }
                    if (string.IsNullOrWhiteSpace(pi.Method))
                    {
                        pi.Method = pi.Proxy;
                    }
                    if (node.HasAttribute(HtmlPresenterAttribute.Interface.Attribute))
                    {
                        pi.Interface = node.GetAttributeValue(HtmlPresenterAttribute.Interface.Attribute);
                        pi.Type = PresenterProxyInterface.InterfaceType.Instance;
                    }
                    if (node.HasAttribute(HtmlPresenterAttribute.StaticInterface.Attribute))
                    {
                        pi.Interface = node.GetAttributeValue(HtmlPresenterAttribute.StaticInterface.Attribute);
                        pi.Type = PresenterProxyInterface.InterfaceType.Static;
                    }
                    if (!string.IsNullOrWhiteSpace(pi.Method)! && !string.IsNullOrWhiteSpace(pi.Method))
                    {
                        presenter.InterfaceMethods.Add(pi);
                    }
                }
                else
                {
                    var pi = new PresenterProxy()
                    {
                        Name = node.GetAttributeValue(HtmlPresenterAttribute.Proxy.Attribute),
                        Method = node.GetAttributeValue(HtmlPresenterAttribute.Method.Attribute)
                    };
                    if (string.IsNullOrWhiteSpace(pi.Method) && pi.Name.Contains("=>"))
                    {
                        var splits = pi.Name.Split("=>");
                        pi.Name = splits[0];
                        pi.Method = splits[1];
                    }
                    if (string.IsNullOrWhiteSpace(pi.Method))
                    {
                        pi.Method = pi.Name;
                    }
                    if (!string.IsNullOrWhiteSpace(pi.Method)! && !string.IsNullOrWhiteSpace(pi.Method))
                    {
                        presenter.PresenterProxies.Add(pi);
                    }
                }
            }
            foreach (var node in publishers)
            {
                if (!node.HasAttribute(HtmlPresenterAttribute.Publisher.Attribute)) { continue; }
                var publisher = new PresenterPublisher()
                {
                    Name = node.GetAttributeValue(HtmlPresenterAttribute.Publisher.Attribute)
                };
                presenter.Publishers.Add(publisher);
            }
            foreach (var node in subscribers)
            {
                if (!node.HasAttribute(HtmlPresenterAttribute.Subscriber.Attribute)) { continue; }
                var subscriber = new PresenterSubscriber()
                {
                    Name = node.GetAttributeValue(HtmlPresenterAttribute.Subscriber.Attribute),
                    Method = node.GetAttributeValue(HtmlPresenterAttribute.Method.Attribute)
                };
                presenter.Subscribers.Add(subscriber);
            }
            foreach (var node in systemSubscribers)
            {
                if (!node.HasAttribute(HtmlPresenterAttribute.SystemSubscriber.Attribute)) { continue; }
                var systemSubscriber = new PresenterSystemSubscriber()
                {
                    Name = node.GetAttributeValue(HtmlPresenterAttribute.SystemSubscriber.Attribute),
                    Method = node.GetAttributeValue(HtmlPresenterAttribute.Method.Attribute)
                };
                presenter.SystemSubscribers.Add(systemSubscriber);
            }
            foreach (var node in viewHandles)
            {
                //If the handle is within a template, then it will not be here.
                if (node.GetAttributes(HtmlPresenterAttribute.Handle.Attribute).FirstOrDefault() == null) { continue; }
                var n = node;
                var valid = true;
                while (n != null && n != document.Root)
                {
                    if (n.GetAttributes(HtmlPresenterAttribute.Template.Attribute).Count() > 0 || n.GetAttributes(HtmlPresenterAttribute.Clone.Attribute).Count() > 0)
                    {
                        valid = false;
                        break;
                    }
                    n = n.Parent;
                }
                if (!valid) { continue; }
                var handle = new PresenterHandle();
                handle.Index = node.GetAttributes(HtmlPresenterAttribute.Index.Attribute).FirstOrDefault().Value;
                handle.Name = node.GetAttributes(HtmlPresenterAttribute.Handle.Attribute).FirstOrDefault().Value;
                var wrapper = node.GetAttributes(HtmlPresenterAttribute.DomeWrapper.Attribute).FirstOrDefault();
                if (wrapper != null) { handle.Wrapper = wrapper.Value; }
                presenter.Handles.Add(handle);
            }
            foreach (var node in scriptImports)
            {
                if (node.HasAttribute(HtmlPresenterAttribute.ScriptFrame.Attribute) && node.HasAttribute(HtmlPresenterAttribute.Assign.Attribute) && node.HasAttribute(HtmlPresenterAttribute.Handle.Attribute))
                {
                    var import = new ScriptFrameHandle()
                    {
                        Frame = node.GetAttributeValue(HtmlPresenterAttribute.ScriptFrame.Attribute),
                        Assign = node.GetAttributeValue(HtmlPresenterAttribute.Assign.Attribute),
                        Handle = node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute)
                    };
                    presenter.ScriptFrameHandles.Add(import);
                }
            }
            foreach (var node in presenterImports)
            {
                if (node.HasAttribute(HtmlPresenterAttribute.PresenterFrame.Attribute) && node.HasAttribute(HtmlPresenterAttribute.Handle.Attribute))
                {
                    var import = new PresenterFrameHandle()
                    {
                        Frame = node.GetAttributeValue(HtmlPresenterAttribute.PresenterFrame.Attribute),
                        Handle = node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute)
                    };
                    presenter.PresenterFrameHandles.Add(import);
                }
            }

            foreach (var node in communicators)
            {

            }

            foreach (var node in dataSources)
            {

            }



            document.RemoveNodes(remove.ToArray());

            if (viewWrap)
            {
                var node = new HtmlNode() { Element = HtmlElement.Div };
                foreach (var n in document.Root.Nodes) { node.Nodes.Add(n); }
                document.Root.Nodes.Clear();
                document.Root.Nodes.Add(node);
            }

            presenter.View = document.ToDocumentString(new HtmlSerializationOptions() { Format = HtmlDocumentStringFormat.Minimal });


            return presenter;
        }


        public PresenterReference? AssignPresenterReference(HtmlNode node)
        {
            if (!node.HasAttribute(HtmlPresenterAttribute.Presenter.Attribute)) { return null; }
            var result = new PresenterReference()
            {
                PresenterName = node.GetAttributeValue(HtmlPresenterAttribute.Presenter.Attribute),
                Frame = node.GetAttributeValue(HtmlPresenterAttribute.PresenterFrame.Attribute),
                Handle = node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute),
                Index = node.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute)
            };

            if (result.PresenterName.Contains('/'))
            {
                var splits = result.PresenterName.Split('/');
                result.PresenterName = splits[1];
                result.Frame = splits[0];
            }
            result.Subscriptions.AddRange(Subscriptioneference.Parse(node.GetAttributeValue(HtmlPresenterAttribute.Subscribe.Attribute)));


            result.Style = node.GetAttributeValue("style");
            result.CssClass = node.GetAttributeValue("class");

            return result;
        }


        public ResourceBuildResult<PresenterTemplate> CreatePresenterTemplate(HtmlNode source, Presenter presenter, Dictionary<HtmlNode, PresenterReference> templatePresenters, HashSet<HtmlNode> components, Dictionary<HtmlNode, PresenterTemplate> templateComponents)
        {
            var result = new ResourceBuildResult<PresenterTemplate>();

            var template = new PresenterTemplate();
            result.Resource = template;
            template.Name = source.GetAttributeValue(HtmlPresenterAttribute.Template.Attribute);
            template.Method = source.GetAttributeValue(HtmlPresenterAttribute.Method.Attribute);

            if (presenter.Templates.ContainsKey(template.Name))
            {
                result.WasSuccessful = false;
                result.Messages.Add(new ResourceBuildMessage()
                {
                    Message = String.Format("Presenter '{0}' was instructed to add the template '{1}' more than once.", presenter.Name, template.Name),
                    Status = ResouceBuildMessageStatus.Error
                });
                return result;
            }

            if (source.HasAttribute(HtmlPresenterAttribute.Encoding.Attribute))
            {
                template.Encoding = source.GetAttributeValue(HtmlPresenterAttribute.Encoding.Attribute);
                template.EncodingDelimiter = GetDelimiter(template.Encoding);
                template.EncodingType = GetEncodingType(template.Encoding);
            }

            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, source, node => node.Nodes.ToArray(), true);
            foreach (var node in plan)
            {
                if (node.HasAttribute(HtmlPresenterAttribute.Handle.Attribute))
                {
                    var handle = new PresenterHandle()
                    {
                        Name = node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute),
                        Index = node.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute)
                    };
                    template.Handles.Add(handle);
                }


                if (components.Contains(node) //The node is a component within the template.
                    && !components.Contains(node.Parent) //If the parent is a component, then this node belongs to that component structure.
                    && !templatePresenters.ContainsKey(node.Parent))
                {
                    templateComponents.Add(node, template);
                    continue;
                }

                if (node.HasAttribute(HtmlPresenterAttribute.Presenter.Attribute))
                {
                    var presenterReference = AssignPresenterReference(node);
                    templatePresenters.Add(node, presenterReference);
                    template.Presenters.Add(presenterReference);
                    node.RemoveNodes(node.Nodes.ToArray());
                }

                if (node.HasAttribute(HtmlPresenterAttribute.CloneInstance.Attribute))
                {
                    template.CloneInstances.Add(CreateCloneInstance(node).Resource);
                }

                if (node.HasAttribute(HtmlPresenterAttribute.TemplateInstance.Attribute))
                {
                    template.TemplateInstances.Add(CreateTemplateInstance(node, presenter).Resource);
                }

            }
            template.View = source.ToDocumentString(new HtmlSerializationOptions() { Format = HtmlDocumentStringFormat.Minimal });
            presenter.Templates.Add(template.Name, template);
            return result;
        }


        public ResourceBuildResult<TemplateInstance> CreateTemplateInstance(HtmlNode source, Presenter presenter)
        {
            var result = new ResourceBuildResult<TemplateInstance>();
            result.Resource = new TemplateInstance();

            result.Resource.Index = source.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute);
            result.Resource.Name = source.GetAttributeValue(HtmlPresenterAttribute.TemplateInstance.Attribute);
            result.Resource.Handle = source.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute);
            result.Resource.CssClass = source.GetAttributeValue(classIndicator);
            result.Resource.Style = source.GetAttributeValue(styleIndicator);

            var template = presenter.Templates[result.Resource.Name];

            if (source.HasAttribute(HtmlPresenterAttribute.Parameter.Attribute))
            {
                result.Resource.Parameter = source.GetAttributeValue(HtmlPresenterAttribute.Parameter.Attribute);
                result.Resource.DecodedParameter = DecodeParameter(result.Resource.Parameter, template.Encoding, template.EncodingDelimiter);
            }

            return result;
        }


        public ResourceBuildResult<PresenterClone> CreatePresenterClone(HtmlNode source)
        {
            var result = new ResourceBuildResult<PresenterClone>();
            var clone = new PresenterClone();
            result.Resource = clone;
            clone.Name = source.GetAttributeValue(HtmlPresenterAttribute.Clone.Attribute);

            var traverser = new TreeTraverser<HtmlNode>();
            var plan = traverser.CreateEvaluationPlan(TreeTraversalOrder.ParentLeftRight, source, node => node.Nodes.ToArray(), true);
            foreach (var node in plan)
            {
                if (node.HasAttribute(HtmlPresenterAttribute.Handle.Attribute))
                {
                    clone.Handles.Add(new PresenterHandle()
                    {
                        Index = node.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute),
                        Name = node.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute)
                    });
                }
                if (node.HasAttribute(HtmlPresenterAttribute.CloneInstance.Attribute))
                {
                    clone.CloneInstances.Add(CreateCloneInstance(node).Resource);
                }
                //ScrubAttributes(node);
                //node.Attributes = node.GetAttributes(HtmlPresenterAttribute.Index.Attribute).ToList();
            }

            //ScrubAttributes(source);
            clone.View = source.ToDocumentString(new HtmlSerializationOptions() { Format = HtmlDocumentStringFormat.Minimal });

            return result;
        }


        public ResourceBuildResult<CloneInstance> CreateCloneInstance(HtmlNode source)
        {
            var result = new ResourceBuildResult<CloneInstance>();
            result.Resource = new CloneInstance();
            result.Resource.Index = source.GetAttributeValue(HtmlPresenterAttribute.Index.Attribute);
            result.Resource.Name = source.GetAttributeValue(HtmlPresenterAttribute.CloneInstance.Attribute);
            result.Resource.Handle = source.GetAttributeValue(HtmlPresenterAttribute.Handle.Attribute);
            result.Resource.Class = source.GetAttributeValue(classIndicator);
            result.Resource.Style = source.GetAttributeValue(styleIndicator);
            return result;
        }



        public void ScrubNodeAttributes(HtmlNode node)
        {
            node.Attributes = node.Attributes.Where(x => !x.Name.StartsWith("h-") || x.Name == HtmlPresenterAttribute.Index.Attribute).ToList();
        }


        private string GetDelimiter(string encoding)
        {
            encoding = string.Format("{0}", encoding).Trim().ToLower();
            var delimiter = string.Empty;
            if (encoding.StartsWith(encodingDelimiter)) { encoding = encoding.Substring(encodingDelimiter.Length, encoding.Length - encodingDelimiter.Length); }
            if (encoding.StartsWith(encodingPDelimiter)) { encoding = encoding.Substring(encodingPDelimiter.Length, encoding.Length - encodingPDelimiter.Length); }
            return encoding;
        }

        private string GetEncodingType(string encoding)
        {
            encoding = string.Format("{0}", encoding).Trim().ToLower();
            if (encoding.StartsWith(encodingPDelimiter))
            {
                return encodingTypeParams;
            }
            return encodingTypeSingle;
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


        public void GetMemberAccess(HtmlNode source, Presenter presenter)
        {
            //"h-access=memberName=>get;set;"
            var available = new HashSet<string>(new[] { "get", "set" });

            var accessString = source.GetAttributeValue(HtmlPresenterAttribute.Access.Attribute);

            if (!accessString.Contains("=>"))
            {
                return;
            }
            var mps = accessString.Split("=>", StringSplitOptions.RemoveEmptyEntries);
            if(mps.Length < 2)
            {
                return;
            }
            var memberName = mps[0];
            var accessors = new HashSet<string>(mps[1].Split(";", StringSplitOptions.RemoveEmptyEntries).Select(x=>x.ToLower()));


            accessors = new HashSet<string>(accessors.Where(x => available.Contains(x)));

            if(accessors.Count == 0)
            {
                return;
            }

            if (!presenter.Members.ContainsKey(memberName))
            {
                presenter.Members[memberName] = new PresenterMemberInfo() { Name = memberName };
            }
            var memberInfo = presenter.Members[memberName];

            foreach (var accessor in accessors)
            {
                memberInfo.Access.Add(accessor);
            }

        }

    }
}
