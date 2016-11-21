using UnityEngine;
using System.Collections;

public class TagPlayers : MonoBehaviour
{

    private bool foundFirstPlayer = false;
    private bool foundSecondPlayer = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (!foundFirstPlayer)
        {

            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                GameObject firstPlayer = GameObject.FindGameObjectWithTag("Player");
                firstPlayer.gameObject.tag = "FirstPlayer";
                foundFirstPlayer = true;
            }
        }

        if (!foundSecondPlayer && foundFirstPlayer)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                GameObject secondPlayer = GameObject.FindGameObjectWithTag("Player");
                secondPlayer.gameObject.tag = "SecondPlayer";
                foundSecondPlayer = true;
            }

        }
    }
}
