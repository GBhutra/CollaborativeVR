using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryEngineScript : MonoBehaviour {
	/*
	 * Shots transition as per the followiwing state machine
	 * 															|‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|
	 * Start - CaveIntro - CrossRoads - FindTheBaby - BabyFound - End
	 * 											|________________________________|
	 *
	 * The entry and exit conditions of individual cases are explained in the main update loop
	*/

	enum Shot { Start, CaveIntro, CrossRoads, FindTheBaby, BabyFound, End };
	enum Command { toStart , toNextShot, toEnd };

	class Process
	{
		class StateTransition
		{
			readonly Shot CurrentShot;
			readonly Command Command;

			public StateTransition(Shot shot, Command command)
			{
				CurrentShot = shot;
				Command = command;
			}

			public override int GetHashCode()
			{
				return 17 + 31 * CurrentShot.GetHashCode() + 31 * Command.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				StateTransition other = obj as StateTransition;
				return other != null && this.CurrentShot == other.CurrentShot && this.Command == other.Command;
			}
		}

		Dictionary<StateTransition, Shot> transitions;
		public Shot CurrentShot { get; set; }

		// Constructor for creating the process variable
		public Process()
		{
			CurrentShot = Shot.Start;
			transitions = new Dictionary<StateTransition, Shot>
			{
				{ new StateTransition(Shot.Start, Command.toNextShot), Shot.CaveIntro },
				{ new StateTransition(Shot.CaveIntro, Command.toNextShot), Shot.CrossRoads },
				{ new StateTransition(Shot.CrossRoads, Command.toNextShot), Shot.FindTheBaby },
				{ new StateTransition(Shot.FindTheBaby, Command.toNextShot), Shot.BabyFound },
				{ new StateTransition(Shot.BabyFound, Command.toNextShot), Shot.End },
				{ new StateTransition(Shot.CrossRoads, Command.toEnd), Shot.End },
				{ new StateTransition(Shot.FindTheBaby, Command.toEnd), Shot.End }
			};
		}

		public Shot GetNext(Command command)
		{
			StateTransition transition = new StateTransition(CurrentShot, command);
			Shot nextState = Shot.Start;
			if (!transitions.TryGetValue (transition, out nextState))
				print ("Invalid Transition ! reseting the current state!");
			return nextState;
		}

		public Shot MoveNext(Command command)
		{
			CurrentShot = GetNext(command);
			return CurrentShot;
		}
	}

	//The FSM state variable
	private Process p;

	//Locations
	public GameObject caveIntro;
	public GameObject crossRoads;


	public GameObject player1;
	public GameObject player2;

	public GameObject mother;


	// Use this for initialization
	void Start () {
		p = new Process();
	}
	
	// Update is called once per frame
	void Update () {
		MotherBehaviourScript m = mother.GetComponent<MotherBehaviourScript> (); 
		switch (p.CurrentShot) {
		case Shot.Start:
			print (" In the Starting Shot");
			if (!m.isTalking ())
				m.StartTalking ();
			p.MoveNext (Command.toNextShot);
			break;
		case Shot.CaveIntro:
			if (5 > (caveIntro.transform.position - mother.transform.position).magnitude) {
				// Do the Talking and the move to CR
				print (" In the cave intro Shot");
			} else if (6 < (crossRoads.transform.position - mother.transform.position).magnitude)
				p.MoveNext (Command.toNextShot);
			break;
		case Shot.CrossRoads:
			print (" In the cave intro CrossRoad");
			if (!m.isTalking ())
				m.StartTalking ();
			break;
		}
	}
}
