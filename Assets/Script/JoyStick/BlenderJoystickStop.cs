using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlenderJoystickStop : IOGameBehaviour {

	[HideInInspector]
	public Button StopButton;

	public void Start(){
		StopButton = GetComponent<Button> ();
		SetupButton ();
	}

	public void StopAction(){
		BlenderController blenderController = GlobalGameState.PlayerBlenderController;
		if (blenderController == null)
			return;

		blenderController.StopAction ();
	}

	public void SetupButton () {

		StopButton.onClick.RemoveAllListeners ();

		StopButton.onClick.AddListener(delegate () { this.ButtonClicked(); });
	}

	public void ButtonClicked () {
		StopAction ();
	}
}
