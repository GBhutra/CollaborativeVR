using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {
	public float speed = 6.0F;
	private Vector3 moveDirection = Vector3.zero;
	private GameObject player;

	void Start(){
		player = this.gameObject;
	}

	void Update() {
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		//moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;

		player.transform.position += moveDirection;
		if (player.transform.position.y < 1 ) {
			player.transform.position = new Vector3(player.transform.position.x, 1, player.transform.position.z);
		}
	}
}
