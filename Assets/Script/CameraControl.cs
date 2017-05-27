using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : IOGameBehaviour {

	Player UserPlayer;

	public float DistOffsetMult = 12f;

	float OriginalY;

	// Use this for initialization
	void Start () {
		OriginalY = transform.position.y;
	}

	public void Reset() {
		Vector3 camPos = transform.position;
		camPos.y = OriginalY;
		transform.position = camPos;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if (!UserPlayer) {
			if (PlayerControllerComp) {
				UserPlayer = PlayerControllerComp.PlayerObject;
			}
			return;
		}

		Vector3 velocity = Vector3.zero;
		Vector3 forward = (UserPlayer.transform.position - transform.position).normalized * DistOffsetMult;
		Vector3 needPos = UserPlayer.transform.position - forward;
		needPos.y = transform.position.y;
		transform.position = Vector3.SmoothDamp(transform.position, needPos,
			ref velocity, 0.05f);
		transform.LookAt (UserPlayer.transform);


	}
}
