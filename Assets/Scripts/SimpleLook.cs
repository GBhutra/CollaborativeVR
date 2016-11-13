using UnityEngine;
using System.IO;
using System.Collections;

public class SimpleLook : MonoBehaviour {

	public string playerName = "Player Capsule";
	public string targetName = "Target Capsule";
	public string inputName = "Rotate";

	public float rotationRate = 3.0f;	//degrees per update

	private GameObject target, player;
	private Vector3 targetFront, targetBack, playerFront, playerBack;
	private bool rotating = false;

	void updateForwardVector(GameObject obj, Vector3 vec){
		vec = obj.transform.forward;
	}

	void updateForwardVectors(){
		targetFront = target.transform.forward;
		playerFront = player.transform.forward;
		Debug.Log ("target vector = (" + targetFront.x + ", " + targetFront.y + ", " + targetFront.z + ")");
		Debug.Log ("player vector = (" + playerFront.x + ", " + playerFront.y + ", " + playerFront.z + ")");
		
	}

	void Start(){
		target = GameObject.Find (targetName);
		player = this.gameObject;
		//myCamera = GameObject.Find ("Camera");
		updateForwardVectors();
	}

	// Update is called once per frame
	void Update () {

		//forces target to constantly look at player; need to find a way around this
		//workaround; attach invisible object to focal point, set invis as target
		target.transform.LookAt (player.transform);
		updateForwardVector(target,targetFront);

		float rads = Mathf.Deg2Rad * rotationRate;

		if (Input.GetKeyDown(KeyCode.P)) {
			Debug.Log ("Button has been pressed!");
			rotating = !rotating;
		}

		if (rotating){

			updateForwardVectors();
			float angleDifference = Vector3.Angle (playerFront, targetFront);
			Debug.Log ("Angle from player to target = " + angleDifference);

			if (angleDifference != 180) {
				Vector3 newPlayerFront = Vector3.RotateTowards(playerFront, (-1 * targetFront), rads, 0.0f);
				player.transform.rotation = Quaternion.LookRotation(newPlayerFront);
			}
			else{
				rotating = false;
			}
		}
	}
}
