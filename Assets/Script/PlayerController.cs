using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class PlayerController : IOGameBehaviour {

	[HideInInspector]
	public Player PlayerObject;

	public enum PlayerState
	{
		Waiting,
		Joined,
		Playing
	}

	public PlayerState State = PlayerState.Waiting;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (State != PlayerState.Waiting) {
			
			float x = Input.GetAxis ("Vertical");
			float z = Input.GetAxis ("Horizontal");

			if (x == 0f && z == 0f)
				return;

			Vector3 newPosition = PlayerObject.gameObject.transform.position + new Vector3(x * Time.deltaTime * 16f, 0f, z * Time.deltaTime * 16f);
			PlayerObject.gameObject.transform.position = newPosition;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["position"] = newPosition.x + "," + newPosition.y + "," + newPosition.z;

			Debug.Log ("Attempting move:" + data["position"]);
			SocketIOComp.Emit("SERVER:MOVE", new JSONObject(data));
		}

	}
}
