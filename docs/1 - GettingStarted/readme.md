<a id="top"></a> 

![Image](Hularion.png)
#### Hularion - *Software with a Strategy*
#### Hularion Experience (HX) - A modular client application framework for web and desktop using HTML, CSS, and JS.

&nbsp;

# Hularion Experience - Getting Started

&nbsp;


## Requirements

#### Hularion Developer - https://github.com/JohnathanADrews/HularionDeveloper

Please use the latest release. Alternatively, you can run it from your IDE by setting the HularionDeveloper project as the startup project.

&nbsp;

## Contents 

1. [Introduction](#Introduction)
1. [Creating the Directories and Files](#CreatingDirectoriesFiles)
1. [Edit the Project File](#EditProjectFile)
1. [Edit the Application File](#EditApplicationFile)
1. [Edit the Presenter File](#EditPresenterFile)
1. [Run the Project](#RunProject)

&nbsp;
<a id="Introduction"></a>
## Introduction 

###### goto &rarr; [(next)](#CreatingDirectoriesFiles) - [(top)](#top)

In this article, we will discuss how to create the folder structure and necessary files to create a working HX project. In later articles, we will expand on the capabilities of an HX project and how to use it to create an HX package. An HX package could contain applications or it could be used as a module in other projects.

To run HX projects, we will use the Hularion Developer application, which is able to run multiple projects at once, and it can also build packages from projects. You can run the latest release or run it from your IDE. Use the following link to access the repository.

https://github.com/JohnathanADrews/HularionDeveloper


&nbsp;
<a id="CreatingDirectoriesFiles"></a>
## Creating the Directories and Files 

###### goto &rarr; [(prev)](#Introduction) - [(next)](#EditProjectFile) - [(top)](#top)

1. Create a new folder. In this example we are using the MyProject folder.
2. In the new folder, create a file called MyProject.hxproject. This is the main project configuration file.
3. Create a sub-folder called Applications. This is the folder that will contain all of the configurations for applications declared within the project.
4. In the Applications folder, create a file called MyApp.html. This contains the configuration details for an application. A project may have zero or more applications.
5. Create a sub-folder called PresenterSets. This folder contains sub-folders, each having its own set of presenters, which are the UI unit.
6. In the PresenterSets folder, create a folder called MyProjectPresenters. This is a name we choose. This name is important and will be used by configuration files.
7. In the MyProjectPresenters folder, create a file called MyAppEntryPoint.html. This will be used as the entry point for the MyApp application.

It should end up looking like this  
![Image](GettingStartedFiles.png)
 

&nbsp;
<a id="EditProjectFile"></a>
## Edit the Project File, MyProject.hxproject

###### goto &rarr; [(prev)](#CreatingDirectoriesFiles) - [(next)](#EditApplicationFile) - [(top)](#top)

Open MyProject.hxproject in a text editor. The project file, like most configuration files in HX, is an HTML fragment. When HX builds a package from a project, it uses HTML attributes to decide the build for both configuration files and presenters. Using HTML also enables seamless integration of CSS and javascript into the configuration. In the MyProject.hxproject file, add the following code.

```
<hx h-hxpackage="true"
    h-package-name="MyPackage"
    h-version="1.0.0"
    h-package-key="MyPackage">

</hx>
```


&nbsp;
<a id="EditApplicationFile"></a>
## Edit the Application File, MyApp.html 

###### goto &rarr; [(prev)](#EditProjectFile) - [(next)](#EditPresenterFile) - [(top)](#top)

Open MyApp.html in a text editor. Like the project file, it is also an HTML fragment. Add the following code.

```
<hx h-application="true"
    h-application-key="MyApp"
    h-application-name="My Application"
    h-presenter-set="MyAppPresenters"
    h-application-presenter="MyAppEntryPoint"
    h-application-is-default="true">
</hx>
```
This file lets HX know that there is an application, MyApp, and where to find the entry point. Note that both the presenter set MyAppPresenters, and the presenter MyAppEntryPoint are required.



&nbsp;
<a id="EditPresenterFile"></a>
## Edit the Presenter File, MyAppEntryPoint.html. 

###### goto &rarr; [(prev)](#EditApplicationFile) - [(next)](#RunProject) - [(top)](#top)

Open MyAppEntryPoint.html in a text editor. Like the project file, it is also an HTML fragment. Rather than configuration information, this file contains the presenter's functionality.

```
<h1>My App</h1>

<script>

    function MyAppEntryPoint() {
    }

    MyAppEntryPoint.prototype = {

        start: function (parameters) {            
            console.log("MyAppEntryPoint.start - ", this, window);
        }
    }

</script>
```
Here we have a presenter that displays "My App" in an h1 tag. Notice the script tag and the javascript constructor function. The function must have the same name as the presenter, and it is required for the presenter to run. Also, there is a "start" method on the constructor function. This method is automatically called once the HX framework creates an instance of the presenter and performs the initialization.



&nbsp;
<a id="RunProject"></a>
## Run the Project 

###### goto &rarr; [(prev)](#EditPresenterFile) - [(next)](#End) - [(top)](#top)

1. Go to the HularionDeveloper repository and download the latest release. https://github.com/JohnathanADrews/HularionDeveloper.
2. Extract the files and run HularionDeveloper.exe. Creating a shortcut may be helpful.
3. When you run it, two windows will pop up. One is the application window, and the other is the web development tools. The first time you run it, you will need to create a context name (like a username) and agree to the disclaimer. This is only necessary the first time. If you look in the HularionDeveloper.exe folder, a .hx folder will appear. There is a database file that stored some basic information such as your context name, which packages are installed, and which applications you have selected. The .hx folder is also where any HX packages you install are kept.
4. In the menu, go to Package->View Sources. This will show you the package sources you have added, and it will also allow you to add new sources.
![Image](ViewSources.png)
5. Click the Add Source button.
![Image](ClickAddSource.png)
6. Enter the name of the source, which can be anything. The location is the directory of the project. The, click the Create button.
![Image](CreateSource.png)
7. Go to Package->View Packages
![Image](ViewPackages.png)
8. Add the package by clicking Add within the MyPackage box. Note that projects have an association with the color green, while projects have an association with the color blue.
![Image](AddPackage.png)
9. View your applications by clicking Apps->My Apps.
![alt text](MyApps.png)
10. In the applications view, there is now a line item for the application. Click the run button to play the application in a new tab.
![alt text](ViewApps.png)
11. Look at the application in the new tab.
![alt text](RunMyApp.png)


&nbsp;
<a id="End"></a>
### The End 
Please take a look a this [EndState](EndState) folder for the final project.
###### goto &rarr; [(prev)](#RunProject) - [(top)](#top)