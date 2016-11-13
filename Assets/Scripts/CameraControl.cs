using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public enum RotationMode { Distrator, GuidedRotation, SnapToTarget ,Free };

public class CameraControl : MonoBehaviour
{
    RotationMode selectedMode =  RotationMode.Distrator;
    public Transform player;
    public Transform target;
    public GameObject distractor;
    public Camera cam = Camera.main;
    public float rotationRate = 0.5f;
    public bool b_ongoingStory = false;

    // Use this for initialization
    void Start()
    {
        target = GameObject.Find("Rock2").transform;
        player = GameObject.Find("FirstPersonCharacter").transform;
        distractor = GameObject.Find("firefly");
    }

    // Update is called once per frame
    void Update() {
        // Angle betwween the target scene and the viewer
        Vector3 localTarget = cam.transform.InverseTransformPoint(target.position);
        //Vector3 localTarget = Camera.main.transform.InverseTransformPoint(target.position);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        //Debug.Log(targetAngle);

        Vector3 screenPoint = cam.WorldToViewportPoint(target.position);
        //Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0.49 && screenPoint.x < 0.51 && screenPoint.y > 0 && screenPoint.y < 1;
        bool aligned = true;

        if (!(targetAngle > -5 && targetAngle < 5)) {
            aligned = false;
        }

        if (b_ongoingStory)
        { 
            switch (selectedMode) {
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

        if (Input.GetKey(KeyCode.U))    {
            target.gameObject.GetComponent<AudioSource>().Play();
        }

        if (Input.GetKey(KeyCode.P))
        {
            b_ongoingStory = !b_ongoingStory;
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
                    player.transform.Rotate(Vector3.up, rotationRate);
                }
                else
                {
                    player.transform.Rotate(Vector3.up, rotationRate * (-1));
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
        player.transform.Rotate(Vector3.up, targetAngle);
    }

    // Visual distractor based Redirection
    void DistractorFunction(float targetAngle, bool aligned, bool onScreen)
    {
        if (!aligned)
        {
            if (!onScreen)
            {
                //Move the distractor 20 units away from where the camera is looking at
                distractor.transform.position = player.transform.position + cam.transform.forward * 20f;
                //Initialize the distractor
                distractor.SetActive(true);
            }
        }
    }
}
