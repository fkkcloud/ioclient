using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillerJoystickKill : IOGameBehaviour {

	[HideInInspector]
	public Button KillButton;

	public void Start(){
		KillButton = GetComponent<Button> ();
		SetupButton ();
	}

	public void Kill(){
		KillerController killerController = GlobalGameState.PlayerKillerController;
		if (killerController == null)
			return;

		killerController.TryKill ();
	}

	public void SetupButton () {

		KillButton.onClick.RemoveAllListeners ();

		KillButton.onClick.AddListener(delegate () { this.ButtonClicked(); });
	}

	public void ButtonClicked () {
		Kill ();
	}
}
