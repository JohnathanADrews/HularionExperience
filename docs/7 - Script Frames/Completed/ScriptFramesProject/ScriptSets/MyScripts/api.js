
function API(){
	this.Calculator = Calculator;
	this.Worker = Worker;
	
}
API.prototype ={	
	getCommonPresenters: function(){
		return sets.common;
	},
	getJobs: function(){
		return sets.jobs;
	}
}
