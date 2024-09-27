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

<a id="CompanionAttribute"></a>
**Companion Attribute**: An attribute that is applicable in the context of a [primary attribute](#PrimaryAttribute).

<a id="DirectiveAttribute"></a>
**Directive Attribute**: An attribute that results in the element being removed from the presenter's template during package build.

<a id="IndependentAttribute"></a>
**Independent Attribute**: An attribute that can be applied regardless of the context to a regular HTML element or one with a [primary attribute](#PrimaryAttribute).

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
[h-script-frame](#h-script-frame) - [Project](#ProjectConfiguration) , [Set](#SetConfiguration)   
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
Description
```
``` 

&nbsp;
<!--                    h-application-name                 -->
<a id="h-application-name"></a>
* ### h-application-name  
Description
```
``` 

&nbsp;
<!--                    h-application-presenter                 -->
<a id="h-application-presenter"></a>
* ### h-application-presenter  
Description
```
``` 

&nbsp;
<!--                    h-application-set                 -->
<a id="h-application-set"></a>
* ### h-application  
Description
```
``` 

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
Description
```
``` 

&nbsp;
<!--                    h-brand-name                 -->
<a id="h-brand-name"></a>
* ### h-brand-name  
Description
```
``` 

&nbsp;
<!--                    h-clone                 -->
<a id="h-clone"></a>
* ### h-clone  
Description
```
``` 

&nbsp;
<!--                    h-clone-instance                 -->
<a id="h-clone-instance"></a>
* ### h-clone-instance  
Description
```
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
Description
```
``` 

&nbsp;
<!--                    h-description                 -->
<a id="h-description"></a>
* ### h-description  
Description
```
``` 

&nbsp;
<!--                    h-dome-wrapper                 -->
<a id="h-dome-wrapper"></a>
* ### h-dome-wrapper  
Description
```
``` 

&nbsp;
<!--                    h-frame                 -->
<a id="h-frame"></a>
* ### h-frame  
Description
```
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
Description
```
``` 


&nbsp;
<!--                    h-import-script                 -->
<a id="h-import-script"></a>
* ### h-import-script  
Description
```
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
Description
```
``` 


&nbsp;
<!--                    h-package-key                 -->
<a id="h-package-key"></a>
* ### h-package-key  
Description
```
``` 


&nbsp;
<!--                    h-package-name                 -->
<a id="h-package-name"></a>
* ### h-package-name  
Description
```
``` 


&nbsp;
<!--                    h-package-import                 -->
<a id="h-package-import"></a>
* ### h-package-import  
Description
```
``` 


&nbsp;
<!--                    h-presenter-configuration                 -->
<a id="h-presenter-configuration"></a>
* ### h-presenter-configuration  
Description
```
``` 


&nbsp;
<!--                    h-presenter-frame                 -->
<a id="h-presenter-frame"></a>
* ### h-presenter-frame  
Description
```
``` 


&nbsp;
<!--                    h-presenter-set                 -->
<a id="h-presenter-set"></a>
* ### h-presenter-set  
Description
```
``` 


&nbsp;
<!--                    h-product-key                 -->
<a id="h-product-key"></a>
* ### h-product-key  
Description
```
``` 


&nbsp;
<!--                    h-project                 -->
<a id="h-project"></a>
* ### h-project  
Description
```
``` 


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
<!--                    h-script-frame                  -->
<a id="h-script-frame "></a>
* ### h-script-frame   
Description
```
``` 


&nbsp;
<!--                    h-start-parameter                 -->
<a id="h-start-parameter"></a>
* ### h-start-parameter  
Description
```
``` 


&nbsp;
<!--                    h-Presenter                 -->
<a id="h-Presenter"></a>
* ### h-Presenter  
Description
```
``` 


&nbsp;
<!--                    h-template-instance                 -->
<a id="h-template-instance"></a>
* ### h-template-instance  
Description
```
``` 


&nbsp;
<!--                    h-version                 -->
<a id="h-version"></a>
* ### h-version  
Description
```
``` 

 
 

