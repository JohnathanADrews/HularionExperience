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
using System.Text;

namespace HularionExperience.PackageBuilder
{
    public class HtmlResourceAttribute
    {

        public string Name { get; set; }

        public string Attribute { get { return String.Format("h-{0}", Name); } }

        public string Description { get; set; }

        public List<HtmlResourceAttribute> Requirements = new List<HtmlResourceAttribute>();

        public List<HtmlResourceAttribute> Options = new List<HtmlResourceAttribute>();

        public bool IsInitiator { get; set; } = false;


    }


    public class HtmlPackageAttribute : HtmlResourceAttribute
    {
        public static HtmlPackageAttribute HXPackage { get; private set; } = new HtmlPackageAttribute() { Name = "hxPackage" };
        public static HtmlPackageAttribute ProductKey { get; private set; } = new HtmlPackageAttribute() { Name = "product-key" };
        public static HtmlPackageAttribute PackageKey { get; private set; } = new HtmlPackageAttribute() { Name = "package-key" };
        public static HtmlPackageAttribute Version { get; private set; } = new HtmlPackageAttribute() { Name = "version" };
        public static HtmlPackageAttribute PackageName { get; private set; } = new HtmlPackageAttribute() { Name = "package-name" };
        public static HtmlPackageAttribute Brand { get; private set; } = new HtmlPackageAttribute() { Name = "brand-name" };
        public static HtmlPackageAttribute Link { get; private set; } = new HtmlPackageAttribute() { Name = "link" };
        public static HtmlPackageAttribute DomeWrapper { get; private set; } = new HtmlPackageAttribute() { Name = "dome-wrapper", Description = "Document Object Model Element Wrapper: Replaces the DOM element with a proxy." };
        public static HtmlPackageAttribute ScriptFrame { get; private set; } = new HtmlPackageAttribute() { Name = "script-frame" };
        public static HtmlPackageAttribute ImportSet { get; private set; } = new HtmlPackageAttribute() { Name = "import-set" };
        public static HtmlPackageAttribute DefaultApplication { get; private set; } = new HtmlPackageAttribute() { Name = "default-application" };
        public static HtmlPackageAttribute PackageImport { get; private set; } = new HtmlPackageAttribute() { Name = "package-import" };
        public static HtmlPackageAttribute ScriptLoadContext { get; private set; } = new HtmlPackageAttribute() { Name = "script-load-context" };
        public static HtmlPackageAttribute ScriptSet { get; private set; } = new HtmlPackageAttribute() { Name = "script-set" };
        public static HtmlPackageAttribute Graphic { get; private set; } = new HtmlPackageAttribute() { Name = "graphic" };
        public static HtmlPackageAttribute Alias { get; private set; } = new HtmlPackageAttribute() { Name = "alias" };
        public static HtmlPackageAttribute License { get; private set; } = new HtmlPackageAttribute() { Name = "license" };
        public static HtmlPackageAttribute LicenseRequired { get; private set; } = new HtmlPackageAttribute() { Name = "license-must-agree" };
        public static HtmlPackageAttribute Tags { get; private set; } = new HtmlPackageAttribute() { Name = "tags" };
        public static HtmlPackageAttribute Contributor { get; private set; } = new HtmlPackageAttribute() { Name = "contributor" };
        public static HtmlPackageAttribute Role { get; private set; } = new HtmlPackageAttribute() { Name = "role" };
        public static HtmlPackageAttribute ObjectKey { get; private set; } = new HtmlPackageAttribute() { Name = "key" };
        public static HtmlPackageAttribute ObjectName { get; private set; } = new HtmlPackageAttribute() { Name = "name" };
        public static HtmlPackageAttribute ObjectDescription { get; private set; } = new HtmlPackageAttribute() { Name = "description" };
        public static HtmlPackageAttribute ServerRouter { get; private set; } = new HtmlPackageAttribute() { Name = "server-router" };
        public static HtmlPackageAttribute SystemRouter { get; private set; } = new HtmlPackageAttribute() { Name = "system-router" };
        public static HtmlPackageAttribute MethodGroup { get; private set; } = new HtmlPackageAttribute() { Name = "method-group" };
        public static HtmlPackageAttribute Handle { get; private set; } = new HtmlPackageAttribute() { Name = "handle" };
        public static HtmlPackageAttribute Attach { get; private set; } = new HtmlPackageAttribute() { Name = "attach" };
        public static HtmlPackageAttribute Project { get; private set; } = new HtmlPackageAttribute() { Name = "project" };
    }

    public class HtmlApplicationAttribute : HtmlResourceAttribute
    {
        public static HtmlPackageAttribute Application { get; private set; } = new HtmlPackageAttribute() { Name = "application" };
        public static HtmlPackageAttribute ApplicationKey { get; private set; } = new HtmlPackageAttribute() { Name = "application-key" };
        public static HtmlPackageAttribute ApplicationName { get; private set; } = new HtmlPackageAttribute() { Name = "application-name" };
        public static HtmlPackageAttribute ApplicationPresenter { get; private set; } = new HtmlPackageAttribute() { Name = "application-presenter" };
        public static HtmlPackageAttribute ApplicationTitle { get; private set; } = new HtmlPackageAttribute() { Name = "application-title" };
        public static HtmlPackageAttribute ApplicationIsDefault { get; private set; } = new HtmlPackageAttribute() { Name = "application-is-default" };
        public static HtmlPackageAttribute CommunicationRouteProvider { get; private set; } = new HtmlPackageAttribute() { Name = "communication-route-provider" };
        public static HtmlPackageAttribute PackageImport { get; private set; } = new HtmlPackageAttribute() { Name = "package-import" };
        public static HtmlPackageAttribute PackageImportKey { get; private set; } = new HtmlPackageAttribute() { Name = "package-import-key" };
        public static HtmlPackageAttribute PackageImportName { get; private set; } = new HtmlPackageAttribute() { Name = "package-import-name" };
        public static HtmlPackageAttribute PackageImportSource { get; private set; } = new HtmlPackageAttribute() { Name = "package-import-source" };
        public static HtmlPackageAttribute PackageIndicator { get; private set; } = new HtmlPackageAttribute() { Name = "package-indicator" };
        public static HtmlPackageAttribute PackageKey { get; private set; } = new HtmlPackageAttribute() { Name = "package-key" };
        public static HtmlPackageAttribute PackageName { get; private set; } = new HtmlPackageAttribute() { Name = "package-name" };
        public static HtmlPackageAttribute PackageSource { get; private set; } = new HtmlPackageAttribute() { Name = "package-source" };
        public static HtmlPackageAttribute PresenterConfiguration { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-configuration" };
        public static HtmlPackageAttribute PresenterSet { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-set" };
        public static HtmlPackageAttribute ScriptHandle { get; private set; } = new HtmlPackageAttribute() { Name = "script-handle" };
        public static HtmlPackageAttribute ScriptImport { get; private set; } = new HtmlPackageAttribute() { Name = "script-import" };
        public static HtmlPackageAttribute ScriptLoadContext { get; private set; } = new HtmlPackageAttribute() { Name = "script-load-context" };
        public static HtmlPackageAttribute StyleConfiguration { get; private set; } = new HtmlPackageAttribute() { Name = "style-configuration" };
        public static HtmlPackageAttribute Link { get; private set; } = new HtmlPackageAttribute() { Name = "link" };
        public static HtmlPackageAttribute ObjectName { get; private set; } = new HtmlPackageAttribute() { Name = "name" };
        public static HtmlPackageAttribute ObjectDescription { get; private set; } = new HtmlPackageAttribute() { Name = "description" };
        public static HtmlPackageAttribute ObjectKey { get; private set; } = new HtmlPackageAttribute() { Name = "key" };
    }

    public class HtmlConfigurationAttribute : HtmlResourceAttribute
    {
        public static HtmlPackageAttribute PresenterConfiguration { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-configuration" };
        public static HtmlPackageAttribute PresenterSet { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-set" };
        public static HtmlPackageAttribute PresenterFrame { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-frame" };
        public static HtmlPackageAttribute Assign { get; private set; } = new HtmlPackageAttribute() { Name = "assign" };
        public static HtmlPackageAttribute AssignFrame { get; private set; } = new HtmlPackageAttribute() { Name = "assign-frame" };
        public static HtmlPackageAttribute AssignPresemter { get; private set; } = new HtmlPackageAttribute() { Name = "assign-presenter" };
        public static HtmlPackageAttribute Attach { get; private set; } = new HtmlPackageAttribute() { Name = "attach" };
        public static HtmlPackageAttribute Communicator { get; private set; } = new HtmlPackageAttribute() { Name = "communicator" };
        public static HtmlPackageAttribute DataSource { get; private set; } = new HtmlPackageAttribute() { Name = "data-source" };
        public static HtmlPackageAttribute Execute { get; private set; } = new HtmlPackageAttribute() { Name = "execute" };
        public static HtmlPackageAttribute Frame { get; private set; } = new HtmlPackageAttribute() { Name = "frame" };
        public static HtmlPackageAttribute FrameShare { get; private set; } = new HtmlPackageAttribute() { Name = "frame-share" };
        public static HtmlPackageAttribute Handle { get; private set; } = new HtmlPackageAttribute() { Name = "handle" };
        public static HtmlPackageAttribute Identifier { get; private set; } = new HtmlPackageAttribute() { Name = "identifier" };
        public static HtmlPackageAttribute ImportPresenter { get; private set; } = new HtmlPackageAttribute() { Name = "import-presenter" };
        public static HtmlPackageAttribute ImportScript { get; private set; } = new HtmlPackageAttribute() { Name = "import-script" };
        public static HtmlPackageAttribute ImportSet { get; private set; } = new HtmlPackageAttribute() { Name = "import-set" };
        public static HtmlPackageAttribute Order { get; private set; } = new HtmlPackageAttribute() { Name = "order" };
        public static HtmlPackageAttribute PackageKey { get; private set; } = new HtmlPackageAttribute() { Name = "package-key" };
        public static HtmlPackageAttribute DomeWrapper { get; private set; } = new HtmlPackageAttribute() { Name = "dome-wrapper", Description = "Document Object Model Element Wrapper: Replaces the DOM element with a proxy." };
        public static HtmlPackageAttribute ScriptFrame { get; private set; } = new HtmlPackageAttribute() { Name = "script-frame" };
        public static HtmlPackageAttribute VersionKey { get; private set; } = new HtmlPackageAttribute() { Name = "version" };
        public static HtmlPackageAttribute StyleCategoryDefaultKey { get; private set; } = new HtmlPackageAttribute() { Name = "style-category-default-key" };
        public static HtmlPackageAttribute StyleCategoryDefaultValue { get; private set; } = new HtmlPackageAttribute() { Name = "style-category-default-value" };

        public static HtmlPackageAttribute StyleConfiguration { get; private set; } = new HtmlPackageAttribute() { Name = "style-configuration" };
        public static HtmlPackageAttribute Group { get; private set; } = new HtmlPackageAttribute() { Name = "group" };
        public static HtmlPackageAttribute Include { get; private set; } = new HtmlPackageAttribute() { Name = "include" };
        public static HtmlPackageAttribute StyleSheet { get; private set; } = new HtmlPackageAttribute() { Name = "style-sheet" };


        public static HtmlPackageAttribute ScriptConfiguration { get; private set; } = new HtmlPackageAttribute() { Name = "script-configuration" };
        public static HtmlPackageAttribute ScriptSet { get; private set; } = new HtmlPackageAttribute() { Name = "script-set" };

        public static HtmlPackageAttribute AttachObject { get; private set; } = new HtmlPackageAttribute() { Name = "attach-object" };

        public static HtmlPackageAttribute ReceivePresenterFrame { get; private set; } = new HtmlPackageAttribute() { Name = "receive-presenter-frame" };
        public static HtmlPackageAttribute ProvideFrame { get; private set; } = new HtmlPackageAttribute() { Name = "provide-frame" };
        public static HtmlPackageAttribute Contributor { get; private set; } = new HtmlPackageAttribute() { Name = "contributor" };


        public static HtmlPackageAttribute GraphicName { get; private set; } = new HtmlPackageAttribute() { Name = "graphic-name" };
        public static HtmlPackageAttribute GraphicAlias { get; private set; } = new HtmlPackageAttribute() { Name = "graphic-alias" };

    }

    public class HtmlScriptAttribute : HtmlResourceAttribute
    {

    }

    public class HtmlStyleAttribute : HtmlResourceAttribute
    {

    }

    public class HtmlInterfaceAttribute : HtmlResourceAttribute
    {
        public static HtmlPackageAttribute Proxy { get; private set; } = new HtmlPackageAttribute() { Name = "proxy" };
    }

    public class HtmlPresenterAttribute : HtmlResourceAttribute
    {
        public static HtmlPackageAttribute Access { get; private set; } = new HtmlPackageAttribute() { Name = "access" };
        public static HtmlPackageAttribute Assign { get; private set; } = new HtmlPackageAttribute() { Name = "assign" };
        public static HtmlPackageAttribute Clone { get; private set; } = new HtmlPackageAttribute() { Name = "clone" };
        public static HtmlPackageAttribute CloneInstance { get; private set; } = new HtmlPackageAttribute() { Name = "clone-instance" };
        public static HtmlPackageAttribute Code { get; private set; } = new HtmlPackageAttribute() { Name = "code" };
        public static HtmlPackageAttribute Communicator { get; private set; } = new HtmlPackageAttribute() { Name = "communicator" };
        public static HtmlPackageAttribute Component { get; private set; } = new HtmlPackageAttribute() { Name = "component", Description = "A component that will be injected into a target presenter." };
        //public static HtmlPackageAttribute ComponentElement { get; private set; } = new HtmlPackageAttribute() { Name = "component-element", Description="Indicates that the component will be the DOM element having this attribute." };
        public static HtmlPackageAttribute ComponentHandler { get; private set; } = new HtmlPackageAttribute() { Name = "component-handler" };
        public static HtmlPackageAttribute ComponentHandlerMethod { get; private set; } = new HtmlPackageAttribute() { Name = "component-handler-method" };
        public static HtmlPackageAttribute ComponentParameter { get; private set; } = new HtmlPackageAttribute() { Name = "component-parameter" };
        public static HtmlPackageAttribute ComponentParameterEncoding { get; private set; } = new HtmlPackageAttribute() { Name = "component-parameter-encoding" };
        public static HtmlPackageAttribute ComponentPresenter { get; private set; } = new HtmlPackageAttribute() { Name = "component-presenter" };
        public static HtmlPackageAttribute ComponentTarget { get; private set; } = new HtmlPackageAttribute() { Name = "component-target", Description = "The handler of the target presenter that will handle this component." };
        public static HtmlPackageAttribute ConstructorParameter { get; private set; } = new HtmlPackageAttribute() { Name = "constructor-parameter" };
        public static HtmlPackageAttribute DataSource { get; private set; } = new HtmlPackageAttribute() { Name = "data-source" };
        public static HtmlPackageAttribute Encoding { get; private set; } = new HtmlPackageAttribute() { Name = "encoding" };
        public static HtmlPackageAttribute Frame { get; private set; } = new HtmlPackageAttribute() { Name = "frame" };
        public static HtmlPackageAttribute Implements { get; private set; } = new HtmlPackageAttribute() { Name = "implements" };
        public static HtmlPackageAttribute ImportBundle { get; private set; } = new HtmlPackageAttribute() { Name = "import-bundle" };
        public static HtmlPackageAttribute ImportScript { get; private set; } = new HtmlPackageAttribute() { Name = "import-script" };
        public static HtmlPackageAttribute ImportPresenterSet { get; private set; } = new HtmlPackageAttribute() { Name = "import-presenter-set" };
        public static HtmlPackageAttribute Index { get; private set; } = new HtmlPackageAttribute() { Name = "index" };
        public static HtmlPackageAttribute InitializeMethod { get; private set; } = new HtmlPackageAttribute() { Name = "initialize-method" };
        public static HtmlPackageAttribute InitializeParameter { get; private set; } = new HtmlPackageAttribute() { Name = "initialize-parameter" };
        public static HtmlPackageAttribute Interface { get; private set; } = new HtmlPackageAttribute() { Name = "interface" };
        public static HtmlPackageAttribute Handle { get; private set; } = new HtmlPackageAttribute() { Name = "handle" };
        public static HtmlPackageAttribute Handler { get; private set; } = new HtmlPackageAttribute() { Name = "handler" };
        public static HtmlPackageAttribute Method { get; private set; } = new HtmlPackageAttribute() { Name = "method" };
        public static HtmlPackageAttribute PackageAlias { get; private set; } = new HtmlPackageAttribute() { Name = "package-alias" };
        public static HtmlPackageAttribute PackageImport { get; private set; } = new HtmlPackageAttribute() { Name = "passthrough" };
        public static HtmlPackageAttribute Passthrough { get; private set; } = new HtmlPackageAttribute() { Name = "package-import" };
        public static HtmlPackageAttribute Parameter { get; private set; } = new HtmlPackageAttribute() { Name = "parameter" };
        public static HtmlPackageAttribute Presenter { get; private set; } = new HtmlPackageAttribute() { Name = "presenter" };
        public static HtmlPackageAttribute PresenterFrame { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-frame" };
        public static HtmlPackageAttribute PresenterNamespace { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-namespace" };
        //public static HtmlPackageAttribute PresenterViewWrapper { get; private set; } = new HtmlPackageAttribute() { Name = "presenter-view-wrapper" };
        public static HtmlPackageAttribute Proxy { get; private set; } = new HtmlPackageAttribute() { Name = "proxy" };
        public static HtmlPackageAttribute Publisher { get; private set; } = new HtmlPackageAttribute() { Name = "publisher" };
        public static HtmlPackageAttribute ScriptFrame { get; private set; } = new HtmlPackageAttribute() { Name = "script-frame" };
        public static HtmlPackageAttribute Redirect { get; private set; } = new HtmlPackageAttribute() { Name = "redirect" };
        public static HtmlPackageAttribute SetName { get; private set; } = new HtmlPackageAttribute() { Name = "set-name" };
        public static HtmlPackageAttribute StartParameter { get; private set; } = new HtmlPackageAttribute() { Name = "start-parameter" };
        public static HtmlPackageAttribute StyleScoped { get; private set; } = new HtmlPackageAttribute() { Name = "style-scoped" };
        public static HtmlPackageAttribute Subscribe { get; private set; } = new HtmlPackageAttribute() { Name = "subscribe" };
        public static HtmlPackageAttribute Subscriber { get; private set; } = new HtmlPackageAttribute() { Name = "subscriber" };
        public static HtmlPackageAttribute SystemSubscriber { get; private set; } = new HtmlPackageAttribute() { Name = "system-subscriber" };
        /// <summary>
        /// StyleTheme controls for colors and font
        /// </summary>
        //public static HtmlPackageAttribute StyleTheme{ get; private set; } = new HtmlPackageAttribute() { Name = "style-theme" };presenter-wrapper
        /// <summary>
        /// StyleStructure controls for font size, padding, margin, border, width, height, etcetera.
        /// </summary>
        //public static HtmlPackageAttribute StyleStructure { get; private set; } = new HtmlPackageAttribute() { Name = "style-structure" };
        public static HtmlPackageAttribute StyleCategoryKey { get; private set; } = new HtmlPackageAttribute() { Name = "style-category-key" };
        public static HtmlPackageAttribute StyleCategoryValue { get; private set; } = new HtmlPackageAttribute() { Name = "style-category-value" };
        public static HtmlPackageAttribute Template { get; private set; } = new HtmlPackageAttribute() { Name = "template" };
        public static HtmlPackageAttribute TemplateInstance { get; private set; } = new HtmlPackageAttribute() { Name = "template-instance" };
        public static HtmlPackageAttribute Templates { get; private set; } = new HtmlPackageAttribute() { Name = "templates" };
        public static HtmlPackageAttribute VersionKey { get; private set; } = new HtmlPackageAttribute() { Name = "version" };
        //public static HtmlPackageAttribute Wrapper { get; private set; } = new HtmlPackageAttribute() { Name = "wrapper" };
        public static HtmlPackageAttribute DomeWrapper { get; private set; } = new HtmlPackageAttribute() { Name = "dome-wrapper", Description = "Document Object Model Element Wrapper: Replaces the DOM element with a proxy." };
        public static HtmlPackageAttribute StaticInterface { get; private set; } = new HtmlPackageAttribute() { Name = "static-interface" };

        public static HtmlPackageAttribute Graphic { get; private set; } = new HtmlPackageAttribute() { Name = "graphic" };

    }
}
