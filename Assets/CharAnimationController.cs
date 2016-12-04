using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CharAnimationController : MonoBehaviour {

    static Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame

    void Update()
    {
        Vector2 input = GetInput();
        if (input.y > 0 )
        {
            anim.SetBool("IsWalkingForward", true);
           
        }
          
        else if (input.y <0)
        {
            anim.SetBool("IsWalkingBackward", true);
            
        }
        else
        {
            anim.SetBool("IsWalkingForward", false);
            anim.SetBool("IsWalkingBackward", false);
        }
    }

    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = CrossPlatformInputManager.GetAxis("Horizontal"),
            y = CrossPlatformInputManager.GetAxis("Vertical")
        };
        
        return input;
    }
}
