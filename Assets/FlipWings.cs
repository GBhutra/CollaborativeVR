using UnityEngine;
using System.Collections;

public class FlipWings : MonoBehaviour {

    public Transform wing1;
    public Transform wing2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        wing1.transform.localRotation = Quaternion.Euler(wing1.localRotation.eulerAngles.x + 40,wing1.localRotation.eulerAngles.y, wing1.localRotation.eulerAngles.z);
        wing2.transform.localRotation = Quaternion.Euler(wing2.localRotation.eulerAngles.x - 40, wing2.localRotation.eulerAngles.y, wing2.localRotation.eulerAngles.z);
    }
}
