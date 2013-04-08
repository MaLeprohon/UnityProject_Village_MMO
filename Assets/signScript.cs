using UnityEngine;
using System.Collections;



public class signScript : MonoBehaviour {
	
	GameObject sign;
	float distance = 0.0f;
	bool showSign = false;
	public GUIStyle style;
	string content = "yeah";
	GameObject character;
	 public Texture2D textureToDisplay;
	

	// Use this for initialization
	void Start () {
	sign = GameObject.Find("sign3");
	character = GameObject.Find("character");
	}
	
	// Update is called once per frame
	void Update () {
		
		distance = Vector3.Distance(character.transform.position, gameObject.transform.position);
		
	
	}
	
	void OnGUI()
	{
		if(distance < 5.0f)
		  GUI.Label(new Rect(Screen.width/2-(textureToDisplay.width/2), Screen.height/2-(textureToDisplay.height/2), textureToDisplay.width, textureToDisplay.height), textureToDisplay);
		
	}
	
    

}
