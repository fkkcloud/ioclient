﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatUIController : UIController {

	public Button BtnSend;
	public InputField InputMsg;
	public Text ChatText;

	// Use this for initialization
	void Start () {
		BtnSend.onClick.AddListener(OnClickSendBtn);
	}
		

	void Update(){

		PlayerController pc = GlobalGameState.GetPlayerController ();
		if (!pc)
			return;

		if (InputMsg.isFocused) {
			pc.isOnChat = true;

			// only for window or pc
			#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			if (Input.GetKey (KeyCode.Return))
				OnClickSendBtn ();
			#endif
			
		} else {
			pc.isOnChat = false;
		}
	}

	void OnClickSendBtn ()
	{
		// Remove focus from input field
		InputMsg.OnDeselect (new BaseEventData(EventSystem.current));

		if(InputMsg.text != ""){
			SendChat (InputMsg.text);
		}
		else {
			InputMsg.text = "Speak up yourself.";
		}
	}

	void SendChat(string msg){
		Dictionary<string, string> data = new Dictionary<string, string> ();
		data ["chatmsg"] = msg;

		SocketIOComp.Emit ("SERVER:CHATSEND", new JSONObject (data));

		InputMsg.text = "";
	}
}
