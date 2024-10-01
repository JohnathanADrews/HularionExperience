<a id="top"></a> 

![Image](Hularion.png)
#### Hularion - *Software with a Strategy*
#### Hularion Experience (HX) - A modular client application framework for web and desktop using HTML, CSS, and JS.

&nbsp;

# Hularion Experience - Compendium of Attributes

-- **Work in Progress** --

&nbsp;
<a id="Introduction"></a>
## Introduction

###### goto &rarr; [(next)](#Setup) - [(top)](#top)

At a high level, Hularion Experience (HX) relies on the the package builder, the package managers, and the HX runtime kernel. The package builder takes an HX project and emits an HX package according to the project specifications. The packages managers are responsible for locating and storing packages. Finally, the HX kernel runs the packages within the browser.

The package builder mainly uses HTML attributes to decide how to build a package. That is, they are a primary component of the specification, and they are the focus of this document. Here we will exhaustively list the supported attributes and, where able, reference documentation on their usage.


Note: Attributes and their usage can be found in HtmlResourceAttribute.cs, ResourcesToProjectTransform.cs, and PresenterTransform.cs within the HularionExperience C# project. Some of the attributes found there are no longer supported.

&nbsp;
<a id="Definitions"></a>
## Definitions

###### goto &rarr; [(prev)](#Introduction) - [(next)](#AddingLabelClone) - [(top)](#top)



<a id="ApplicationConfiguration"></a>
**Application Configuration**: The configuration information for an application within an HX project. An HTML file within the Applications folder that declares an application and indicates a starting presenter set and presenter..

<a id="OptionAttribute"></a>
**Option Attribute**: An attribute that is applicable in the context of a [primary attribute](#PrimaryAttribute).

<a id="DirectiveAttribute"></a>
**Directive Attribute**: An attribute that results in the element being removed from the presenter's template during package build.

<a id="IndependentAttribute"></a>
**Independent Attribute**: An attribute that can be applied regardless of the context to a regular HTML element or one with a [primary attribute](#PrimaryAttribute).

<a id="PartnerAttribute"></a>
**Partner Attribute**: An attribute that is required alongside another attribute.

<a id="ProjectConfiguration"></a>
**Project Configuration**: The configuration information for a project. An HTML file ending with ".hxproject".

<a id="PresenterFile"></a>
**Presenter File**: The HTML project file containing a presenter specification.

<a id="PresenterInstance"></a>
**Presenter Instance**: The instance of a presenter as see internally from the presenter.

<a id="PresenterObject"></a>
**Presenter Object**: The object that is returned to the caller when a presenter instance is created, usually when the h-presenter tag is used, the h-component tag is used, or when a presenter is created in javascript. It has four members: proxy, which contains the public methods and accessors; publisher, which handles pub/sub communications; dome, which is the DOM element; and view which is the wrapped DOM element or the DOM element if there is no wrapper.

<a id="PrimaryAttribute"></a>
**Primary Attribute**: An attrtibute that alone specifies the way in which the element should be handled during package build. Since the elements are in the context of HTML, the primary attribute serves as a stand-in for the element type. Without a primary attribute, the element is treated as a normal HTML element.

<a id="ProxyObject"></a>
**Proxy Object**: The member of the [presenter object](#PresenterObject) that contains methods and accessors available on the [presenter object](#PresenterObject).

<a id="Publisher"></a>
**Publisher**: The member of the [presenter object](#PresenterObject) that handles pub/sub communications. It contains members as declared by the presenter with subscribe and/or publish methods.

<a id="SetConfiguration"></a>
**Set Configuration**: The configuration information for a set within an HX project. An HTML file within the Configuration folder.

<a id="ReplacementAttribute"></a>
**Replacement Attribute**: An attribute that results in the element being replaced by a stub during package build, and later replaced by a DOM node duing construction.

<a id="View"></a>
**View**: The member of the [presenter object](#PresenterObject) that is the wrapped DOM element or the DOM element if there is no wrapper.




&nbsp;
<a id="CompleteList"></a>
## Complete List


[h-alias](#h-alias) - [Project](#ProjectConfiguration)   
[h-application](#h-application) - [Application](#ApplicationConfiguration)  
[h-application-is-default](#h-application-is-default) - [Application](#ApplicationConfiguration)  
[h-application-key](#h-application-key) - [Application](#ApplicationConfiguration)  
[h-application-name](#h-application-name) - [Application](#ApplicationConfiguration)  
[h-application-presenter](#h-application-presenter) - [Application](#ApplicationConfiguration)  
[h-application-set](#h-application-set) - [Application](#ApplicationConfiguration)     
[h-assign](#h-assign) - [Set](#SetConfiguration)  
[h-attach](#h-attach) - [Set](#SetConfiguration)  
[h-brand-name](#h-brand-name) - [Project](#ProjectConfiguration)   
[h-clone](#h-clone) - [Presenter](#PresenterFile)   
[h-clone-instance](#h-clone-instance) - [Presenter](#PresenterFile)    
[h-component](#h-component) - [Presenter](#PresenterFile)    
[h-component-handler](#h-component-handler) - [Presenter](#PresenterFile)   
[h-contributor](#h-contributor) - [Project](#ProjectConfiguration)    
[h-description](#h-description) - [Project](#ProjectConfiguration)     
[h-dome-wrapper](#h-dome-wrapper) - [Project](#ProjectConfiguration)     
[h-frame](#h-frame) - [Project](#ProjectConfiguration) , [Set](#SetConfiguration)     
[h-graphic](#h-graphic) - [Project](#ProjectConfiguration)       
[h-handle](#h-handle) - [Presenter](#PresenterFile) , [Set](#SetConfiguration)     
[h-hxpackage](#h-hxpackage) - [Project](#ProjectConfiguration)  
[h-import-presenter](#h-import-presenter) - [Set](#SetConfiguration)    
[h-import-script](#h-import-script) - [Project](#ProjectConfiguration) , [Set](#SetConfiguration)    
[h-import-set](#h-import-set) - [Project](#ProjectConfiguration) , [Set](#SetConfiguration)        
[h-license](#h-license) - [Project](#ProjectConfiguration) 
[h-license-must-agree](#h-license-must-agree) - [Project](#ProjectConfiguration) 
[h-link](#h-link) - [Project](#ProjectConfiguration)         
[h-package](#h-package) - [Project](#ProjectConfiguration)   
[h-package-key](#h-package-key) - [Project](#ProjectConfiguration)   
[h-package-name](#h-package-name) - [Project](#ProjectConfiguration)    
[h-package-import](#h-package-import) - [Project](#ProjectConfiguration)     
[h-presenter-configuration](#h-presenter-configuration) - [Set](#SetConfiguration)   
[h-presenter-frame](#h-presenter-frame) - [Set](#SetConfiguration)   
[h-presenter-set](#h-presenter-set) - [Set](#SetConfiguration)      
[h-product-key](#h-product-key) - [Project](#ProjectConfiguration)      
[h-project](#h-project) - [Project](#ProjectConfiguration)      
[h-proxy](#h-proxy) - [Presenter](#PresenterFile)    
[h-publisher](#h-publisher) - [Presenter](#PresenterFile)   
[h-role](#h-role) - [Project](#ProjectConfiguration)   
[h-script-frame](#h-script-frame) - [Project](#ProjectConfiguration) , [Set](#SetConfiguration)  
[h-server-router](#h-server-router) - [Project](#ProjectConfiguration)
[h-start-parameter](#h-start-parameter) - [Presenter](#PresenterFile)    
[h-template](#h-template) - [Presenter](#PresenterFile)    
[h-template-instance](#h-template-instance) - [Presenter](#PresenterFile)        
[h-version](#h-version) - [Project](#ProjectConfiguration)         



&nbsp;
<a id="AttributeList"></a>
## Attributes

###### goto &rarr; [(prev)](#Introduction) - [(next)](#AddingLabelClone) - [(top)](#top)



&nbsp;
<!--                    h-alias                 -->
<a id="h-alias"></a>
* ### h-alias  
Description
```
``` 

&nbsp;
<!--                    h-application-is-default                 -->
<a id="h-application-is-default"></a>
* ### h-application-is-default  
Description
```
``` 

&nbsp;
<!--                    h-application-key                 -->
<a id="h-application-key"></a>
* ### h-application-key 
Adds a key to an application file, which uniquely identifies the application within the package. This is required for a an application to run.
```
<hx
    ...
    h-application-key="<application_key>"
>
 ...
</hx>
``` 

&nbsp;
<!--                    h-application-name                 -->
<a id="h-application-name"></a>
* ### h-application-name  
Adds a name to an application file, which publicly identifies the application and should be unique. This is required for a user to properly recognize the application.
```
<hx
    ...
    h-application-name="<application_name>"
>
 ...
</hx>
``` 
``` 

&nbsp;
<!--                    h-application-presenter                 -->
<a id="h-application-presenter"></a>
* ### h-application-presenter  
Declares the presenter that will be used as the entry point into the application. 
```
<hx
    ...
    h-application-presenter="<presenter_name>"
>
 ...
</hx>
``` 

&nbsp;
<!--                    h-application-set                 -->
<a id="h-application-set"></a>
* ### h-application-set 
Declares the presenter set containing the presenter that will be used as the entry point into the application. 
```
<hx
    ...
    h-application-set="<presenter_set_name>"
>
 ...
</hx>

&nbsp;
<!--                    h-assign                 -->
<a id="h-assign"></a>
* ### h-assign  
Description
```
``` 

&nbsp;
<!--                    h-attach                 -->
<a id="h-attach"></a>
* ### h-attach 
Attaches an object to either the caller frame's global window using the "frame" value, or to each presenter instance if the caller is a presenter frame and the "inject" values was used.
See [h-presenter-frame](#h-presenter-frame), [h-script-frame](#h-script-frame).

```


``` 

&nbsp;
<!--                    h-brand-name                 -->
<a id="h-brand-name"></a>
* ### h-brand-name  
Adds a brand name to a .hxproject project file that will be included in the package. This will help the package be visually identifiable.
```
<hx
    ...
    h-brand-name="<product_brand_name>"
>
 ...
</hx>
``` 

&nbsp;
<!--                    h-clone                 -->
<a id="h-clone"></a>
* ### h-clone  
Indicates that the tag and its contents, the fragment, shall be removed from the view. This fragment can then be used to create individual instances in the [instance](#PresenterInstance) or using a [h-clone-instance](#h-clone-instance).
```
<div h-clone="<clone_name>">
    <div>
        <label h-handle="<example.label.handle>" />
    </div>
</div>
```
For example, in <clone_name>.start()
```

this.hularion.createClone("<clone_name>");

```
For example, in the view:
```
<hx h-template-instance="<clone_name>" h-handle="<my.clone.handle>" />
```



&nbsp;
<!--                    h-clone-instance                 -->
<a id="h-clone-instance"></a>
* ### h-clone-instance  
Declared within a [presenter](#PresenterFile) to create an instance of a clone, inserting it into the view at the desired location. A handle can be added to access it within the [presenter instance](#PresenterInstance);
```
<hx h-template-instance="<clone_name>" h-handle="<my.clone.handle>" />
``` 


&nbsp;
<!--                    h-component                 -->
<a id="h-component"></a>
* ### h-component  
Creates a [presenter object](#PresenterObject) and passes it as a parameter to the containing presenter instance. 
 
```
h-component="<presenter-name>=><component-handler-name>"
```
```
h-component="<frame-name>\<presenter-name>=><component-handler-name>"
``` 

&nbsp;
<!--                    h-component-handler                 -->
<a id="h-component-handler"></a>
* ### h-component-handler  
Maps a named component handler to a method on the [presenter instance](#PresenterInstance). Typically, the handler name will start with an uppercase letter.

Must be accompanied by the h-method attribute to map to the method.
```
<hx h-component-handler="<HandlerName>" h-method="<methodName>" />
``` 

&nbsp;
<!--                    h-contributor                 -->
<a id="h-contributor"></a>
* ### h-contributor  
Adds a contributor to the .hxproject project file. Zero or more links can be added. These are individuals who contributed to the project.
```
<hx ...
>
    ...
    
    <hx h-contributor="<contributor_name>" h-role="<contributor_role>" />
    <hx h-contributor="<contributor_name>" h-role="<contributor_role>" />
    <hx h-contributor="<contributor_name>" h-role="<contributor_role>" />

    ...
</hx>
``` 

&nbsp;
<!--                    h-description                 -->
<a id="h-description"></a>
* ### h-description  
Adds a description to a .hxproject project file that will be included in the package.
```
<hx
    ...
    h-description="This is my package's description"
>
 ...
</hx>
``` 

&nbsp;
<!--                    h-dome-wrapper                 -->
<a id="h-dome-wrapper"></a>
* ### h-dome-wrapper  
Adds a DOM element wrapper to the view elements of the package. Setup in the .hxproject project file. In the following code, we are importing the package containing the wrapper and assigning it an alias. Then, we are creating a script frame for the script set containing the wrapper. Finally, we are defining a wrapper function that uses myWrapperFunc to wrap the element. 
```
<hx ...
>
    <hx h-package-import="<package_name>@<package_version>=><package_alias>" />
    <hx h-import-script="<package_alias>" h-import-set="<set_name>" h-frame="<frame_name>" />
    <hx h-script-frame="<frame_name>" h-dome-wrapper="domElement=><my_wrapper_func>(dom_element);" />
    
    <!-- for example,
        <hx h-script-frame="<frame_name>" h-dome-wrapper="e=>$(e);" />
     -->	
</hx>
``` 



&nbsp;
<!--                    h-frame                 -->
<a id="h-frame"></a>
* ### h-frame  
Assigns a name to a frame. In the following code, an iframe is created containing the code from the setName set within the packageAlias package. The h-frame attribute essentially creates an alias for the frame so that the frame can be referenced elsewhere.
```
    <hx h-import-script="<packageAlias>" h-import-set="<setName>" h-frame="<frameName>" />
``` 


&nbsp;
<!--                    h-graphic                 -->
<a id="h-graphic"></a>
* ### h-graphic  
Description
```
``` 


&nbsp;
<!--                    h-handle                 -->
<a id="h-handle"></a>
* ### h-handle   
Assigns an object resource to the [presenter instance](#PresenterInstance) using the provided fully qualified path. If the handle is assigned to a regular HTML tag, then the resource will be the [view](#View). If the handle is assigned to an h-presenter or h-component tag, then it will refer to the corresponding [presenter object](#PresenterObject). If the handle is assigned to an h-clone-instance or h-template-instance, then it will be assigned to the clone instance or template instance, respectively.
 
```
h-handle="<period-delimited-handle-path>"
```
 


&nbsp;
<!--                    h-hxpackage                 -->
<a id="h-hxpackage"></a>
* ### h-hxpackage  
Description
```
``` 


&nbsp;
<!--                    h-import-presenter                 -->
<a id="h-import-presenter"></a>
* ### h-import-presenter  
Creates an iframe using the indicated set from the indicated package. <frame_name> can the be used in [presenters](#PresenterFile) or to attach references (see [h-presenter-frame](#h-presenter-frame)).
```
<hx h-import-presenter="<package_alias>" h-import-set="<set_name>" h-frame="<frame_name>" />
``` 


&nbsp;
<!--                    h-import-script                 -->
<a id="h-import-script"></a>
* ### h-import-script  
Creates an iframe using the indicated set from the indicated package. <frame_name> can the be used in [presenters](#PresenterFile) or to attach references (see [h-script-frame](#h-script-frame)).
<hx h-import-script="<package_alias>" h-import-set="<set_name>" h-frame="<frame_name>" />
``` 


&nbsp;
<!--                    h-import-set                 -->
<a id="h-import-set"></a>
* ### h-import-set  
Description
```
``` 


&nbsp;
<!--                    h-license                 -->
<a id="h-license"></a>
* ### h-license  
Adds a license agreement to a .hxproject project file that will be included in the package. If the h-license-must-agree="true" attribute is included and is "true", then a package installer must have the user agree to the license terms prior to installation.
```
<hx ...
>
    ...
    <hx h-license h-license-must-agree="true">
        This is the license agreement for this package.
    </hx>
    ...
</hx>
``` 

&nbsp;
<!--                    h-license                 -->
<a id="h-license-must-agree"></a>
* ### h-license-must-agree  
An option for [h-license](#h-license).


&nbsp;
<!--                    h-link                 -->
<a id="h-link"></a>
* ### h-link  
Adds a link to the .hxproject project file. This could be a website, repository, social media, etcetera. Zero or more links can be added.
```
<hx ...
>
    ...
    
    <hx h-link="<url>" h-name="<link_name>" h-description="<link_description>"  />
    <hx h-link="<url>" h-name="<link_name>" h-description="<link_description>"  />
    <hx h-link="<url>" h-name="<link_name>" h-description="<link_description>"  />

    ...
</hx>
``` 

&nbsp;
<!--                    h-package                 -->
<a id="h-package"></a>
* ### h-package  
The primary attribute of a .hxproject project file, indicating that the contained tags are related to a Hularion Experience project. This attribute also requires [h-package-key](#h-package-key), [h-package-version](#h-package-version), and [h-package-name](#h-package-name).
```
<hx h-hxpackage="true"
    h-package-key="<package_key>"
    h-version="<package_version>"
    h-package-name="<package_name>"
    ...
>

<!-- Additional Configuration -->



</hx>
``` 

&nbsp;
<!--                    h-package-key                 -->
<a id="h-package-key"></a>
* ### h-package-key  
Indicates the unique key of a package. See [h-package](#h-package).


&nbsp;
<!--                    h-package-name                 -->
<a id="h-package-name"></a>
* ### h-package-name  
Indicates the unique name of a package. See [h-package](#h-package).


&nbsp;
<!--                    h-package-import                 -->
<a id="h-package-import"></a>
* ### h-package-import  
In a .hxproject project file, adds a reference to another "imported" package, enabling the use of the imported package. The package key and package version are both required to locate the package, and the package alias is required for configuration files to link to the package.  There are two ways to use the tag.

Option 1: Use option tags for version and alias.
```
<hx h-package-import="<package_key>" h-version="<package_version>" h-alias="<pacakge_alias>"/>
``` 

Option 2: Use a condensed form.
```
<hx h-package-import="<package_key>@<package_version>=><package_alias>"/>
```

Also, there is an option attribute for project references. In such a case, the version is ignored and the framework will locate the pre-built project.
```

<!-- Absolute Path -->

<hx h-package-import="<package_key>@<package_version>=><package_alias>" h-project="<absolute_project_path>" />

<!-- Relative Path -->

<hx h-package-import="<package_key>@<package_version>=><package_alias>" h-project="..\<relative_project_path>" />
``` 


&nbsp;
<!--                    h-presenter-configuration                 -->
<a id="h-presenter-configuration"></a>
* ### h-presenter-configuration  

[partner](#PartnerAttribute) of  [h-presenter-set](#h-presenter-set)

Indicates that the tags contained within are related to a presenter set configuration. Used together with a h-presenter-set attribute, which indicates the presenter set being configured.
```
<hx h-presenter-configuration="<configuration_key>" h-presenter-set="<presenter_set_name>">
    ...
</hx>
``` 


&nbsp;
<!--                    h-presenter-frame                 -->
<a id="h-presenter-frame"></a>
* ### h-presenter-frame  
Indicates that a presenter frame reference should be used for some purpose. The frame name is generally defined in the preceeding line. Options include [h-attach](#h-attach) and [h-handle](#h-handle). Related to [h-import-presenter](#h-import-presenter).
```
<hx h-import-presenter="<package_alias>" h-import-set="<set_name>" h-frame="<frame_name>" />
<hx h-presenter-frame="<frame_name>" h-attach="<frame | inject>" h-handle="<frame.reference.path>" />
``` 


&nbsp;
<!--                    h-presenter-set                 -->
<a id="h-presenter-set"></a>
* ### h-presenter-set  

[partner](#PartnerAttribute) of [h-presenter-configuration](#h-presenter-configuration): Indicates which presenter set is being configured.


&nbsp;
<!--                    h-product-key                 -->
<a id="h-product-key"></a>
* ### h-product-key  
Adds a product key a .hxproject project file that will be included in the package.
```
<hx
    ...
    h-product-key ="<product_key>"
>
 ...
</hx>
``` 


&nbsp;
<!--                    h-project                 -->
<a id="h-project"></a>
* ### h-project  
Within the context of a pacakge import, h-project indicates that a pre-built project should be referenced rather than a package. See [h-package-import](#h-package-import).


&nbsp;
<!--                    h-proxy                 -->

<a id="h-proxy"></a>

* ### h-proxy
Assigns a method on the [proxy](#ProxyObject) having the name of the method on the presenter's constructor function. [proxy](#ProxyObject) method call in turn calls the method on the constructor function.

```
h-proxy="<public-method>"
```
The [proxy](#ProxyObject) method and the constructor function method have the same name.

 
```
h-proxy="<public-method>=><private-method>"
```
The [proxy](#ProxyObject) method and the constructor function method have different names.

[(complete list)](#CompleteList)

&nbsp;
<!--                    h-publisher                 -->
<a id="h-publisher"></a>
* ### h-publisher  
Assigns a "subscribe" method on [proxy](#ProxyObject). Assigns a "publish" method on this.hularion.&lt;publisher-name&gt;.
 
```
h-publisher="<publisher-name>"
```

&nbsp;
<!--                    h-role                 -->
<a id="h-role"></a>
* ### h-role  
An option for [h-contributor](#h-contributor) indicating the role of the contributor in the project.


&nbsp;
<!--                    h-script-frame                  -->
<a id="h-script-frame "></a>
* ### h-script-frame   
Indicates that a script frame reference should be used for some purpose. The frame name is generally defined in the preceeding line. Options include [h-attach](#h-attach), [h-handle](#h-handle), and  [h-assign](#h-assign). Related to [h-import-script](#h-import-script).
```
<hx h-import-script="<package_alias>" h-import-set="<set_name>" h-frame="<frame_name>" />
<hx h-script-frame="<frame_name>" h-attach="<frame | inject>" h-handle="<frame.reference.path>" h-assign="<javascript_returning_value>" />
``` 


&nbsp;
<!--                    h-server-router                 -->
<a id="h-server-router"></a>
* ### h-server-router 
Description
```
``` 


&nbsp;
<!--                    h-start-parameter                 -->
<a id="h-start-parameter"></a>
* ### h-start-parameter  
An [option](#OptionAttribute) for [h-component](#h-component), indicating parameters that should be given to the component instance on start().
```
<hx h-component="<frame_name>/<presenter_name>" h-start-parameter='{"name": "Item Name"}'
``` 


&nbsp;
<!--                    h-Presenter                 -->
<a id="h-presenter"></a>
* ### h-presenter  
Declared within a [presenter](#PresenterFile) to create an instance of a presenter, inserting it into the view at the desired location.  A handle can be added to access it within the [presenter instance](#PresenterInstance);  

If the referenced presenter is defined in the same presenter set, then only the presenter name is required.
```
<hx h-presenter="<presenter_name>" h-handle="<my.presenter.handle>" />
``` 

If the presenter is defined within another set (in any package), then a frame must be created in the configuration and the frame name must precede the presenter name, separated by a forward slash.

```
<hx h-presenter="<frame_name>/<presenter_name>" h-handle="<my.presenter.handle>" />
``` 



&nbsp;
<!--                    h-template                 -->
<a id="h-template"></a>
* ### h-template  
Indicates that the tag and its contents, the fragment, shall be removed from the view. This fragment can then be used to create individual instances in the [instance](#PresenterInstance) or using a [h-template-instance](#h-template-instance). A template can contain [h-presenter](#h-presenter), [h-clone-instance](#h-clone-instance), [h-template-instance](#h-template-instance), and [h-component](#h-component) tags.
```
<div h-template="<template_name>">
    <div>
        <hx h-presenter="<frame_name>/<presenter_name>" h-handle="<example.presenter.handle>" />
    </div>
    <hx h-template_instance="<template_name>" h-handle="<example.template.handle>" />
    <hx h-clone_instance="<clone_name>" h-handle="<example.clone.handle>" />
    <hx h-presenter="<frame_name>/<presenter_name>" h-handle="<example.presenter.handle>">
        <hx h-component="<frame_name>/<presenter_name>">
            <hx h-component="<frame_name>/<presenter_name>">
            </hx>
        </hx>        
        <hx h-component="<frame_name>/<presenter_name>">
        </hx>
    </hx>

    
</div>
```
For example, in <presenter_name>.start()
```

this.hularion.createTemplate("<template_name>");

```
For example, in the view:
```
<hx h-template-instance="<template_name>" h-handle="<my.template.handle>" />
```


&nbsp;
<!--                    h-template-instance                 -->
<a id="h-template-instance"></a>
* ### h-template-instance  
Declared within a [presenter](#PresenterFile) to create an instance of a template, inserting it into the view at the desired location. A handle can be added to access it within the [presenter instance](#PresenterInstance);
```
<hx h-template-instance="<template_name>" h-handle="<my.template.handle>" />
``` 


&nbsp;
<!--                    h-version                 -->
<a id="h-version"></a>
* ### h-version  
Adds a version to a .hxproject project file that will be included in the package. This is required for a package to be successfully located by the framework.
```
<hx
    ...
    h-version="1.2.3"
>
 ...
</hx>
``` 

 
 

