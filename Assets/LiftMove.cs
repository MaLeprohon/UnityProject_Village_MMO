using UnityEngine;
using System.Collections;

public class LiftMove : MonoBehaviour {
	
	bool move = false;
	float startTime = 0.0f;
	float duration = 5.0f;
	Vector3 endPoint = new Vector3(281.228f,67,179.1852f);
	Vector3 startPoint;
	float ratio;
	float fractTime;
	Vector3 upPos;
	float speed = .4f;
	
	// Use this for initialization
	void Start () {
		
		startTime = Time.time;
		startPoint = gameObject.transform.position;
		//StartCoroutine(CoStart());
		ratio = Time.deltaTime;
		
		
	
	}
	
	
	void FixedUpdate()
	{
		
		
	}
	// Update is called once per frame
	void Update () {
	
		
		gameObject.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.PingPong(Mathf.Sin(Time.time*speed)/2+.5f, 1.0f));	
		//gameObject.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Sin(Time.time * speed) * 0.5f + 0.5f);

		
		//gameObject.transform.position = Vector3.Lerp(endpoint, startPoint, (Time.deltaTime - startTime) / duration);
		//LerpMove();

	}
	/*
	IEnumerator CoStart()
	{
		
		 yield return (CoUpdate());
		
	}

	IEnumerator CoUpdate()
	{
		while(true)
		{
			LerpMove();
			yield return StartCoroutine(Wait());
			/*
			if(gameObject.transform.position.y < 66 && move == false)
			{
				
				gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
				
			}
			
			else if (move == false)
			{
				move = true;
				yield return StartCoroutine(Wait());
			}
			
			
			
			if(gameObject.transform.position.y > 1 && move == true)
			{
				gameObject.transform.Translate(Vector3.down * speed * Time.deltaTime);
				
			}
			
			else if (move == true)
			{
				move = false;
				yield return StartCoroutine(Wait());
			}
			
		}
		
	}
	*/
	
	void moveLift()
	{
		if(gameObject.transform.position.y < 66 && move == false)
		{
			
			gameObject.transform.Translate(Vector3.up * speed * Time.deltaTime);
		}
		
		else if (move == false)
		{
			StartCoroutine(Wait());
			move = true;
		}
		
		
		
		if(gameObject.transform.position.y > 1 && move == true)
		{
			gameObject.transform.Translate(Vector3.down * speed * Time.deltaTime);
		}
		
		else if (move == true)
		{
			StartCoroutine(Wait());
			move = false;
		}
	}
	
	void LerpMove()
	{
		gameObject.transform.position = Vector3.Lerp(startPoint, endPoint, (Time.time - startTime) / duration);
		
	}
	
	 

	IEnumerator Wait() 
	{
		Debug.Log ("waiting");
        yield return new WaitForSeconds(5.0F);
    }
}
