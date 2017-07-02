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

	public void LeaveGame(){
		bool IsByPlayerWill = true;
		GlobalGameState.LeaveGame (IsByPlayerWill);
	}
}
