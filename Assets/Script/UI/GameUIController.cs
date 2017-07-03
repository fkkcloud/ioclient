using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : UIController {

	public enum GameUIState {Lobby, InGame};
	public Button LeaveChannelBtn;
	public Button LeaveRoomBtn;

	// Use this for initialization
	void Start () {
		LeaveChannelBtn.onClick.AddListener (LeaveGame);
		LeaveRoomBtn.onClick.AddListener (LeaveRoom);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowWithOption(GameUIState i){
		base.Show ();

		if (i == GameUIState.InGame) {
			LeaveChannelBtn.gameObject.SetActive (true);
			LeaveRoomBtn.gameObject.SetActive (false);
		} else if (i == GameUIState.Lobby) {
			LeaveChannelBtn.gameObject.SetActive (false);
			LeaveRoomBtn.gameObject.SetActive (true);
		}
	}

	public void LeaveGame(){
		bool IsByPlayerWill = true;
		GlobalGameState.LeaveGame (IsByPlayerWill);
	}

	public void LeaveRoom(){
		GlobalGameState.LeaveRoom ();
	}
}
