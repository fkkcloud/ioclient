using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Text.RegularExpressions;

public class GameState : IOGameBehaviour {

	public LoginController LoginUI;
	public ChatUIController ChatUI;
	public GameUIController GameUI;
	public DialogueUIController DialogueUI;

	public Text ResponseText;
	public Text ChannelText;

	public GameObject PlayerPrefab;
	public GameObject ScenePrefab;

	[HideInInspector]
	public List<Player> Players = new List<Player>();
	[HideInInspector]
	public GameObject CurrentScene;

	bool ServerConnected = false;

	float pingtime = 0f;
	float pongtime = 0f;
	float lastpongtime = 0f;
	bool pingdone = false;

	// Use this for initialization
	void Start () {

		ChatUI.Hide ();
		LoginUI.Hide ();
		DialogueUI.Hide ();
		GameUI.Hide ();
		
		//SocketIOComp.url = "http://safe-bastion-63386.herokuapp.com:80/socket.io/?EIO=4&transport=websocket";
		//SocketIOComp.url = "http://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";
		InitCallbacks ();

		StartCoroutine (Connection ());

		//ping
		StartCoroutine (PingUpdate ());
	}

	IEnumerator PingUpdate()
	{
		SocketIOComp.Emit ("SERVER:PING");
		pingtime = Time.timeSinceLevelLoad;

		yield return new WaitForSeconds (1f);

		// check server is connected
		float lastpongduration = Mathf.Abs (Time.timeSinceLevelLoad - lastpongtime);
		if (lastpongduration > 4f) {
			// if server is disconnected
			PlayerControllerComp.State = PlayerController.PlayerState.Lobby;
			ServerConnected = false;
			ChatUI.Hide ();
			LoginUI.Hide ();
			GameUI.Hide ();
			ClearScene ();
		}

		if (pingdone)
		{
			pongtime = pongtime * 1000f;
			ResponseText.text = (int)pongtime + "ms";
			pingdone = false;
		}
		StartCoroutine (PingUpdate ());
	}


	IEnumerator Connection(){
		
		yield return new WaitForSeconds(1f);

		if (!ServerConnected) {
			DialogueUI.Show (DialogueUIController.DialogueTypes.ConnectingServer);
			SocketIOComp.Emit ("SERVER:CONNECT");
		}
		StartCoroutine (Connection ());
	}

	private void InitCallbacks(){
		SocketIOComp.On ("CLIENT:PING", OnPing);

		SocketIOComp.On ("CLIENT:CONNECTED", OnServerConnected);

		SocketIOComp.On ("CLIENT:JOINED", OnUserJoined);

		SocketIOComp.On ("CLIENT:CREATE_OTHER", OnOtherUserCreated);

		SocketIOComp.On ("CLIENT:MOVE", OnUserMove);

		SocketIOComp.On ("CLIENT:DISCONNECTED", OnUserDisconnect);

		SocketIOComp.On ("CLIENT:CHATSEND", OnChatSend);

		SocketIOComp.On ("CLIENT:ROOM_FULL", OnRoomFull);

		SocketIOComp.On ("CLIENT:SERVER_FULL", OnServerFull);
	}

	void OnPing(SocketIOEvent evt){ 
		pongtime = Mathf.Abs(Time.timeSinceLevelLoad - pingtime);
		lastpongtime = Time.timeSinceLevelLoad;
		pingdone = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	/*
	----------------------------------------------------------------------------------------------------------------
	Callbacks
	----------------------------------------------------------------------------------------------------------------
	*/

	private void OnServerConnected(SocketIOEvent evt){

		ServerConnected = true;

		DialogueUI.Hide ();
		LoginUI.Show ();
	}

	private void OnUserJoined(SocketIOEvent evt){

		// --------- GAME BEGIN ----------

		Debug.Log ("Connected server as " + evt.data);
		GameUI.Show ();
		DialogueUI.Hide ();
		LoginUI.Hide ();
		ChatUI.Show ();

		// create currentUser here
		PlayerControllerComp.PlayerObject = CreateUser(evt, false);
		PlayerControllerComp.State = PlayerController.PlayerState.Joined;

		// create temp scene
		CreateScene();

		ChannelText.text = JsonToString(evt.data.GetField("room").ToString(), "\"");
	}

	private void OnOtherUserCreated(SocketIOEvent evt){
		Debug.Log ("Creating other user " + evt.data);

		// create otherUser here
		CreateUser(evt, true);
	}

	private void OnUserMove(SocketIOEvent evt){
		//Debug.Log ("Moved data " + evt.data);

		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		MoveUser (id, pos);
	}

	private void OnUserDisconnect(SocketIOEvent evt){
		
		Debug.Log ("disconnected user " + evt.data);

		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		//Debug.Log ("disconnected user id:" + id);

		Player disconnectedPlayer = FindUserByID (id);

		//Debug.Log ("disconnected user found:" + disconnectedPlayer);

		Players.Remove (disconnectedPlayer);

		Destroy (disconnectedPlayer.gameObject);
	}

	private void OnChatSend(SocketIOEvent evt){

		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		Player player = FindUserByID (id);

		player.txtChatMsg.text = JsonToString(evt.data.GetField("chatmsg").ToString(), "\"");
	}

	private void OnRoomFull(SocketIOEvent evt){
		DialogueUI.Hide ();
		DialogueUI.ShowWithOKBtn (DialogueUIController.DialogueTypes.RoomFull);
	}

	private void OnServerFull(SocketIOEvent evt){
		DialogueUI.Hide ();
		DialogueUI.ShowWithOKBtn (DialogueUIController.DialogueTypes.ServerFull);
	}

	/*
	----------------------------------------------------------------------------------------------------------------
	GENERAL
	----------------------------------------------------------------------------------------------------------------
	*/

	private void MoveUser(string id, Vector3 position){
		Player playerComp = FindUserByID (id);
		playerComp.simulationTimer = 0f;
		playerComp.simulatedStartPos = playerComp.gameObject.transform.position;
		playerComp.simulatedEndPos = position;
	}

	private Player CreateUser(SocketIOEvent evt, bool IsSimulated){
		Debug.Log ("Creating player object: " + evt);

		string name = JsonToString( evt.data.GetField("name").ToString(), "\"");
		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		GameObject go = Instantiate (PlayerPrefab, pos, Quaternion.identity);
		Player playerObject = go.GetComponent<Player> ();

		// set basics
		playerObject.IsSimulated = IsSimulated;
		playerObject.id = id;
		playerObject.txtUserName.text = name;
		playerObject.txtChatMsg.text = "";
		go.name = name;
		go.transform.position = pos;

		Players.Add (playerObject);
		return playerObject;
	}

	public void CreateScene(){
		CurrentScene = Instantiate (ScenePrefab);
	}

	public void ClearAllPlayers(){
		// clear all the characters
		foreach (Player player in Players) {
			Destroy (player.gameObject);
		}
		Players.Clear ();
	}

	public void ClearScene(){

		if (CurrentScene)
			Destroy (CurrentScene);	

		ClearAllPlayers ();

	}

	/*
	----------------------------------------------------------------------------------------------------------------
	UTILITY
	----------------------------------------------------------------------------------------------------------------
	*/

	private Player FindUserByID(string id){
		foreach (Player playerComp in Players){
			if (playerComp.id == id)
				return playerComp;
		}
		return null;
	}

	string JsonToString( string target, string s){

		string[] newString = Regex.Split(target,s);

		return newString[1];

	}

	Vector3 StringToVecter3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,",");
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]));

		return newVector;
	}
}
