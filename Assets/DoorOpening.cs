using UnityEngine;
using System.Collections;

public class DoorOpening : MonoBehaviour {
	
	bool isOpen = false;
	GameObject character;
	float distance;
	
	// Use this for initialization
	void Start () {
	character = GameObject.Find("Camera");
	}
	
	// Update is called once per frame
	void Update () {
		
		distance = Vector3.Distance(character.transform.position, gameObject.transform.position);
		
		if(Input.GetKeyDown("e") && distance < 20)
		{
			if(isOpen == false)
			{
				gameObject.animation["bestDoorAnim"].speed = 1;
				gameObject.animation.Play();
				isOpen = true;
			}
			
			else if(isOpen == true && distance < 20)
			{
				gameObject.animation["bestDoorAnim"].speed = -1;
				gameObject.animation["bestDoorAnim"].time = gameObject.animation["bestDoorAnim"].length;
				gameObject.animation.Play();
				isOpen = false;
			}
			
			
		}
	
	}
	

}
