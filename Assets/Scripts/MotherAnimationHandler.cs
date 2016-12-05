using UnityEngine;
using System.Collections;

public class MotherAnimationHandler : MonoBehaviour
{
    Animator anim;
    AnimatorStateInfo stateInfo;

    //trigger hashes, sorted by starting state
    //idle sourced triggers
    int idleToRun = Animator.StringToHash("Idle-Running");
    int idleToTurn = Animator.StringToHash("Idle-180Turn");
    int idleToAngry = Animator.StringToHash("Idle-Angry");
    int idleToExplain = Animator.StringToHash("Idle-Explaining");

    //running sourced triggers
    int runToIdle = Animator.StringToHash("Running-Idle");
    int runToTurn = Animator.StringToHash("Running-180Turn");
    int runToExplain = Animator.StringToHash("Running-Explaining");
    int runToPickUpBaby = Animator.StringToHash("Running-PickUpBaby");
    int runToExit = Animator.StringToHash("Running-Exit"); //not sure if needed?

    //180 turn sourced triggers
    int turnToRun = Animator.StringToHash("180Turn-Running");
    int turnToIdle = Animator.StringToHash("180Turn-Idle");

    //explaining sourced triggers
    int explainToRun = Animator.StringToHash("Explaining-Running");
    int explainToIdle = Animator.StringToHash("Explaining-Idle");
    int explainToTurn = Animator.StringToHash("Explaining-180Turn");

    //angry talking sourced triggers
    int angryToRun = Animator.StringToHash("AngryTalking-Running");
    int angryToIdle = Animator.StringToHash("AngryTalking-Idle");

    //pickupbaby sourced triggers
    int pickUpBabyToStandWBaby = Animator.StringToHash("PickUpBaby-StandWithBaby");

    //standwithbaby sourced triggers
    int standWBabyToWalkWBaby = Animator.StringToHash("StandWithBaby-WalkWithBaby");

    //walkwithbaby sourced triggers
    int walkWBabyToStandWBaby = Animator.StringToHash("WalkWithBaby-StandWithBaby");
    int walkWBabyToExit = Animator.StringToHash("WalkWithBaby-Exit");

    //state hashes
    int idleStateHash = Animator.StringToHash("Base Layer.Idle");
    int runningStateHash = Animator.StringToHash("Base Layer.Running");
    int turningStateHash = Animator.StringToHash("Base Layer.180Turn");
    int explainingStateHash = Animator.StringToHash("Base Layer.Explaining");
    int angryStateHash = Animator.StringToHash("Base Layer.AngryTalking");
    int pickUpBabyStateHash = Animator.StringToHash("Base Layer.PickUpBaby");
    int standWBabyStateHash = Animator.StringToHash("Base Layer.StandWithBaby");
    int walkWBabyStateHash = Animator.StringToHash("Base Layer.WalkWithBaby");

    /// <summary>
    /// Trigger a transition to Idle State. Can come from Running, Turning, Angry, or Explaining states.
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToIdle()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //run
        if (stateInfo.fullPathHash == runningStateHash)
        {
            anim.SetTrigger(runToIdle);
            return "running to idle";
        }
        //turn
        else if (stateInfo.fullPathHash == turningStateHash)
        {
            anim.SetTrigger(turnToIdle);
            return "turning to idle";
        }
        //angry
        else if (stateInfo.fullPathHash == angryStateHash)
        {
            anim.SetTrigger(angryToIdle);
            return "angry to idle";
        }
        //explaining
        else if (stateInfo.fullPathHash == explainingStateHash)
        {
            anim.SetTrigger(explainToIdle);
            return "explaining to idle";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to Running State. Can come from Idle, Turning, Angry, or Explaining states.
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToRun()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //idle
        if (stateInfo.fullPathHash == idleStateHash)
        {
            anim.SetTrigger(idleToRun);
            return "idle to run";
        }
        //turn
        else if (stateInfo.fullPathHash == turningStateHash)
        {
            anim.SetTrigger(turnToRun);
            return "turning to run";
        }
        //angry
        else if (stateInfo.fullPathHash == angryStateHash)
        {
            anim.SetTrigger(angryToRun);
            return "angry to run";
        }
        //explaining
        else if (stateInfo.fullPathHash == explainingStateHash)
        {
            anim.SetTrigger(explainToRun);
            return "explaining to run";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to Turning State. Can come from Idle, Running, or Explaining states.
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToTurn()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //idle
        if (stateInfo.fullPathHash == idleStateHash)
        {
            anim.SetTrigger(idleToTurn);
            return "idle to turning";
        }
        //run
        else if (stateInfo.fullPathHash == runningStateHash)
        {
            anim.SetTrigger(runToTurn);
            return "running to turning";
        }
        //explaining
        else if (stateInfo.fullPathHash == explainingStateHash)
        {
            anim.SetTrigger(explainToTurn);
            return "explaining to turning";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to Explaining State. 
    /// Can come from Idle or Running states
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToExplain()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //idle
        if (stateInfo.fullPathHash == idleStateHash)
        {
            anim.SetTrigger(idleToExplain);
            return "idle to explaining";
        }
        //run
        else if (stateInfo.fullPathHash == runningStateHash)
        {
            anim.SetTrigger(runToExplain);
            return "running to explaining";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to Angry Talking State. 
    /// Can come from Idle state
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToAngry()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //idle
        if (stateInfo.fullPathHash == idleStateHash)
        {
            anim.SetTrigger(idleToAngry);
            return "idle to angry";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to PickUpBaby State. 
    /// Can come from Running state
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToPickUpBaby()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //running
        if (stateInfo.fullPathHash == runningStateHash)
        {
            anim.SetTrigger(runToPickUpBaby);
            return "running to pickUpBaby";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to StandWBaby State. 
    /// Can come from PickUpBaby and WalkWBaby states
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToStandWithBaby()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //pickUpBaby
        if (stateInfo.fullPathHash == pickUpBabyStateHash)
        {
            anim.SetTrigger(pickUpBabyToStandWBaby);
            return "pickUpBaby to standWBaby";
        }
        //walkWBaby
        else if (stateInfo.fullPathHash == walkWBabyStateHash)
        {
            anim.SetTrigger(walkWBabyToStandWBaby);
            return "walkWBaby to standWBaby";
        }
        return "cannot transition from this state";
    }

    /// <summary>
    /// Trigger a transition to WalkWBaby State. 
    /// Can come from StandWBaby state
    /// </summary>
    /// <returns>
    /// Description of state transition, if applicable
    /// </returns>
    public string switchToWalkWithBaby()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //standWBaby
        if (stateInfo.fullPathHash == standWBabyStateHash)
        {
            anim.SetTrigger(standWBabyToWalkWBaby);
            return "standWBaby to walkWBaby";
        }
        return "cannot transition from this state";
    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        string output;
        /*
        //call to run
        if (Input.GetKeyDown(KeyCode.H))
        {
            output = switchToRun();
            Debug.Log(output);
        }
        //call to idle
        else if (Input.GetKeyDown(KeyCode.J))
        {
            output = switchToIdle();
            Debug.Log(output);
        }
        //call to turn
        else if (Input.GetKeyDown(KeyCode.K))
        {
            output = switchToTurn();
            Debug.Log(output);
        }
        //call to explain
        else if (Input.GetKeyDown(KeyCode.L))
        {
            output = switchToExplain();
            Debug.Log(output);
        }
        //call to angry
        else if (Input.GetKeyDown(KeyCode.V))
        {
            output = switchToAngry();
            Debug.Log(output);
        }
        //call to pickUpBaby
        else if (Input.GetKeyDown(KeyCode.B))
        {
            output = switchToPickUpBaby();
            Debug.Log(output);
        }
        //call to standWithBaby
        else if (Input.GetKeyDown(KeyCode.N))
        {
            output = switchToStandWithBaby();
            Debug.Log(output);
        }
        //call to walkWithBaby
        else if (Input.GetKeyDown(KeyCode.M))
        {
            output = switchToWalkWithBaby();
            Debug.Log(output);
        }
        */

    }
}