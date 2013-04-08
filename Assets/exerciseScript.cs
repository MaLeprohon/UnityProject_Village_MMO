using UnityEngine;
using System.Collections;

public class exerciseScript : MonoBehaviour {
	
	GameObject cube;
	bool shrinking = true;
	float shrinkSpeed = 0.1f;
	float targetScale = 0.1f;

	// Use this for initialization
	void Start () {
		
		cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.position = new Vector3(0,0,0);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if(Input.GetButton("Jump"))
		{
			cube.transform.Rotate(Vector3.up, Time.deltaTime*10);
		}
		
		if(Input.GetKey(KeyCode.M))
		{
			cube.transform.position = new Vector3(10,0,10);
		}
		
		
		
	
	
	}
}
