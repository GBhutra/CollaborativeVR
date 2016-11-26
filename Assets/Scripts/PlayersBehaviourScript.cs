using UnityEngine;
using System.Collections;

public class PlayersBehaviourScript : MonoBehaviour
{
	enum RotationMode { Distrator, GuidedRotation, SnapToTarget, Free };
	enum Gate { Cave1=1, Cave2=2, Cave3=3, Cave4=4, Loop=5 };

	//inGate : keeps track of which cave the player is in. Cave3 => Spawning cave
	private int inGate = (int)Gate.Cave3;

	private GameObject baby;

	private RotationMode selectedMode = RotationMode.SnapToTarget;

	private Transform target;
	private GameObject distractor;

	public float rotationRate = 0.5f;
	private float time = 3.0f;

	//This is a boolean indicates that the mother is talking
	bool b_ongoingStory = false;

	//Trigger When the player passes through a gate
	void OnTriggerEnter(Collider other) {
		GameObject cave = GameObject.Find ("Cave");
		StoryEngineScript s = cave.GetComponent<StoryEngineScript> (); 
		switch (other.gameObject.name)	{
		case "caveGate1":
			inGate = (int)Gate.Cave1;
			s.visitedCave (1);
			break;
		case "caveGate2":
			inGate = (int)Gate.Cave2;
			s.visitedCave (2);
			break;
		case "caveGate3":
			inGate = (int)Gate.Cave3;
			break;
		case "caveGate4":
			inGate = (int)Gate.Cave4;
			s.visitedCave (4);
			break;
		case "caveLoopGate1":
		case "caveLoopGate2":
			inGate = (int)Gate.Loop;
			s.visitedCave (3);
			break;
		}
	}
	// Use this for initialization  
	void Start()
	{
		target = GameObject.Find("Mother").transform;
		baby = GameObject.Find("Baby");
		if ("FirstPlayer" == gameObject.tag)    
			distractor = GameObject.Find("fireflyPlayer1");
		else if ("SecondPlayer" == gameObject.tag)
			distractor = GameObject.Find("fireflyPlayer2");
	}

	// Update is called once per frame
	void Update()
	{
		if (b_ongoingStory) {
			Vector3 localTarget = Camera.main.transform.InverseTransformPoint(target.position);
			float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

			switch (selectedMode)
			{
			case (RotationMode.Distrator):
				DistractorFunction(targetAngle);
				break;
			case (RotationMode.GuidedRotation):
				GuidedRotationFunction(targetAngle);
				break;
			case (RotationMode.SnapToTarget):
				SnapToTargetFunction(targetAngle);
				break;
			default:
				//Free Rotation Do Nothing
				break;
			}
		}

		if (5>(baby.transform.position - transform.position).magnitude) {
			GameObject cave = GameObject.Find ("Cave");
			StoryEngineScript s = cave.GetComponent<StoryEngineScript> (); 
			s.setBabyFound (true);
		}
	}

	// Redirection based on gaze of the viewer.
	void GuidedRotationFunction(float targetAngle)
	{
		if (targetAngle > 0)
		{
			transform.Rotate(Vector3.up, rotationRate);
		}
		else
		{
			transform.Rotate(Vector3.up, rotationRate * (-1));
		}
	}

	// Fade in and Fade out Redirection
	void SnapToTargetFunction(float targetAngle)
	{
		//TODO : Add a fade in fade out sequence
		transform.Rotate(Vector3.up, targetAngle);
	}

	// Visual distractor based Redirection
	void DistractorFunction(float targetAngle)
	{
		if (5<targetAngle || -5>targetAngle)	
		{
			//Move the distractor 20 units away from where the camera is looking at
			distractor.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
			//Initialize the distractor
			distractor.SetActive(true);
		}
		else
		{
			distractor.SetActive(false);
		}
	}

	public void setStoryMode(bool val)
	{
		b_ongoingStory = val;
	}
}

