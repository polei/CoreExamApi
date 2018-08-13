// JavaScript Document
function zoomMain()
	{
	    if(document.getElementById("leftDiv").style.display == "none")
	    {
	        document.getElementById("leftDiv").style.display = "block";
	        document.getElementById("mnav").style.left = "173px";
	    }
	    else
	    {
	        document.getElementById("leftDiv").style.display = "none";
	        document.getElementById("mnav").style.left = "0px";
	    }
	}
	function mouseofzoom(obj)
	{
	    obj.className = "zoomout";
	}
	function mouseoutzoom(obj)
	{
	    obj.className = "zoombar";
	}