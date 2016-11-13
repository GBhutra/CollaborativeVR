using UnityEngine;
using System.Collections;

public class fireflyBehaviourScript : MonoBehaviour {

	/*
	 * ProcessState is an enum for the states of the fireflyBehaviour
	 * Start : location of the firefly should be at the bottom of the user camera
	 * Inview : firefly comes into the center of the user screen
	 * Guiding : firefly moves out of view of the camera and waits for the user to look at it or resets to start
	 * Timer :  In this state the the firefly waits for the timeout between the other states
	 * End : when the user is looking at the target object
	*/
	enum ProcessState { Start, InView, Guiding, Timer, End 	};
	enum Command	  { toStart , toInView, toGuiding, toEnd, toTimer };

	class Process
	{
		class StateTransition
		{
			readonly ProcessState CurrentState;
			readonly Command Command;

			public StateTransition(ProcessState currentState, Command command)
			{
				CurrentState = currentState;
				Command = command;
			}

			public override int GetHashCode()
			{
				return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				StateTransition other = obj as StateTransition;
				return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
			}
		}

		Dictionary<StateTransition, ProcessState> transitions;
		public ProcessState CurrentState { get; set; }
		public ProcessState PrevState { get; private set; }

		// Constructor for creating the process variable
		public Process()
		{
			CurrentState = ProcessState.Start;
			PrevState = ProcessState.Start;
			transitions = new Dictionary<StateTransition, ProcessState>
			{
				{ new StateTransition(ProcessState.Start, Command.toTimer), ProcessState.Timer },
				{ new StateTransition(ProcessState.Timer, Command.toInView), ProcessState.InView },
				{ new StateTransition(ProcessState.InView, Command.toTimer), ProcessState.Timer },
				{ new StateTransition(ProcessState.Timer, Command.toGuiding), ProcessState.Guiding },
				{ new StateTransition(ProcessState.Guiding, Command.toEnd), ProcessState.End },
				{ new StateTransition(ProcessState.Guiding, Command.toTimer), ProcessState.Timer },
				{ new StateTransition(ProcessState.Timer, Command.toStart), ProcessState.Start }
			};
		}

		public ProcessState GetNext(Command command)
		{
			StateTransition transition = new StateTransition(CurrentState, command);
			ProcessState nextState = ProcessState.Start;
			if (!transitions.TryGetValue (transition, out nextState))
				print ("Invalid Transition ! reseting the current state!");
			return nextState;
		}

		public ProcessState MoveNext(Command command)
		{
			PrevState = CurrentState;
			CurrentState = GetNext(command);
			return CurrentState;
		}
	}

	// Use this for initialization
	public GameObject target;
	public GameObject FPSCam;
	private Process p;
	float timeOut = 2.0f;
	float speed = 0.2f;

	// Use this for initialization
	void Start () {
		p = new Process ();
	}

	// Update is called once per frame
	void Update()	{
		float angleToTarget = Vector3.Angle(FPSCam.transform.forward, (target.transform.position - FPSCam.transform.position));
		Vector3 startPos = (transform.position - FPSCam.transform.position);
		float angleToCamera = Vector3.Angle (FPSCam.transform.forward, startPos);
		Vector3 diff;
		print ("angle to Target : "+angleToTarget+" angle to camera : "+angleToCamera+" currState : "+p.CurrentState);
		if (25 < angleToTarget) {
			switch (p.CurrentState) {
			case ProcessState.Start:
				//Moving the firefly to the bottom of the view
				timeOut = 1.5f;
				p.MoveNext (Command.toTimer);
				break;
			case ProcessState.Timer:
				timeOut -= Time.deltaTime;
				print (" timeOut : " + timeOut);
				if (ProcessState.Guiding == p.PrevState && 45 >= angleToCamera)
					p.MoveNext (Command.toGuiding);
				if (0 > timeOut) {
					if (ProcessState.Start == p.PrevState)
						p.MoveNext (Command.toInView);
					else if (ProcessState.Guiding == p.PrevState)
						p.MoveNext (Command.toStart);
					else if(ProcessState.InView == p.PrevState)
						p.MoveNext (Command.toGuiding);
				}
				break;

			case ProcessState.InView:
				Vector3 destination = FPSCam.transform.position + FPSCam.transform.forward * 20f;
				diff = transform.position - destination;
				if (1 < diff.magnitude)
					transform.position -= speed * diff;
				else {
					timeOut = 1.0f;
					p.MoveNext (Command.toTimer);
				}
				break;

			case ProcessState.Guiding:
				if (45 >= angleToCamera) {
					float radius = startPos.magnitude;
					Vector3 destPos = radius * ((target.transform.position - FPSCam.transform.position).normalized);
					Vector3 normal = radius * (Vector3.Cross (Vector3.Cross (startPos, destPos), startPos).normalized);
					float angleToDest = Vector3.Angle (startPos, destPos) / 1000;
					Vector3 step = Mathf.Cos (angleToDest) * startPos + Mathf.Sin (angleToDest) * normal;
					diff = transform.position - FPSCam.transform.position - step;
					Debug.DrawLine (FPSCam.transform.position, (FPSCam.transform.position + step));
					transform.position -= speed * diff;
				} else {
					timeOut = 1.0f;
					p.MoveNext (Command.toTimer);
				}
				break;
			case ProcessState.End:
				gameObject.SetActive (false);
				if (25<angleToTarget)
					p.CurrentState = ProcessState.Start;
				break;
			}
		} else {
			gameObject.SetActive(false);
			p.CurrentState = ProcessState.Start;
		}
	}
}
