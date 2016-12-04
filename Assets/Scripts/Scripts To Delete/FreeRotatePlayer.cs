using UnityEngine;
using System.IO;
using System.Collections;

public class FreeRotatePlayer : MonoBehaviour {

	//simple camera rotation shamelessly stolen from top answer of http://gamedev.stackexchange.com/questions/104693/how-to-use-input-getaxismouse-x-y-to-rotate-the-camera
	//we broke this shit because it was interfering with rotation towards a thing. need to fix.
	public float speedH = 2.0f;
	public float speedV = 2.0f;
	public bool canRotate = true;

	private float yaw = 0.0f;
	private float pitch = 0.0f;

	void Update () {
		/*
		if (Input.GetButtonDown ("Jump")) {
			if (canRotate) {
				canRotate = false;
				Debug.Log ("Can't rotate!");
			} else {
				canRotate = true;
				Debug.Log ("Can rotate!");
			}
		}
		*/
		if (canRotate) {
			yaw = speedH * Input.GetAxis ("Mouse X");
			pitch = speedV * Input.GetAxis ("Mouse Y");

			transform.Rotate(pitch, yaw, 0.0f);
		} 
		else {
			
		}

	}
}
