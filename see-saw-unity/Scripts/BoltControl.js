var UnScrewtime:float;
var Screwtime:float;

function UnScrew()
{	
	iTween.MoveTo(gameObject,{"z":-0.1, "time":UnScrewtime});	
}

function Screw()
{
	iTween.MoveTo(gameObject,{"z":0, "time":Screwtime});
}