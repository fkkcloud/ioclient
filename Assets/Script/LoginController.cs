using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class LoginController : IOGameBehaviour {

	public Button BtnJoin;
	public InputField InputName;

	// Use this for initialization
	void Start () {
		BtnJoin.onClick.AddListener(OnClickPlayBtn);
	}

	public void OnClickPlayBtn ()
	{
		if(InputName.text != ""){
			Dictionary<string, string> data = new Dictionary<string, string> ();

			data ["name"]     = InputName.text;

			/*
				TODO: player position will be set from server-side
			*/
			Vector3 position = new Vector3 (0f, 0f, 0f);
			data ["position"] = position.x + "," + position.y + "," + position.z;

			SocketIOComp.Emit("SERVER:JOIN", new JSONObject(data));
		}
		else {
			InputName.text = "Please enter your name again";
		}
	}
}
