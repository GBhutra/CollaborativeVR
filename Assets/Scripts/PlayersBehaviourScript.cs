using UnityEngine;
using System.Collections;

public class PlayersBehaviourScript : MonoBehaviour
{
    enum RotationMode { Distractor, GuidedRotation, SnapToTarget, Free };
    enum Gate { Cave1 = 1, Cave2 = 2, Cave3 = 3, Cave4 = 4, Loop = 5 };

    //inGate : keeps track of which cave the player is in. Cave3 => Spawning cave
    private int inGate = (int)Gate.Cave3;

    private GameObject baby;

    private RotationMode selectedMode = RotationMode.Distractor;

    private Transform target;
    private GameObject distractor;
    private GameObject cam;
    private int MonsterGateIndex = -1;
    private int BabyGateIndex = -1;

    public float rotationRate = 0.25f;

    private bool b_Distracted = false;
    private bool babyFound = false;
    //This is a boolean indicates that the mother is talking
    bool b_ongoingStory = false;

    //Trigger When the player passes through a gate
    void OnTriggerEnter(Collider other)
    {
        GameObject cave = GameObject.Find("Cave");
        StoryEngineScript s = cave.GetComponent<StoryEngineScript>();
        switch (other.gameObject.name)
        {
            case "caveGate1":
                inGate = (int)Gate.Cave1;
                s.visitedCave(1);
                break;
            case "caveGate2":
                inGate = (int)Gate.Cave2;
                s.visitedCave(2);
                break;
            case "caveGate3":
                inGate = (int)Gate.Cave3;
                break;
            case "caveGate4":
                inGate = (int)Gate.Cave4;
                s.visitedCave(4);
                break;
            case "caveLoopGate1":
            case "caveLoopGate2":
                inGate = (int)Gate.Loop;
                s.visitedCave(3);
                break;
        }
        if (inGate==MonsterGateIndex)
        {
            GameObject.Find("Monster").GetComponentInChildren<AudioSource>().Play();
        }
        if (inGate==BabyGateIndex)
        {
            baby.GetComponentInChildren<AudioSource>().Play();
        }

    }
    // Use this for initialization  
    void Start()
    {
        cam = transform.FindChild("MainCamera").gameObject;
        target = GameObject.Find("Mother").transform;
        baby = GameObject.Find("Baby");
    }

    // Update is called once per frame
    void Update()
    {
        if (b_ongoingStory)
        {
            Vector3 localTarget = cam.transform.InverseTransformPoint(target.position);
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            print("Player to Target to angle :" + targetAngle);
            switch (selectedMode)
            {
                case (RotationMode.Distractor):
                    DistractorFunction(targetAngle);
                    break;
                case (RotationMode.GuidedRotation):
                    GuidedRotationFunction(targetAngle);
                    break;
                case (RotationMode.SnapToTarget):
                    SnapToTargetFunction(targetAngle);
                    break;
                default:
                    //Free Rotation Do Nothing
                    break;
            }
        }
        if (!babyFound)
        {
            if (5 > (baby.transform.position - transform.position).magnitude)
            {
                GameObject cave = GameObject.Find("Cave");
                StoryEngineScript s = cave.GetComponent<StoryEngineScript>();
                s.setBabyFound(true);
                babyFound = true;
            }
        }
    }

    // Redirection based on gaze of the viewer.
    void GuidedRotationFunction(float targetAngle)
    {
        if (!b_Distracted)
        {
            if (10 < targetAngle)
            {
                transform.Rotate(Vector3.up, rotationRate);

            }
            else if (-10 > targetAngle)
            {
                transform.Rotate(Vector3.up, rotationRate * (-1));
            }
            else
            {
                b_Distracted = true;
            }
        }
    }

    // Fade in and Fade out Redirection
    void SnapToTargetFunction(float targetAngle)
    {
        //TODO : Add a fade in fade out sequence
        transform.Rotate(Vector3.up, targetAngle);
    }

    // Visual distractor based Redirection
    void DistractorFunction(float targetAngle)
    {
        GameObject dist = GameObject.Find("Distractors");
        if ("FirstPlayer" == gameObject.tag)
            distractor = dist.transform.FindChild("fireflyPlayer1").gameObject;
        else if ("SecondPlayer" == gameObject.tag)
            distractor = dist.transform.FindChild("fireflyPlayer2").gameObject;
        if (25 < targetAngle || -25 > targetAngle)
        {
            //Move the distractor 20 units away from where the camera is looking at
            //distractor.transform.position = cam.transform.position + cam.transform.forward * 0.8f;
            //Initialize the distractor
            if (!distractor.activeSelf)
                distractor.SetActive(true);
        }
        /*else
        {
            if (distractor.activeSelf)
                distractor.SetActive(false);
        }*/
    }

    public void setStoryMode(bool val)
    {
        b_Distracted = false;
        if (false == val && distractor!= null)
            if (distractor.activeSelf)
                distractor.SetActive(false);
        print("Setting the  StoryMode in Player :" + val);
        b_ongoingStory = val;

        if (RotationMode.GuidedRotation==selectedMode || RotationMode.SnapToTarget==selectedMode)
        {
            setPlayerLocationLock(val);
        }
    }

    public void setPlayerLocationLock(bool val)
    {
        gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().enabled = !val;
    }

    public void setMonsterGateIndex(int val)
    {
        MonsterGateIndex = val;
    }
    public void setBabyGateIndex(int val)
    {
        BabyGateIndex = val;
    }
}

