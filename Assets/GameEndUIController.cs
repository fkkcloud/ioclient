using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUIController : UIController {

	public Text GameEndUIText;

	public void SetState(GameState.WinSide winSide){
		if (winSide == GameState.WinSide.Blender) {
			GameEndUIText.text = "Blender Win";
		} else if (winSide == GameState.WinSide.Killer) {
			GameEndUIText.text = "Killer Win";
		} else if (winSide == GameState.WinSide.Neither) {
			GameEndUIText.text = "Game got disrupted";
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
