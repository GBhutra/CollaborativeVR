using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryEngineScript : MonoBehaviour {
	/*
	 * Shots transition as per the followiwing state machine
	 * 															|‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|
	 * Start - IntroSlides - CaveIntro - CrossRoads - FindTheBaby - BabyFound - End
	 * 											|________________________________|
	 *
	 * The entry and exit conditions of individual cases are explained in the main update loop
	*/

	enum Shot { Start, IntroSlides, CaveIntro, CrossRoads, FindTheBaby, BabyFound, End };
	enum Command	  { toStart , toNextShot, toEnd };

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
				{ new StateTransition(Shot.Start, Command.toNextShot), Shot.IntroSlides },
				{ new StateTransition(Shot.IntroSlides, Command.toNextShot), Shot.CaveIntro },
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
	private CallSlideshow cs;

	// Use this for initialization
	void Start () {
		p = new Process();
		cs = (CallSlideshow)GameObject.Find ("Slideshow Controller").GetComponent (typeof(CallSlideshow));
	}
	
	// Update is called once per frame
	void Update () {
		switch (p.CurrentShot) {
		case Shot.Start:
			/*
			 * Entry : Both the players are connected
			 * Exit : end of the slideshow
			*/
			//CallSlideshow cs = (CallSlideshow)GameObject.Find ("Slideshow Controller").GetComponent (typeof(CallSlideshow));
			if (!cs.midfade && !cs.fadedOut && !cs.sceneComplete) {
				//extra first step to fade out objects
				StartCoroutine (cs.fadeEachOut (2.5F));
			}
			if (!cs.midfade && !cs.sceneComplete && cs.fadedOut) {
				//slides initially faded out, can play slides
				StartCoroutine (cs.startSlideshow ());
			}
			if (!cs.midfade && !cs.fadedOut && cs.sceneComplete) {
				//slideshow and title are done, transition
				print ("We are ready to leave shot 1");
			}

			//StartCoroutine (cs.fadeEachOut (2.5F));
			//if (
				
			//CallSlideshow.StartCoroutine(fadeEachOut(fadeTime));
			//CallSlideshow.

			break;
		}
	}
}
