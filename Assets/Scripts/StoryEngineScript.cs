using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryEngineScript : MonoBehaviour {
	/*
	 * Shots transition as per the followiwing state machine
	 * 											  |‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|
	 * Start - CaveIntro - CrossRoads - FindTheBaby - BabyFound - End
	 * 							  |________________________________|
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
	public GameObject caveIntroZone;
	public GameObject crossRoadZone;


	public GameObject player1;
	public GameObject player2;

	public GameObject mother;

	private bool caveIntro = false;
	private bool crossRoadExpl = false;

	// This is the timeout for the warning call
	float warningTimeout = 10.0f;
	float endTimeOut = 10.0f;


	// Use this for initialization
	void Start () {
		p = new Process();
	}
	
	// Update is called once per frame
	void Update () {
		MotherBehaviourScript m = mother.GetComponent<MotherBehaviourScript> (); 
		switch (p.CurrentShot) {
		case Shot.Start:
			print (" Story : Start or Shot 1");
			//getting the mother to the cave Intro Shot
			float distFromIntro = (caveIntroZone.transform.position - mother.transform.position).magnitude;
			if (5 > distFromIntro) {
				p.MoveNext (Command.toNextShot);
			}
			else {
				// 1 : indicates that the mother is already moving
				if (1!=m.getState())
					m.moveTo (caveIntroZone.transform.position);
			}
			break;
		case Shot.CaveIntro:
			print (" Story : Cave Intro or Shot 2 ");
			// The mother is waiting asking her to introduce the cave
			if (5 == m.getState() && !caveIntro) {	
				//StartCoroutine(m.introduceTheCave ());
				m.introduceTheCave ();
				caveIntro = true;
			}
			else if (5 == m.getState() && caveIntro) {
				float distFromCR = (crossRoadZone.transform.position - mother.transform.position).magnitude;
				if (5 > distFromCR)
					p.MoveNext (Command.toNextShot);
				else {
					// 1 : indicates that the mother is already moving
					if (1!=m.getState())
						m.moveTo (crossRoadZone.transform.position);
				}
			}
			break;
		case Shot.CrossRoads:
			print (" Story : Crossroads or Shot 3 warning timeout: "+warningTimeout + " end timeout: "+endTimeOut);
			//TODO: Only Checking for player 1 now, to add the coordinates for player 2
			if (!crossRoadExpl && 5 < (mother.transform.position - player1.transform.position).magnitude && 5==m.getState()) {
				// get the warning timer going. 
				if (0 < warningTimeout) { 
					warningTimeout -= Time.deltaTime;
					if (0 > warningTimeout)
						//warningCall(int w) : w=1 => intro Warning and w=2 => crossroads Warning
						m.warningCall (1);
					}
				//stubborn players still havent moved !! getting the end timer going. 
				else {
					endTimeOut -= Time.deltaTime;
					if (0 > endTimeOut)
						p.MoveNext (Command.toEnd);
				}
			} 
			// Players are close to the mother. Explain the cross roads.
			else {
				if (5 == m.getState () && !crossRoadExpl) {
					m.explainCrossRoads ();
					crossRoadExpl = true;
				}
				else if (5 == m.getState () && !crossRoadExpl) {
					// Move the mother to aonther location
				}
			}
			break;
		case Shot.End:
			print ("End of the Story !!");
			break;
		}
	}
}
