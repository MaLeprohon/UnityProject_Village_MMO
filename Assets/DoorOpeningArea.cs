using UnityEngine;
using System.Collections;

public class DoorOpeningArea : MonoBehaviour {
	


	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
		/*
		distance = Vector3.Distance(character.transform.position, gameObject.transform.position);
		
		if(distance>20)
		{
			gameObject.animation["bestDoorAnim"].wrapMode = WrapMode.Once;
			gameObject.animation["bestDoorAnim"].speed = 1;
			gameObject.animation.Play();
		}
		
		else if(distance<20)
		{
			gameObject.animation["bestDoorAnim"].wrapMode = WrapMode.Once;
			gameObject.animatio	n["bestDoorAnim"].speed = -1;
			gameObject.animation["bestDoorAnim"].time = gameObject.animation["bestDoorAnim"].length;
			gameObject.animation.Play();
		}
		*/
	
	}
	
	void OnTriggerEnter()
	{
		gameObject.animation["bestDoorAnimArea"].speed = 1;
		gameObject.animation.Play();
	}
	
	void OnTriggerExit()
	{
		gameObject.animation["bestDoorAnimArea"].speed = -1;
		gameObject.animation["bestDoorAnimArea"].time = gameObject.animation["bestDoorAnimArea"].length;
		gameObject.animation.Play();
	}
	
}


