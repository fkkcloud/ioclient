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
	public CameraJoystick Camerastick;

	public float SensitivityX = 1f;
	public float SensitivityZ = 1f;


	public float SensitivityYaw = 20f;
	public float SensitivityPitch = 20f;

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
			
			float x = Joystick.Vertical ();
			float z = Joystick.Horizontal();

			float yaw = Camerastick.Yaw ();
			float pitch = Camerastick.Pitch ();

			//debug
			Vector3 forward = PlayerObject.gameObject.transform.forward * 2f;
			Debug.DrawRay(PlayerObject.gameObject.transform.position, forward, Color.green);

			GameObject playerGameObject = PlayerObject.gameObject;
			Vector3 newPosition = Vector3.zero;

			if (x != 0f || z != 0f) {
				// position //------------------------------------------------------------
				newPosition = playerGameObject.transform.position
					+ playerGameObject.transform.forward * x * SensitivityX
					+ playerGameObject.transform.right * z * SensitivityZ;

				playerGameObject.transform.position = newPosition;
			}

			Vector3 newBodyRotation = Vector3.zero;
			if (yaw != 0f || pitch != 0f) {
				// rotation //------------------------------------------------------------
				// reminder - Euler(pitch , yaw , roll)
				newBodyRotation = Camerastick.BaseBodyRotation + new Vector3 (0f, yaw * SensitivityYaw, 0f);
				Vector3 newHeadRotation = Camerastick.BaseHeadLocalRotation + new Vector3 (pitch * SensitivityPitch, 0f, 0f);

				playerGameObject.transform.rotation = Quaternion.Euler (newBodyRotation);
				PlayerObject.HeadTransform.localRotation = Quaternion.Euler (newHeadRotation);
			}

			// network //------------------------------------------------------------
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["position"] = newPosition.x + "," + newPosition.y + "," + newPosition.z;
			data ["rotation"] = newBodyRotation.x + "," + newBodyRotation.y + "," + newBodyRotation.z;

			//Debug.Log ("Attempting move:" + data["rotation"]);
			SocketIOComp.Emit("SERVER:MOVE", new JSONObject(data));
		}

	}
}
