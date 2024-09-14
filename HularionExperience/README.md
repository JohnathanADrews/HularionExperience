
![Image](https://github.com/JohnathanADrews/Hularion/blob/main/Hularion%20image.png?raw=true)

# Hularion - *Software with a Strategy*

##### Hularion TM &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Software with a Strategy TM

&nbsp;

## Using Hularion Experience

#### Hularion Experience (HX) is an application framework that runs applications using web-based user interface code, i.e. HTML/CSS/Javascript. HX applications exist in packages, and each package can have zero or more applications, as some packages contain reusable modules. Packages are developed in a project folder, and HX can then build that project into a package. HX can also reload applications during development, enabling faster development times. See the Hularion Developer application for more details. It is an application that can run projects and packages in a tabbed environment, and it can build packages for releases.

#### On the front-end, "presenters" act as the user interface unit. They are HTML files that contain attribute-based directives, the HTML for the presenter template, a javascript constructor function that acts as a controller, and some CSS that is automatically scoped to the presenter. When a presenter is created, the caller receives a proxy object containing the public methods and accessors, pub/sub event handlers, the DOM element, and an optionally wrapped DOM element. Presenters can use attribute-driven directives to define accessors, public methods, publishers, and many more things. Presenters can also use attributes to declare a handle on an HTML element, thus removing the need for DOM queries.

#### On the back-end, there is a routing system that enables communications through well-defined routes. These routes can be automatically masked by method names so that the presenters do not need to keep track of them.


 