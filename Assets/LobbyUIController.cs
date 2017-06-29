using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LobbyUIController : UIController {

	public Button JoinGameBtn;
	public Button LeaveGameBtn;


	// Use this for initialization
	void Start () {
		JoinGameBtn.onClick.AddListener(OnClickReadyBtn);
		LeaveGameBtn.onClick.AddListener(OnClickReadyCancelBtn);
	}

	void OnClickReadyBtn(){
		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["ready"] = "1";

		SocketIOComp.Emit ("SERVER:USER_READY", new JSONObject (data));
	}

	void OnClickReadyCancelBtn(){
		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["ready"] = "0";

		SocketIOComp.Emit ("SERVER:USER_READY", new JSONObject (data));
	}
}
