using UnityEngine;
using System.Collections;

public class MotherBehaviourScript : MonoBehaviour {

	private float timeOut = 5.0f;

	bool b_Talking = false;

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
		}
	}

	public void StartTalking() {
		b_Talking = true;
	}

	public bool isTalking()	{
		return b_Talking;
	}
}
