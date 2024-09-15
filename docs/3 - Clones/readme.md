
![Image](https://github.com/JohnathanADrews/Hularion/blob/main/Hularion%20image.png?raw=true)

# Hularion - *Software with a Strategy*

##### Hularion TM &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Software with a Strategy TM

&nbsp;

If you are unfamiliar with Hularion Experience, please take a look at the first two documents, Getting Started and Button. The concepts in those two are necessary for any further study. They are available using the following links.

https://github.com/JohnathanADrews/HularionExperience/tree/main/docs/1%20-%20GettingStarted/readme.md

https://github.com/JohnathanADrews/HularionExperience/tree/main/docs/2%20-%20Button/readme.md

## Hularion Experience - Clones


In Hularion Experience, there are two types of templating mechanisms, clones and templates. Clones are lightweight copies of HTML fragments. Templates, on the other hand, can copy presenter references, clone instances, and even other template instance. In this document, we will discuss clones.

We will use a clean version the MyAppEntryPoint presenter from the first document. So, it will have the following code. 

```
<h1>My App</h1>

<script>

    function MyAppEntryPoint() {
    }

    MyAppEntryPoint.prototype = {

        start: function (parameters) {
            var t = this;
            
            console.log("MyAppEntryPoint.start - ", t, window);

        }
    }

</script>
```

### Adding a label clone

1. In MyAppEntryPoint, add the following tag after the h1 tag.
```
<label h-clone="labelClone" h-handle="label"></label>
```
This tag adds a clone to the presenter which can then be used to create copies using the name "labelClone". 
We can also add the clone logic in the javascript and create a few clones, change their names, and add them to the presenter. 
```
<h1>My App</h1>

<label h-clone="labelClone"></label>

<script>

    function MyAppEntryPoint() {
    }

    MyAppEntryPoint.prototype = {

        start: function (parameters) {
            var t = this;
            
            console.log("MyAppEntryPoint.start - ", t, window);
			
			var clone1 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone1 - ", clone1);
			clone1.dome.innerHTML = "label1-";
			t.hularion.principal.append(clone1.dome);
			
			var clone2 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone2 - ", clone2);
			clone2.dome.innerHTML = "label2-";
			t.hularion.principal.append(clone2.dome);
			
			var clone3 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone3 - ", clone3);
			clone3.dome.innerHTML = "label3-";
			t.hularion.principal.append(clone3.dome);
			
        }
    }

</script>
```
Notice that the t.hularion.createClone method is used to create the clone by passing the name of the clone. this.hularion.presenter is a reference to the DOM element of the presenter instance, which is where the clones are being added.
![Image](LabelClones.png)

2. Next, we are going to make a div clone, and we will add some elements to that clone. Add the following HTML.
```
<div h-clone="divClone" class="divClone">
	<div class="outerDiv">
		<div class="innerDiv">
			<h1 h-handle="h1"></h1>
		</div>
		<p h-handle="p"></p>
	</div>
</div>
```
Within clones, you can add handles to tags in that clone. These are available in the "handles" object within the returned clone object.

Next, let's add some javascript to create a couple of clones and add them to the presenter.
```
var divClone1 = t.hularion.createClone("divClone");			
console.log("MyAppEntryPoint.start divClone1 - ", divClone1);
divClone1.handles.h1.innerHTML = "h1-1";
divClone1.handles.p.innerHTML = "p-1";
t.divCloneContainer.append(divClone1.dome);

var divClone2 = t.hularion.createClone("divClone");			
console.log("MyAppEntryPoint.start divClone2 - ", divClone2);
divClone2.handles.h1.innerHTML = "h1-2";
divClone2.handles.p.innerHTML = "p-2";
t.divCloneContainer.append(divClone2.dome);
```

I also added a container for the divCloneClones and some styling. Here is the complete text.
```
<h1>My App</h1>

<div h-handle="divCloneContainer" class="divCloneContainer"></div>

<!-- Although not necessary, clones should be after the presenter view but before the script. -->

<label h-clone="labelClone"></label>

<div h-clone="divClone" class="divClone">
	<div class="outerDiv">
		<div class="innerDiv">
			<h1 h-handle="h1"></h1>
		</div>
		<p h-handle="p"></p>
	</div>
</div>


<script>

    function MyAppEntryPoint() {
    }

    MyAppEntryPoint.prototype = {

        start: function (parameters) {
            var t = this;
            
            console.log("MyAppEntryPoint.start - ", t, window);
			
			var clone1 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone1 - ", clone1);
			clone1.dome.innerHTML = "label1-";
			t.hularion.principal.append(clone1.dome);
			
			var clone2 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone2 - ", clone2);
			clone2.dome.innerHTML = "label2-";
			t.hularion.principal.append(clone2.dome);
			
			var clone3 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone3 - ", clone3);
			clone3.dome.innerHTML = "label3-";
			t.hularion.principal.append(clone3.dome);
						
			var divClone1 = t.hularion.createClone("divClone");			
            console.log("MyAppEntryPoint.start divClone1 - ", divClone1);
			divClone1.handles.h1.innerHTML = "h1-1";
			divClone1.handles.p.innerHTML = "p-1";
			t.divCloneContainer.append(divClone1.dome);
			
			var divClone2 = t.hularion.createClone("divClone");			
            console.log("MyAppEntryPoint.start divClone2 - ", divClone2);
			divClone2.handles.h1.innerHTML = "h1-2";
			divClone2.handles.p.innerHTML = "p-2";
			t.divCloneContainer.append(divClone2.dome);
			
        }
    }

</script>

<style>

	.divCloneContainer{
		display: inline-block;
	}

	.divClone{
		width:200px;
		height:200px;
		margin:20px;
		background-color: black;
		float:left;
	}
	
	.outerDiv{
		width:80%;
		height:80%;
		background-color: lightblue;
	}
	
	.innerDiv{
		width:80%;
		height:80%;
		background-color: lightgreen;
	}

</style>
```

![Image](DivClones.png)

3. Clone Instances
In HX, you can create instances of clones and insert them as tags onto the presenter. In this example, we will create a couple of labelClone instances and add them just under the My App h1.
```
<hx h-clone-instance="labelClone" h-handle="labelInstace1" />
<br>
<hx h-clone-instance="labelClone" h-handle="labelInstace2" />
<br>
```
Then in the javascript, we will give them some new HTML text using the assigned handles.
```
t.labelInstace1.dome.innerHTML = "labelInstace1";
t.labelInstace2.dome.innerHTML = "labelInstace2";
```
![Image](LabelInstances.png)

Full Code
```
<h1>My App</h1>

<hx h-clone-instance="labelClone" h-handle="labelInstace1" />
<br>
<hx h-clone-instance="labelClone" h-handle="labelInstace2" />
<br>
<div h-handle="divCloneContainer" class="divCloneContainer"></div>


<!-- Although not necessary, clones should be after the presenter view but before the script. -->

<label h-clone="labelClone"></label>

<div h-clone="divClone" class="divClone">
	<div class="outerDiv">
		<div class="innerDiv">
			<h1 h-handle="h1"></h1>
		</div>
		<p h-handle="p"></p>
	</div>
</div>


<script>

    function MyAppEntryPoint() {
    }

    MyAppEntryPoint.prototype = {

        start: function (parameters) {
            var t = this;
            
            console.log("MyAppEntryPoint.start - ", t, window);
			
			var clone1 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone1 - ", clone1);
			clone1.dome.innerHTML = "label1-";
			t.hularion.principal.append(clone1.dome);
			
			var clone2 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone2 - ", clone2);
			clone2.dome.innerHTML = "label2-";
			t.hularion.principal.append(clone2.dome);
			
			var clone3 = t.hularion.createClone("labelClone");			
            console.log("MyAppEntryPoint.start clone3 - ", clone3);
			clone3.dome.innerHTML = "label3-";
			t.hularion.principal.append(clone3.dome);
						
			var divClone1 = t.hularion.createClone("divClone");			
            console.log("MyAppEntryPoint.start divClone1 - ", divClone1);
			divClone1.handles.h1.innerHTML = "h1-1";
			divClone1.handles.p.innerHTML = "p-1";
			t.divCloneContainer.append(divClone1.dome);
			
			var divClone2 = t.hularion.createClone("divClone");			
            console.log("MyAppEntryPoint.start divClone2 - ", divClone2);
			divClone2.handles.h1.innerHTML = "h1-2";
			divClone2.handles.p.innerHTML = "p-2";
			t.divCloneContainer.append(divClone2.dome);
			
			t.labelInstace1.dome.innerHTML = "labelInstace1";
			t.labelInstace2.dome.innerHTML = "labelInstace2";
        }
    }

</script>

<style>

	.divCloneContainer{
		display: inline-block;
	}

	.divClone{
		width:200px;
		height:200px;
		margin:20px;
		background-color: black;
		float:left;
	}
	
	.outerDiv{
		width:80%;
		height:80%;
		background-color: lightblue;
	}
	
	.innerDiv{
		width:80%;
		height:80%;
		background-color: lightgreen;
	}

</style>
```

Similarly, instances of divClone could be placed onto the presenter and could then be managed by the javascript.




### The End