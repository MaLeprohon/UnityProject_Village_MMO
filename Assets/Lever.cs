using UnityEngine;
using System.Collections;

public class Lever : MonoBehaviour {
	
	
	float distance = 0.0f;
	GameObject character;
	GameObject LeverPivot;
	bool levUp = false;
	bool isSpinning = false;

	
	
	// Use this for initialization
	void Start () {
	
		character = GameObject.Find("character");
		LeverPivot = GameObject.Find ("LeverPivot");
		
		GameObject.Find ("Cone001").animation["windmillrotation"].wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
		
		distance = Vector3.Distance(character.transform.position, gameObject.transform.position);

	
	}
		
		void OnMouseDown()
		{
			
		if(distance < 5.0f && levUp == false)
		{
			LeverPivot.transform.Rotate(-90,0,0);
			/*
			LeverPivot.animation["lever_anim"].speed = 1;
			LeverPivot.animation.Play();
			*/
			if(isSpinning == false)
			{	
				Debug.Log (isSpinning);
				GameObject.Find ("Cone001").animation["windmillrotation"].speed = 1;
				GameObject.Find("Cone001").animation.Play();
				isSpinning = true;
			}
			
			
			
			levUp = true;
		}
			
		
			else if(distance < 5.0f && levUp == true)
		{
			LeverPivot.transform.Rotate(90,0,0);
			/*
			 * 
			LeverPivot.animation["lever_anim"].speed = -1;
			LeverPivot.animation.Play();
			*/
			if(isSpinning == true)
			{
				GameObject.Find ("Cone001").animation["windmillrotation"].speed = 0;
				isSpinning = false;
			}
			levUp = false;
		}
			
		}
		
		

}
