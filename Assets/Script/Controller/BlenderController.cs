using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BlenderController : PlayerController {

	[HideInInspector]
	public Blender CharacterObject;

	public KillerJoystickMove Joystick;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	public void Move(){
		
	}
	
	// Update is called once per frame
	protected override void Update () {

		base.Update ();

		/*
		if (State != PlayerState.Lobby && !isOnChat) {
			
			float x = Joystick.Vertical ();
			float z = Joystick.Horizontal();

			float yaw = Camerastick.Yaw ();
			float pitch = Camerastick.Pitch ();

			bool IsDirtyPlayerPositionOnNetwork = false;
			bool IsDirtyPlayerRotationOnNetwork = false;

			//debug
			Vector3 forward = PlayerObject.gameObject.transform.forward * 2f;
			Debug.DrawRay(PlayerObject.gameObject.transform.position, forward, Color.green);

			// set player game object
			GameObject playerGameObject = PlayerObject.gameObject;

			// position //------------------------------------------------------------
			// default when there is no position input from player
			Vector3 newPosition = playerGameObject.transform.position;

			if (x != 0f || z != 0f) {
				newPosition = playerGameObject.transform.position
					+ playerGameObject.transform.forward * x * SensitivityX
					+ playerGameObject.transform.right * z * SensitivityZ;

				playerGameObject.transform.position = newPosition;
				IsDirtyPlayerPositionOnNetwork = true;
			}



			// rotation //------------------------------------------------------------
			// default when there is no rotation input from player
			Vector3 newBodyRotation = playerGameObject.transform.rotation.eulerAngles; 
			Vector3 newHeadRotation = PlayerObject.HeadTransform.localRotation.eulerAngles;

			if (yaw != 0f || pitch != 0f) {
				
				// reminder - Euler(pitch , yaw , roll)
				newBodyRotation = Camerastick.BaseBodyRotation + new Vector3 (0f, yaw * SensitivityYaw, 0f);
				newHeadRotation = Camerastick.BaseHeadLocalRotation + new Vector3 (pitch * SensitivityPitch, 0f, 0f);

				playerGameObject.transform.rotation = Quaternion.Euler (newBodyRotation);
				PlayerObject.HeadTransform.localRotation = Quaternion.Euler (newHeadRotation);

				IsDirtyPlayerRotationOnNetwork = true;
			}



			// network //------------------------------------------------------------
			if (IsDirtyPlayerPositionOnNetwork || IsDirtyPlayerRotationOnNetwork) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["position"] = newPosition.x     + "," + newPosition.y      + "," + newPosition.z;
				data ["rotation"] = newHeadRotation.x + "," + newBodyRotation.y;

				//Debug.Log ("Attempting move:" + data["rotation"]);
				SocketIOComp.Emit("SERVER:MOVE", new JSONObject(data));
			}

		}
*/
	}
}
