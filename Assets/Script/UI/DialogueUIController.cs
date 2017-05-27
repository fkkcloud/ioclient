using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : UIController {

	public Text DialogueText;
	public Button OKButton;

	public enum DialogueTypes {ConnectingServer, JoiningRoom, ServerFull, RoomFull};

	void Start(){
		OKButton.onClick.AddListener(Hide);
	}

	public void Show(DialogueTypes type){

		UIObject.SetActive (true);
		OKButton.gameObject.SetActive (false);

		switch (type) {
		case DialogueTypes.ConnectingServer:
			DialogueText.text = "Connecting to server..";
			return;
		case DialogueTypes.JoiningRoom:
			DialogueText.text = "Joining room..";
			return;
		case DialogueTypes.ServerFull:
			DialogueText.text = "Server is full..";
			return;
		case DialogueTypes.RoomFull:
			DialogueText.text = "Room is full..";
			return;
		default:
			DialogueText.text = "";
			return;
		}
	}

	public void ShowWithOKBtn(DialogueTypes type){

		UIObject.SetActive (true);
		OKButton.gameObject.SetActive (true);
		
		switch (type) {
		case DialogueTypes.ConnectingServer:
			DialogueText.text = "Connecting to server..";
			return;
		case DialogueTypes.JoiningRoom:
			DialogueText.text = "Joining room..";
			return;
		case DialogueTypes.ServerFull:
			DialogueText.text = "Server is full..";
			return;
		case DialogueTypes.RoomFull:
			DialogueText.text = "Room is full..";
			return;
		default:
			DialogueText.text = "";
			return;
		}
	}

	public void Hide(){
		DialogueText.text = "";
		UIObject.SetActive (false);
		OKButton.gameObject.SetActive (false);
	}
}
