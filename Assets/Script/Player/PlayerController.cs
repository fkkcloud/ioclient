using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class PlayerController : IOGameBehaviour {

	[HideInInspector]
	public Player PlayerObject;

	[HideInInspector]
	public bool isOnChat = false;

	public VirtualJoystick Joystick;

	public enum PlayerState
	{
		Lobby,
		Joined,
		Playing
	}

	public PlayerState State = PlayerState.Lobby;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (State != PlayerState.Lobby && !isOnChat) {
			
			float x = Joystick.Vertical (); //Input.GetAxis ("Vertical");
			float z = Joystick.Horizontal();

			if (x == 0f && z == 0f)
				return;

			GameObject playerGameObject = PlayerObject.gameObject;

			Vector3 newPosition = playerGameObject.transform.position + playerGameObject.transform.forward * x * 0.086f;
			playerGameObject.transform.position = newPosition;

			Vector3 newRotation = playerGameObject.transform.rotation.eulerAngles + new Vector3 (0f, z, 0f);
			playerGameObject.transform.rotation = Quaternion.Euler (newRotation);

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["position"] = newPosition.x + "," + newPosition.y + "," + newPosition.z;
			data ["rotation"] = newRotation.x + "," + newRotation.y + "," + newRotation.z;

			//Debug.Log ("Attempting move:" + data["rotation"]);
			SocketIOComp.Emit("SERVER:MOVE", new JSONObject(data));
		}

	}
}
