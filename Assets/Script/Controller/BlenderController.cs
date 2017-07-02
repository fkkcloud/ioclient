using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BlenderController : PlayerController {

	[HideInInspector]
	public Blender CharacterObject;

	public BlenderJoystickRotate JoystickRotate;

	float movement = 0f;

	public float MoveSpeed = 1f;

	public float SensitivityYaw = 20f;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (JoystickRotate && CharacterObject) 
		{
			// rotation //------------------------------------------------------------
			float yaw = JoystickRotate.Yaw ();
			if (yaw != 0f) {
				
				// reminder - Euler(pitch , yaw , roll)
				Vector3 newBodyRotation = JoystickRotate.BaseBodyRotation + new Vector3 (0f, yaw * SensitivityYaw, 0f);

				CharacterObject.transform.rotation = Quaternion.Euler (newBodyRotation);

				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["rotation"] = 0 + "," + newBodyRotation.y;
				data ["elapsedTime"] = Time.timeSinceLevelLoad.ToString();

				SocketIOComp.Emit("SERVER:ROTATE_BLENDER", new JSONObject(data));
			}

			// position //------------------------------------------------------------
			// TODO: have to get walking animation turn off for local character when they are stuck in the wall and not moving
			if (movement > 0f) {
				//Debug.Log (CharacterObject.transform.forward * MoveSpeed * movement * Time.deltaTime);
				CharacterObject.Rb.MovePosition(CharacterObject.Rb.position + CharacterObject.transform.forward * MoveSpeed * movement * Time.deltaTime);
				movement -= 1f * Time.deltaTime;

				Vector3 newPosition = CharacterObject.transform.position;

				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["position"] = newPosition.x + "," + newPosition.y + "," + newPosition.z;
				data ["elapsedTime"] = Time.timeSinceLevelLoad.ToString();

				//Debug.Log ("Attempting move:" + data["rotation"]);
				SocketIOComp.Emit ("SERVER:WALK_BLENDER", new JSONObject (data));
			} else {
				if (CharacterObject.Anim.GetBool("Walk") == true)
					CharacterObject.Anim.SetBool ("Walk", false);
			}

		}
	}

	public void Move(){
		
		movement = 2f;

		CharacterObject.Anim.SetBool ("Walk", true);
	}

	public void StopAction(){
		movement = 0f;
	}

	/*
	void OnDestroy(){
		if (GlobalGameState == null)
			return;
		GlobalGameState.BlenderJoytickRotate.gameObject.SetActive (false);
		GlobalGameState.BlenderJoytickWalk.gameObject.SetActive (false);
	}*/
}
