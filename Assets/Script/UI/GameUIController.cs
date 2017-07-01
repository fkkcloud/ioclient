using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : UIController {
	public Button LeaveChannelBtn;

	// Use this for initialization
	void Start () {
		LeaveChannelBtn.onClick.AddListener (LeaveGame);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LeaveGame(){
		SocketIOComp.Emit ("SERVER:LEAVE_GAME");

		//GlobalGameState.Disconnect ();
		GlobalGameState.HandleLeaveGame();

		Hide ();
		GlobalGameState.HideAllUI ();
		GlobalGameState.LobbyUI.ResetLobbyState ();
		GlobalGameState.LobbyUI.Show ();
		GlobalGameState.GameUI.Show ();

		// set spectate cam on!
		GlobalGameState.SpectateCam.gameObject.SetActive (true);
	}
}
