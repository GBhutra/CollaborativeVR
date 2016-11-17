using UnityEngine;
using System.Collections;

public class MotherBehaviourScript : MonoBehaviour {

	private float timeOut = 5.0f;
	public GameObject CR;

	bool b_Talking = false;
	bool b_moveToCR = false;
	float speed = 0.01f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (b_Talking) {
			print ("Mother: talking"); 
			timeOut = timeOut - Time.deltaTime;
		}
		else
			print ("Mother: NOT talking"); 

		if (0 > timeOut) {
			timeOut = 2.0f;
			b_Talking = false;
			b_moveToCR = true;

		}
		if (b_moveToCR)	{
			Vector3 diff = CR.transform.position - transform.position;
			transform.position += diff * speed;

			if (5 > diff.magnitude)
				b_moveToCR = false;
		}
	}


	public void StartTalking() {
		b_Talking = true;
	}

	public bool isTalking()	{
		return b_Talking;
	}
}
