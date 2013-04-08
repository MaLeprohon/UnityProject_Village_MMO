using UnityEngine;
using System.Collections;

public class cubePress : MonoBehaviour {
	
	
	GameObject character;
	GameObject cubeToPress;
	float distance = 0.0f;
	bool isSpinning = false;

	// Use this for initialization
	void Start () {
		
		character = GameObject.Find("Camera");
		cubeToPress = GameObject.Find("press");
		GameObject.Find ("Cone001").animation["windmillrotation"].wrapMode = WrapMode.Loop;
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
	distance = Vector3.Distance(character.transform.position, cubeToPress.transform.position);
	
	}
	
	void OnMouseDown()
	{
		
		Debug.Log (distance);
		
		if(distance < 5.0f)
		{
			if(isSpinning == false)
			{	
				Debug.Log (isSpinning);
				GameObject.Find ("Cone001").animation["windmillrotation"].speed = 1;
				GameObject.Find("Cone001").animation.Play();
				isSpinning = true;
			}
			
			else if(isSpinning == true)
			{
				GameObject.Find ("Cone001").animation["windmillrotation"].speed = 0;
				isSpinning = false;
			}
		}
	}
}
