using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LobbyUIController : UIController {

	public Button JoinGameBtn;
	public Button LeaveGameBtn;
	public Button LeaveRoomBrn;


	// Use this for initialization
	void Start () {
		JoinGameBtn.onClick.AddListener(OnClickReadyBtn);
		LeaveGameBtn.onClick.AddListener(OnClickReadyCancelBtn);
	}

	void OnClickReadyBtn(){
		GlobalGameState.IsPlayerReady = true;

		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["ready"] = "1";

		SocketIOComp.Emit ("SERVER:USER_READY", new JSONObject (data));
		LeaveGameBtn.gameObject.SetActive (true);
		JoinGameBtn.gameObject.SetActive (false);
	}

	void OnClickReadyCancelBtn(){
		ResetLobbyState ();
	}

	public override void Show(){
		base.Show ();

		if (!GlobalGameState.IsPlayerReady) {
			LeaveGameBtn.gameObject.SetActive (false);
			JoinGameBtn.gameObject.SetActive (true);
		} else {
			LeaveGameBtn.gameObject.SetActive (true);
			JoinGameBtn.gameObject.SetActive (false);
		}
	}

	public void ResetLobbyState(){
		GlobalGameState.IsPlayerReady = false;

		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["ready"] = "0";

		SocketIOComp.Emit ("SERVER:USER_READY", new JSONObject (data));
		LeaveGameBtn.gameObject.SetActive (false);
		JoinGameBtn.gameObject.SetActive (true);
	}
}
