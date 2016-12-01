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
	enum ELocations { CrossRoad=0, Cave1=1, Cave2=2, Cave3=3, Cave4=4, MothersDest=5 };
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

	//Players
	public GameObject player1=null;
	private GameObject player2=null;

	// This is a reference to the monster object
	private GameObject monster;
	// This is a reference to the baby object
	private GameObject baby;
	// This is a reference to the mother object
	private GameObject mother;

	private int monsterCave = -1;
	private int babyCave = -1;

	//Locations
	private Vector3[] locations = new Vector3[6];
	private bool[] cavesVisited = new bool[6];


	//An enumeration of the mother speeches 
	enum motherAudio { 
		Bad1=0, 
		Bad2=1, 
		Bad3=2,
		Intro=3,
		Timeout1=4,
		CrossRoads=5,
		Timeout2=6,
		BabyFound=7,
		Monster=8,
		ReUnite1=9,
		ReUnite2=10,
		ReUnite3=11
	};

	//step indicated where in the state are we
	private int step = 0;
	//dist: distance between mother and her destination
	private float dist=0;
	//end: indicates the type of ending it is. end => true : good ending i.e. baby found and vice versa
	private bool end = false;
	//variable which indicated that the story engine is listening to the events by the players.
	private bool startPlayerEvents = false;
	private bool babyFound =false;


	// This is the timeout for the warning call
	public float warningTimeout = 10.0f;
	// This is the timeout for Bad Ending 
	public float endTimeOut = 10.0f;
	// This is the time given to the users to find the baby
	public float gameTimeOut = 100.0f;


	// Use this for initialization
	void Start () {
		p = new Process();
		monster = GameObject.Find ("Monster");
		baby = GameObject.Find ("Baby");
		mother = GameObject.Find ("Mother");
		locations [(int)ELocations.CrossRoad] = GameObject.Find ("crossRoads").transform.position;
		locations [(int)ELocations.Cave1] = GameObject.Find ("cave1").transform.position;
		locations [(int)ELocations.Cave2] = GameObject.Find ("cave2").transform.position;
		locations [(int)ELocations.Cave3] = GameObject.Find ("cave3").transform.position;
		locations [(int)ELocations.Cave4] = GameObject.Find ("cave4").transform.position;
		locations [(int)ELocations.MothersDest] = GameObject.Find ("mothersDest").transform.position;
		for (int i = 0; i < cavesVisited.Length; i++)
			cavesVisited [i] = false;

	}

	// Update is called once per frame
	void Update () {
		MotherBehaviourScript m = mother.GetComponent<MotherBehaviourScript> (); 
		switch (p.CurrentShot) {
		case Shot.Start:
			//print (" Story : Start spawn, step: " + step + " mState: " + m.getState ());   
			//The mother is waiting
			if (5 == m.getState ()) {
				switch (step) {
				case 0:
					dist = (mother.transform.position - locations [(int)ELocations.Cave3]).magnitude;
					if (5 > dist)
						step++;
					else
						m.moveTo (locations [(int)ELocations.Cave3]);
					break;
				case 1:
					//if (true==getPlayerRefrences())
					p.MoveNext (Command.toNextShot);
					step = 0;
					break;
				}
			}
			break;
		case Shot.CaveIntro:
			print (" Story : Cave Intro or Shot 2, step: " + step + " mState: " + m.getState ());   
			// The mother is waiting asking her to introduce the cave
			if (5 == m.getState ()) {
				switch (step) {
				case 0:
					m.startTalking ((int)motherAudio.Intro);
					step++;
					break;
				case 1:
					dist = (mother.transform.position - locations [(int)ELocations.CrossRoad]).magnitude;
					if (5 > dist)
						step++;
					else
						m.moveTo (locations [(int)ELocations.CrossRoad]);
					break;
				case 2:
					p.MoveNext (Command.toNextShot);
					step = 0;
					break;
				}
			}
			break;
		case Shot.CrossRoads:
			print (" Story : Cave Expl or Shot 3, step: " + step + " mState: " + m.getState ());   
			switch (step) {
			case 0:
				//Check if Both Players are in cross roads with the mother 
				if (true == getPlayerRefrences ()) {
				//if (!crossRoadExpl && 5 < (mother.transform.position - player1.transform.position).magnitude
				//&& 5 < (mother.transform.position - player2.transform.position).magnitude && 5 == m.getState ()) {
					if (8 < (mother.transform.position - player1.transform.position).magnitude) {
						if (0 < warningTimeout) {
							print ("Warning time left: " + warningTimeout);						
							warningTimeout -= Time.deltaTime;
							//startTalking(int w) : motherAudio.Timeout1 => intro Warning and motherAudio.Timeout2 => crossroads Warning
							if (0 > warningTimeout)
								m.startTalking ((int)motherAudio.Timeout1);
						}
						//stubborn players still havent moved !! getting the end timer going. 
						else {
							print ("Ending time left : " + endTimeOut);
							endTimeOut -= Time.deltaTime;
							if (0 > endTimeOut) {
                                print("Alas ! the players did not participate moving to sad end !");
                                //end:: ending variable : true => good end, false => bad end
                                end = false;
								p.MoveNext (Command.toEnd);
							}
						}
					} else {
						//Players responded mother to explain the crossRoads !
						if (5 == m.getState ()) {
							//Explaining the cross roads
							m.startTalking ((int)motherAudio.CrossRoads);
							step++;
						}
					}
				}
				break;

			case 1:
				//Check if the cross road explanation is complete !!
				if (5 == m.getState ()) {
					m.moveTo (locations [(int)ELocations.MothersDest]);
					//Start listening to the player movements
					startPlayerEvents = true;
					dist = (mother.transform.position - locations [(int)ELocations.MothersDest]).magnitude;
					if (10 > dist)
						step++;
				}
				break;
				//Let the players find the baby
			case 2:
				//print ("Game Time left: " + gameTimeOut);
				gameTimeOut -= Time.deltaTime;
				if (0 > gameTimeOut) {
					end = false;
					p.MoveNext (Command.toEnd);
				}
				//if any of the caves is visited place the monster!
				if (cavesVisited [(int)ELocations.Cave1]) {
					monster.transform.position = locations [(int)ELocations.Cave1];
					monsterCave = 1;
					step++;
				} else if (cavesVisited [(int)ELocations.Cave2]) {
					monster.transform.position = locations [(int)ELocations.Cave2];
					monsterCave = 2;
					step++;
				} else if (cavesVisited [(int)ELocations.Cave4]) {
					monster.transform.position = locations [(int)ELocations.Cave4];
					monsterCave = 4;
					step++;
				}
				break;
				//place the baby
			case 3:
				gameTimeOut -= Time.deltaTime;
				if (0 > gameTimeOut) {
					end = false;
					p.MoveNext (Command.toEnd);
				}
				//if any of the caves is visited place the monster!
				if (cavesVisited [(int)ELocations.Cave1] && 1 != monsterCave) {
					baby.transform.position = locations [(int)ELocations.Cave1];
					babyCave = 1;
					step++;
				} else if (cavesVisited [(int)ELocations.Cave2] && 2 != monsterCave) {
					baby.transform.position = locations [(int)ELocations.Cave2];
					babyCave = 2;
					step++;
				} else if (cavesVisited [(int)ELocations.Cave4] && 4 != monsterCave) {
					baby.transform.position = locations [(int)ELocations.Cave4];
					babyCave = 4;
					step++;
				}
				break;
			case 4:
				gameTimeOut -= Time.deltaTime;
				if (0 > gameTimeOut) {
					end = false;
					p.MoveNext (Command.toEnd);
				}
				//baby found 
				if (babyFound) {
					end = true;
					GameObject baby = GameObject.Find ("Baby");
					m.moveTo (baby.transform.position);
					dist = (mother.transform.position - baby.transform.position).magnitude;
					if (10>dist)
						step++;
				}
				break;
				//Mother reached the babys cave
			case 5:
				if (5 == m.getState()) {
					m.startTalking ((int)motherAudio.BabyFound);
					step++;
				}
				break;
			case 6:
				if (5 == m.getState()) {
					p.MoveNext (Command.toEnd);
				}
				break;
			}
			break;
		case Shot.End:
			print ("End of the Story !!");
			GameObject endingType = new GameObject ();
			if (end)
				endingType.name = "goodEnd";
			else
				endingType.name = "badEnd";
			DontDestroyOnLoad (endingType);
			Application.LoadLevel(Application.loadedLevel + 1);
			break;
		}
	}


	//Helper function : returns true if it finds both the players and assignes them to player1 and player 2 variable else returns false
	bool getPlayerRefrences()	{
		player1 = GameObject.FindWithTag("FirstPlayer");
		//player2 = GameObject.FindWithTag("SecondPlayer");
		if (null != player1)// && null != player2)
			return true;
		else
			return false;
	}

	public void visitedCave(int caveIndex)	{
		if (startPlayerEvents)
			cavesVisited [caveIndex] = true;
	}

	public void setBabyFound(bool var)	{
		//set the baby found variable, invoked by player script
		babyFound = var;
	}
}
