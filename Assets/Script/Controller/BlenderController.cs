using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BlenderController : PlayerController {

	[HideInInspector]
	public Blender CharacterObject;

	public BlenderJoystickWalk JoystickMove;
	public BlenderJoystickRotate JoystickRotate;

	public float MoveSpeed = 1f;

	public float SensitivityYaw = 20f;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (JoystickMove && JoystickRotate) 
		{
			float yaw = JoystickRotate.Yaw ();

			bool IsDirtyPlayerRotationOnNetwork = false;

			// set player game object

			// rotation //------------------------------------------------------------
			// default when there is no rotation input from player
			Vector3 newBodyRotation = CharacterObject.transform.rotation.eulerAngles; 

			if (yaw != 0f) {

				// reminder - Euler(pitch , yaw , roll)
				newBodyRotation = JoystickRotate.BaseBodyRotation + new Vector3 (0f, yaw * SensitivityYaw, 0f);

				CharacterObject.transform.rotation = Quaternion.Euler (newBodyRotation);

				IsDirtyPlayerRotationOnNetwork = true;
			}

			// network //------------------------------------------------------------
			if (IsDirtyPlayerRotationOnNetwork) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["rotation"] = 0 + "," + newBodyRotation.y;

				SocketIOComp.Emit("SERVER:ROTATE_BLENDER", new JSONObject(data));
			}
		}
	}

	public void Move(){
		
		// set player game object
		GameObject playerGameObject = CharacterObject.gameObject;

		// position //------------------------------------------------------------
		// default when there is no position input from player
		Vector3 newPosition = playerGameObject.transform.position + playerGameObject.transform.forward * MoveSpeed;
		
		CharacterObject.simulatedEndPos = newPosition;


		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["position"] = newPosition.x     + "," + newPosition.y      + "," + newPosition.z;

		//Debug.Log ("Attempting move:" + data["rotation"]);
		SocketIOComp.Emit("SERVER:WALK_BLENDER", new JSONObject(data));

	}
	

}
