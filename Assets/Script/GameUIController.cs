using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : UIController {
	public Button LeaveChannelBtn;

	// Use this for initialization
	void Start () {
		LeaveChannelBtn.onClick.AddListener (LeaveChannel);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LeaveChannel(){
		PlayerControllerComp.State = PlayerController.PlayerState.Lobby;
		SocketIOComp.Emit ("SERVER:LEAVE");

		GlobalGameState.ClearScene ();

		GlobalGameState.LoginUI.Show ();

		Hide ();
	}
}
