var origBoxClr:Color;
var buttonName:String;
var boltName:GameObject;

function Start()
{
	origBoxClr = renderer.material.color;
	
	boltName.GetComponent(BoltControl).UnScrew();
}

function OnMouseEnter()
{
	//change the color of the cube
	renderer.material.color = Color.white;

	transform.Find(buttonName).gameObject.renderer.material.color = origBoxClr;
	
	iTween.MoveTo(gameObject,{"z":-0.25, "time":0.5});			
	
	boltName.GetComponent(BoltControl).Screw();
}

function Update()
{
	if( Input.GetMouseButtonUp(0)  && buttonName=="QUIT")
	{
		Application.Quit();
	}
}

function OnMouseExit()
{
	//change the color of the cube
	renderer.material.color = origBoxClr;
	
	transform.Find(buttonName).gameObject.renderer.material.color = Color.white;
		
	iTween.MoveTo(gameObject,{"z":0, "time":1});
	
	boltName.GetComponent(BoltControl).UnScrew();
}

function OnMouseDown()
{
	//change the scale of the cube
	renderer.transform.localScale += Vector3(0.1,0.05,0.025);
	
	//change the color of the text
	transform.Find(buttonName).gameObject.renderer.material.color = Color.red;
}

function OnMouseUp()
{
	//change the scale of the cube
	renderer.transform.localScale -= Vector3(0.1,0.05,0.025);
	
	//change the color of the text
	transform.Find(buttonName).gameObject.renderer.material.color = origBoxClr;
}