using UnityEngine;
using UnityEngine.VR;
using System.Collections;


public class GetCameraValues : MonoBehaviour
{
    enum RotationMode { Distrator, GuidedRotation, SnapToTarget, Free };
    RotationMode selectedMode = RotationMode.GuidedRotation;
    public Transform target;
    public GameObject distractor;
    //public Camera cam = Camera.main;
    public float rotationRate = 0.5f;
    private float time = 3.0f;

    //This is a trigger for the distractions to start or stop
    bool b_ongoingStory = false;

    // Use this for initialization  
    void Start()
    {
        target = GameObject.Find("Mother").transform;
        if ("FirstPlayer" == gameObject.tag)    
            distractor = GameObject.Find("fireflyPlayer1");
        else if ("SecondPlayer" == gameObject.tag)
            distractor = GameObject.Find("fireflyPlayer2");
    }

    // Update is called once per frame
    void Update()
    {
        //print(" Is the story ongoing ? : " + b_ongoingStory);
        // Angle betwween the target scene and the viewer
        Vector3 localTarget = Camera.main.transform.InverseTransformPoint(target.position);
        //Vector3 localTarget = Camera.main.transform.InverseTransformPoint(target.position);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        //Debug.Log(targetAngle);

        Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        //Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0.49 && screenPoint.x < 0.51 && screenPoint.y > 0 && screenPoint.y < 1;
        bool aligned = true;

        if (!(targetAngle > -5 && targetAngle < 5))
        {
            aligned = false;
        }

        if (b_ongoingStory)
        {
            // temporarily its timed simulating the story element
            time -= Time.deltaTime;
            if (0 > time)
            { 
                b_ongoingStory = false;
                if (RotationMode.Distrator == selectedMode)
                    distractor.SetActive(false);
            }
            switch (selectedMode)
            {
                case (RotationMode.Distrator):
                    DistractorFunction(targetAngle, aligned, onScreen);
                    break;
                case (RotationMode.GuidedRotation):
                    GuidedRotationFunction(targetAngle, aligned, onScreen);
                    break;
                case (RotationMode.SnapToTarget):
                    SnapToTargetFunction(targetAngle);
                    break;
                default:
                    //Free Rotation Do Nothing
                    break;
            }
        }

        if (Input.GetKey(KeyCode.U))
        {
            target.gameObject.GetComponent<AudioSource>().Play();
        }

        if (Input.GetKey(KeyCode.P))
        {
            b_ongoingStory = true;
            time = 3f;
        }
    }

    // Redirection based on gaze of the viewer.
    void GuidedRotationFunction(float targetAngle, bool aligned, bool onScreen)
    {
        if (!aligned)
        {
            if (!onScreen)
            {
                if (targetAngle > 0)
                {
                    transform.Rotate(Vector3.up, rotationRate);
                }
                else
                {
                    transform.Rotate(Vector3.up, rotationRate * (-1));
                }
            }
            else
            {
                aligned = true;
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
    void DistractorFunction(float targetAngle, bool aligned, bool onScreen)
    {
        if (!onScreen)
        {
            //Move the distractor 20 units away from where the camera is looking at
            distractor.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            //Initialize the distractor
            distractor.SetActive(true);
        }
        else
        {
            distractor.SetActive(false);
        }
    }

    public void setStoryMode(bool val)
    {
        b_ongoingStory = val;
    }

}
