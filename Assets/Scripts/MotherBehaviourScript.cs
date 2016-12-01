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

	enum ProcessState { Start=0, Walking=1, Talking=2, Waiting=5, End=6 	};
	enum Command	  { toWalk , toWait, toTalk, toEnd };

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
				{ new StateTransition(ProcessState.Waiting, Command.toTalk), ProcessState.Talking },
				{ new StateTransition(ProcessState.Talking, Command.toWait), ProcessState.Waiting },
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
	NavMeshAgent naviAgent;

	//This variable has the destination to which the mother has to go when she is in the walking state
	private Vector3 dest;

	//Temp variable	
	float talkingTimer = 5.0f;

	// Use this for initialization
	void Start () {
		p = new Process ();
		audioSrc = transform.GetComponent<AudioSource>();
		naviAgent = transform.GetComponent<NavMeshAgent> ();
		naviAgent.Stop ();
	}

	void Update () {
		switch (p.CurrentState) {

		case ProcessState.Start:
			print ("Mother : Start");
			p.MoveNext (Command.toWait);
			break;
		case ProcessState.Walking:
			print("Mother : Walking Dist: " + naviAgent.remainingDistance );
			if (3 > naviAgent.remainingDistance)	{ // destination is reached 
				naviAgent.Stop();
				//Wait for the storyEngine to decide what to do nextß
				p.MoveNext(Command.toWait);
			}
			// Move towards the dest
			else {
				//Vector3 diff = dest - transform.position;
				//transform.position += diff * speed;
			}
			break;
		case ProcessState.Talking:
			print("Mother : CaveIntro time Left: "+talkingTimer);
			//TODO: Play the audio for on cave intro and move the state to wait
			// temporarily wait for a timeout
			talkingTimer -= Time.deltaTime;
			if (0 > talkingTimer) {
				GameObject player1 = GameObject.FindWithTag("FirstPlayer");
                //GameObject player2 = GameObject.FindWithTag("SecondPlayer");
                PlayersBehaviourScript p1 = player1.GetComponent<PlayersBehaviourScript>();
                p1.setStoryMode(false);
                //GetCameraValues g2 = player2.GetComponent<GetCameraValues>();
                //g1.setStoryMode(false);
                //g2.setStoryMode(false );
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
		//dest = location;
		p.MoveNext (Command.toWalk);
		naviAgent.SetDestination(location);
		naviAgent.Resume ();
	}

	public void startTalking(int speech)	{
		GameObject player1 = GameObject.FindWithTag("FirstPlayer");
		//GameObject player2 = GameObject.FindWithTag("SecondPlayer");
		if (null!=player1)// && null != player2)
		{
			if (3 == speech || 5 == speech) 
			{
				PlayersBehaviourScript p1 = player1.GetComponent<PlayersBehaviourScript>();
				//PlayersBehaviourScript p2 = player2.GetComponent<PlayersBehaviourScript>();
				p1.setStoryMode(true);
				//g2.setStoryMode(true);
			}
			p.MoveNext(Command.toTalk);
			audioSrc.clip = audioFiles[speech];
			audioSrc.Play();
			talkingTimer = audioSrc.clip.length;
		}
	}
}
