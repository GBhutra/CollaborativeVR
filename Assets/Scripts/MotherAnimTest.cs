using UnityEngine;
using System.Collections;

public class MotherAnimTest : MonoBehaviour {
    Animator anim;

    int idleToRun = Animator.StringToHash("Idle-Running");
    int runToIdle = Animator.StringToHash("Running-Idle");
    int idleStateHash = Animator.StringToHash("Base Layer.Idle");
    int runningStateHash = Animator.StringToHash("Base Layer.Running");

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        //assign arbitrary button to call animations
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (stateInfo.fullPathHash == runningStateHash)
            {
                anim.SetTrigger(runToIdle);
            }
            else if (stateInfo.fullPathHash == idleStateHash)
            {
                anim.SetTrigger(idleToRun);
            }
        }
	}
}
