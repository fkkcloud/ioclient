using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

public class LoginController : UIController {

	public Button JoinBtn;
	public Button JoinCustomBtn;
	public InputField InputName;
	public Text InputNamePlaceHolder;

	public GameObject CustomChannelWindow;
	public InputField ChannelInput;
	public Text ChannelInputPlaceHolder;
	public Button ChannelWindowOKBtn;
	public Button ChannelWindowCancelBtn;


	// Use this for initialization
	void Start () {
		JoinBtn.onClick.AddListener(OnClickJoinBtn);
		JoinCustomBtn.onClick.AddListener (OnClickCustomChannelBtn);

		ChannelWindowOKBtn.onClick.AddListener (JoinCustom);
		ChannelWindowCancelBtn.onClick.AddListener (HideChannelWindow);

		InputName.text = LocalUserData.GetUserName ();
	}

	public void OnClickJoinBtn ()
	{
		if (InputName.text != "") {
			LocalUserData.SetUserName (InputName.text);

			Dictionary<string, string> data = new Dictionary<string, string> ();

			data ["name"] = InputName.text;

			/*
				TODO: player position will be set from server-side
			*/
			Vector3 position = new Vector3 (0f, 0f, 0f);
			data ["position"] = position.x + "," + position.y + "," + position.z;

			SocketIOComp.Emit ("SERVER:JOIN", new JSONObject (data));

			GlobalGameState.DialogueUI.Show (DialogueUIController.DialogueTypes.JoiningRoom);
		} else {
			InputNamePlaceHolder.text = "Nickname is necessary!";
		}
	}

	public void OnClickCustomChannelBtn(){
		if (InputName.text != "") {
			ShowChannelWindow ();
		} else {
			InputNamePlaceHolder.text = "Nickname is necessary!";
		}
	}

	public void ShowChannelWindow () {
		CustomChannelWindow.SetActive (true);
	}

	public void HideChannelWindow() {
		CustomChannelWindow.SetActive (false);
	}

	public void JoinCustom(){
		/*
			There should be only number-inputs
		*/

		if (ChannelInput.text != "") {
			LocalUserData.SetUserName (InputName.text);

			Dictionary<string, string> data = new Dictionary<string, string> ();

			data ["name"] = InputName.text;
			data ["roomnumber"] = ChannelInput.text;

			/*
			TODO: player position will be set from server-side
			*/
			Vector3 position = new Vector3 (0f, 0f, 0f);
			data ["position"] = position.x + "," + position.y + "," + position.z;

			SocketIOComp.Emit ("SERVER:JOINCUSTOM", new JSONObject (data));

			HideChannelWindow ();
			GlobalGameState.DialogueUI.Show (DialogueUIController.DialogueTypes.JoiningRoom);
		} else {
			ChannelInputPlaceHolder.text = "Channel number is necessary!";
		}
	}
}
