using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlenderJoystickWalk : IOGameBehaviour {

	[HideInInspector]
	public Button MoveButton;

	public void Start(){
		MoveButton = GetComponent<Button> ();
		SetupButton ();
	}

	public void Move(){
		BlenderController blenderController = GlobalGameState.PlayerBlenderController;
		if (blenderController == null)
			return;

		blenderController.Move ();
	}

	public void SetupButton () {

		MoveButton.onClick.RemoveAllListeners ();

		MoveButton.onClick.AddListener(delegate () { this.ButtonClicked(); });
	}

	public void ButtonClicked () {
		Move ();
	}
}
