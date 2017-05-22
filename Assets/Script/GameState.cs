using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Text.RegularExpressions;

public class GameState : IOGameBehaviour {

	public GameObject LoginUI;

	public Text ResponseText;

	public GameObject PlayerPrefab;

	[HideInInspector]
	public List<Player> Players = new List<Player>();

	float pingtime = 0f;
	float pongtime = 0f;
	bool pingdone = false;

	// Use this for initialization
	void Start () {
		
		//SocketIOComp.url = "ws://safe-bastion-63386.herokuapp.com:80/socket.io/?EIO=4&transport=websocket";
		//SocketIOComp.url = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";

		StartCoroutine (InitConnection ());

		//ping
		StartCoroutine (PingUpdate());
	}
	IEnumerator PingUpdate()
	{
		SocketIOComp.Emit ("SERVER:PING");
		pingtime = Time.timeSinceLevelLoad;

		yield return new WaitForSeconds (1f);

		if (pingdone)
		{
			pongtime = pongtime * 1000f;
			ResponseText.text = (int)pongtime + "ms";
			pingdone = false;
		}
		StartCoroutine (PingUpdate ());
	}


	IEnumerator InitConnection(){
		InitCallbacks ();

		yield return new WaitForSeconds(0.125f);

		SocketIOComp.Emit ("SERVER:CONNECT");

		//yield return new WaitForSeconds(0.25f);
	}

	private void InitCallbacks(){
		SocketIOComp.On ("CLIENT:PING", OnPing);

		SocketIOComp.On ("CLIENT:JOINED", OnUserJoined);

		SocketIOComp.On ("CLIENT:CREATE_OTHER", OnOtherUserCreated);

		SocketIOComp.On ("CLIENT:MOVE", OnUserMove);

		SocketIOComp.On ("CLIENT:DISCONNECTED", OnUserDisconnect);
	}

	void OnPing(SocketIOEvent evt){ 
		pongtime = Mathf.Abs(Time.timeSinceLevelLoad - pingtime);
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

	private void OnUserJoined(SocketIOEvent evt){
		Debug.Log ("Connected server as " + evt.data);
		LoginUI.SetActive (false);

		// create currentUser here
		PlayerControllerComp.PlayerObject = CreateUser(evt, false);
		PlayerControllerComp.State = PlayerController.PlayerState.Joined;
	}

	private void OnOtherUserCreated(SocketIOEvent evt){
		Debug.Log ("Creating other user " + evt.data);

		// create otherUser here
		CreateUser(evt, true);
	}

	private void OnUserMove(SocketIOEvent evt){
		Debug.Log ("Moved data " + evt.data);

		string name = JsonToString( evt.data.GetField("name").ToString(), "\"");
		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		MoveUser (id, pos);
	}

	private void OnUserDisconnect(SocketIOEvent evt){
		
		Debug.Log ("disconnected user " + evt.data);

		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		Debug.Log ("disconnected user id:" + id);

		Player disconnectedPlayer = FindUserByID (id);

		Debug.Log ("disconnected user found:" + disconnectedPlayer);

		Players.Remove (disconnectedPlayer);

		Destroy (disconnectedPlayer.gameObject);
	}

	/*
	----------------------------------------------------------------------------------------------------------------
	GENERAL
	----------------------------------------------------------------------------------------------------------------
	*/

	private void MoveUser(string id, Vector3 position){
		Player playerComp = FindUserByID (id);
		playerComp.targetPosition = position;
		playerComp.animationTime = 0f;
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
		go.name = name;
		go.transform.position = pos;

		Players.Add (playerObject);
		return playerObject;
	}

	private Player FindUserByID(string id){
		foreach (Player playerComp in Players){
			if (playerComp.id == id)
				return playerComp;
		}
		return null;
	}

	/*
	----------------------------------------------------------------------------------------------------------------
	UTILITY
	----------------------------------------------------------------------------------------------------------------
	*/

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
