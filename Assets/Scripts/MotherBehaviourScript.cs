using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MotherBehaviourScript : MonoBehaviour {

	/*
	 * ProcessState is an enum for the states of the Mother Behaviour
	 * Talking : This indicates that the distractor technique should be active
	 * Walking : Mother moving from one location to another
	 * Talking : Mother hollering at the players for help, reminder, etc
	 * Waiting :  Mother is waiting for the timeouts to expire or player to join the search for the baby
	 * End : Based on the baby found or not
	*/

	enum ProcessState { Start=0, Walking=1, CaveIntro=2, CrossRoads=3 ,Warning=4, Waiting=5, End=6 	};
	enum Command	  { toWalk , toWait, toIntroduceCave, toExplainCrossRoads, toWarn, toEnd };

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

		// Constructor for creating the process variable
		public Process()
		{
			CurrentState = ProcessState.Start;
			transitions = new Dictionary<StateTransition, ProcessState>
			{
				{ new StateTransition(ProcessState.Start, Command.toWait), ProcessState.Waiting },
				{ new StateTransition(ProcessState.Walking, Command.toWait), ProcessState.Waiting },
				{ new StateTransition(ProcessState.Waiting, Command.toIntroduceCave), ProcessState.CaveIntro },
				{ new StateTransition(ProcessState.Waiting, Command.toExplainCrossRoads), ProcessState.CrossRoads },
				{ new StateTransition(ProcessState.Waiting, Command.toWarn), ProcessState.Warning },
				{ new StateTransition(ProcessState.CaveIntro, Command.toWait), ProcessState.Waiting },
				{ new StateTransition(ProcessState.CrossRoads, Command.toWait), ProcessState.Waiting },
				{ new StateTransition(ProcessState.Warning, Command.toWait), ProcessState.Waiting },
				{ new StateTransition(ProcessState.Waiting, Command.toWalk), ProcessState.Walking }

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
			CurrentState = GetNext(command);
			return CurrentState;
		}
	}

	//The variable which gives information on the mothers state
	private Process p;


	// audioFiles :  this is an array of audio Files
	private AudioSource audioSrc;
	public AudioClip[] audioFiles;
	enum motherAudio { 
		Bad1=0, 
		Bad2=1, 
		Bad3=2,
		Intro=3,
		Timeout1,
		CrossRoads,
		Timeout2,
		BabyFound,
		Monster,
		ReUnite1,
		ReUnite2,
		ReUnite3
	};

	//This variable has the destination to which the mother has to go when she is in the walking state
	private Vector3 dest;
	float speed = 0.001f;

	//Temp variable	
	float timerForCaveIntro = 7.0f;
	float timerForCrossRoads = 15.0f;
	float timerForWarningCall = 5.0f;

	// Use this for initialization
	void Start () {
		p = new Process ();
		audioSrc = transform.GetComponent<AudioSource>();
	}
		
	void Update () {
		switch (p.CurrentState) {

		case ProcessState.Start:
			print ("Mother : Start");
			p.MoveNext (Command.toWait);
			break;
		case ProcessState.Walking:
			print("Mother : Walking");
			float dist = (dest - transform.position).magnitude;

			if (5 > dist)	{ // destination is reached 
				//Wait for the storyEngine to decide what to do nextß
				p.MoveNext(Command.toWait);
			}
			// Move towards the dest
			else {
				Vector3 diff = dest - transform.position;
				transform.position += diff * speed;
			}
			break;
		case ProcessState.CaveIntro:
			print("Mother : CaveIntro time Left: "+timerForCaveIntro);
			//TODO: Play the audio for on cave intro and move the state to wait
			// temporarily wait for a timeout
			timerForCaveIntro -= Time.deltaTime;
			if (0 > timerForCaveIntro) {
				timerForCaveIntro = 7.0f;
				p.MoveNext (Command.toWait);
			}
			break;
		case ProcessState.CrossRoads:
			print("Mother : CrossRoads time Left: "+timerForCrossRoads);
			//TODO: Play the audio for on crossRoads and move the state to wait
			// temporarily wait for a timeout
			timerForCrossRoads -= Time.deltaTime;
			if (0 > timerForCrossRoads) {
				timerForCrossRoads = 15.0f;
				p.MoveNext (Command.toWait);
			}
			break;
		case ProcessState.Warning:
			print("Mother : timerForWarningCall time Left: "+timerForWarningCall);
			//TODO: Play the audio for on crossRoads and move the state to wait
			// temporarily wait for a timeout
			timerForWarningCall -= Time.deltaTime;
			if (0 > timerForWarningCall) {
				p.MoveNext (Command.toWait);
			}
			break;
		}
	}


	public int getState() {
		return (int)p.CurrentState;
	}

	//This method is invoked by the story engine and makes the mother to move from to a certain location
	public void moveTo(Vector3 location)	{
		dest = location;
		p.MoveNext (Command.toWalk);
	}
		
	public void introduceTheCave()	{
		audioSrc.clip = audioFiles [(int)motherAudio.Intro];
		audioSrc.Play();
		timerForCaveIntro = audioSrc.clip.length;
		p.MoveNext (Command.toIntroduceCave);
	}
		
	public void explainCrossRoads()	{
		audioSrc.clip = audioFiles [(int)motherAudio.CrossRoads];
		audioSrc.Play();
		timerForCrossRoads = audioSrc.clip.length;
		p.MoveNext (Command.toExplainCrossRoads);
	}

	public void warningCall(int i)	{
		if (1 == i) {
			audioSrc.clip = audioFiles [(int)motherAudio.Timeout1];
			audioSrc.Play();
			timerForWarningCall = audioSrc.clip.length;
		}
		if (2 == i) {
			audioSrc.clip = audioFiles [(int)motherAudio.Timeout2];
			audioSrc.Play();
			timerForWarningCall = audioSrc.clip.length;
		}
		p.MoveNext (Command.toWarn);
	}
}
