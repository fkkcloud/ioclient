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

		if (State != PlayerState.Waiting && Input.anyKey) {
			float x = Input.GetAxis ("Vertical");
			float z = Input.GetAxis ("Horizontal");

			Dictionary<string, string> data = new Dictionary<string, string> ();

			Vector3 newPosition = PlayerObject.gameObject.transform.position + new Vector3(x, 0f, z);
			data ["position"] = newPosition.x + "," + newPosition.y + "," + newPosition.z;

			Debug.Log ("Attempting move:" + data);
			SocketIOComp.Emit("SERVER:MOVE", new JSONObject(data));
		}

	}
}
