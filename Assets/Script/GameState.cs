using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using System.Text.RegularExpressions;

public class GameState : IOGameBehaviour {

	public enum GameStateEnum {Spectate, IsWaitingForGameStart, IsPlaying, EndedGame};
	public enum CharacterType {Blender, Killer};
	public enum WinSide {Blender, Killer, Neither};
	public enum BoolInt {False, True};

	[HideInInspector]
	public bool IsNPCBlenderMaster = false;
	[HideInInspector]
	public int NPCCount = 0;
	[HideInInspector]
	public bool IsPlayerReady = false;

	[Space(20)]
	public LoginController LoginUI;
	public ChatUIController ChatUI;
	public GameUIController GameUI;
	public DialogueUIController DialogueUI;
	public LobbyUIController LobbyUI;
	public Text GameTimeUI;
	public Text LogText;
	public GameEndUIController GameEndUI;

	[Space(20)]
	public Text ResponseText;
	public Text ChannelText;

	[Space(20)]
	public GameObject FirstCam;
	public GameObject ThirdCam;

	[Space(20)]
	public GameObject KillerControllerPrefab;
	public GameObject BlenderControllerPrefab;

	[Space(20)]
	public GameObject KillerPrefab;
	public GameObject BlenderPrefab;
	[Space(10)]
	public GameObject BlenderNPCPrefab;
	[Space(10)]
	public GameObject KillerDieFX;
	public GameObject BlenderPlayerKillFX;
	public GameObject BlenderNPCKillFX;

	[Space(10)]
	public GameObject[] MapPrefabs;

	[HideInInspector]
	public List<Killer> Killers = new List<Killer>();
	[HideInInspector]
	public List<Blender> Blenders = new List<Blender>();
	[HideInInspector]
	public List<Blender> BlenderNPCs = new List<Blender>();
	[HideInInspector]
	public GameObject CurrentScene;

	[HideInInspector]
	public KillerController PlayerKillerController;
	[HideInInspector]
	public BlenderController PlayerBlenderController;

	public Camera SpectateCam;

	ThirdPersonCamera ThirdCamComp;
	FirstPersonCamera FirstCamComp;

	bool ServerConnected = false;
	float pingtime = 0f;
	float pongtime = 0f;
	float lastpongtime = 0f;
	bool pingdone = false;

	public void HideAllUI(){
		ChatUI.Hide ();
		LoginUI.Hide ();
		GameUI.Hide ();
		LobbyUI.Hide ();
		DialogueUI.Hide ();
		GameEndUI.Hide ();
	}
		
	// Use this for initialization
	void Start () {

		#if UNITY_STANDALONE
		Screen.SetResolution(16 * 45, 9 * 45, false);
		#endif


		HideAllUI ();
		DialogueUI.Show ();

		//SocketIOComp.url = "ws://safe-bastion-63386.herokuapp.com:80/socket.io/?EIO=4&transport=websocket";
		//SocketIOComp.url = "ws://127.0.0.1:3000/socket.io/?EIO=4&transport=websocket";
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
			ServerConnected = false;

			DialogueUI.Show ();

			Disconnect ();
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

		SocketIOComp.On ("CLIENT:TIMER_UPDATE", OnGameTimeUpdate);

		SocketIOComp.On ("CLIENT:CREATE_MAP", OnCreateMap);

		SocketIOComp.On ("CLIENT:CONNECTED", OnServerConnected);

		SocketIOComp.On ("CLIENT:JOINED", OnUserJoined);
		SocketIOComp.On ("CLIENT:OTHER_JOINED", OnOtherUserJoined);

		SocketIOComp.On ("CLIENT:GAMESTATE", OnGameState);

		SocketIOComp.On ("CLIENT:CREATE_OTHER", OnOtherUserCreated);

		SocketIOComp.On ("CLIENT:WALK_BLENDER", OnBlenderWalk);
		SocketIOComp.On ("CLIENT:ROTATE_BLENDER", OnBlenderRotate);
		SocketIOComp.On ("CLIENT:KILL_BLENDER", OnKillBlender);

		SocketIOComp.On ("CLIENT:MOVE_KILLER", OnKillerMove);

		SocketIOComp.On ("CLIENT:DISCONNECTED", OnOtherUserDisconnect);

		SocketIOComp.On ("CLIENT:CHATSEND", OnChatSend);

		SocketIOComp.On ("CLIENT:ROOM_FULL", OnRoomFull);

		SocketIOComp.On ("CLIENT:SERVER_FULL", OnServerFull);

		SocketIOComp.On ("CLIENT:BLENDER_NPC_CREATE", OnBlenderNPCCreate);

		SocketIOComp.On ("CLIENT:PLAYERSETUP", OnPlayerSetup);

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

	private void OnGameTimeUpdate(SocketIOEvent evt){

		int gametime = JsonToInt(evt.data.GetField("timer").ToString(), "\"");
		if (gametime > -1) {
			GameTimeUI.gameObject.SetActive (true);
			GameTimeUI.text = gametime.ToString ();
		} else {
			GameTimeUI.gameObject.SetActive (false);
		}
	}

	private void OnServerConnected(SocketIOEvent evt){

		ServerConnected = true;

		HideAllUI ();
		LoginUI.Show ();
	}

	private void OnUserJoined(SocketIOEvent evt){
		// --------- GAME BEGIN ----------
		Debug.Log ("Connected room as " + evt.data);

		HideAllUI ();
		GameUI.Show ();
		ChatUI.Show ();
		LobbyUI.Show ();

		ChannelText.text = JsonToString(evt.data.GetField("room").ToString(), "\"");
		LogText.text = "Joined room: " + ChannelText.text;
	}

	private void OnOtherUserJoined(SocketIOEvent evt){
		LogText.text = JsonToString(evt.data.GetField("name").ToString(), "\"") + " has joined";
	}

	private void OnCreateMap(SocketIOEvent evt){

		Debug.Log ("creating map..:" + evt.data);

		int mapid = int.Parse(evt.data.GetField("mapid").ToString());
		if (CurrentScene == null)
			CurrentScene = Instantiate (MapPrefabs[mapid]);
		
		GlobalMapManager.Init (CurrentScene.GetComponent<MapData> ());

		//SocketIOComp.Emit ("SERVER:CREATED_MAP", );
	}

	private void OnGameState(SocketIOEvent evt){

		GameStateEnum gameState = (GameStateEnum)JsonToInt(evt.data.GetField("gamestate").ToString(), "\"");

		Debug.Log ("////////// GAMESTATE CHANGE ////////// ::" + gameState);

		if (gameState == GameStateEnum.Spectate) {
			HideAllUI ();
			LobbyUI.Show ();
			GameUI.Show ();
			OnSpectateChangeCamera (gameState);
		} else if (gameState == GameStateEnum.IsWaitingForGameStart) {
			HideAllUI ();
			LobbyUI.Show ();
			GameUI.Show ();
			OnSpectateChangeCamera (gameState);
		} else if (gameState == GameStateEnum.IsPlaying) {
			HideAllUI ();
			GameUI.Show ();
			ChatUI.Show ();

			OnSpectateChangeCamera (gameState);
			IsNPCBlenderMaster = JsonToBool (evt.data.GetField ("npc_master").ToString (), "\"");
			NPCCount = int.Parse (evt.data.GetField ("npc_count").ToString ());

			SocketIOComp.Emit ("SERVER:READY_GAMESTART");
		} else if (gameState == GameStateEnum.EndedGame) {
			HideAllUI ();

			WinSide winSide = (WinSide)JsonToInt(evt.data.GetField("winside").ToString(), "\"");
			GameEndUI.SetState (winSide);
			GameEndUI.Show ();

			ClearScene ();

			SocketIOComp.Emit ("SERVER:ENDED_GAME");

			OnSpectateChangeCamera (gameState);
		}
	}

	private void OnSpectateChangeCamera(GameStateEnum gs){
		if (gs != GameStateEnum.IsPlaying) {
			SpectateCam.gameObject.SetActive (true);
		} else {
			SpectateCam.gameObject.SetActive (false);
		}
	}

	private void OnPlayerSetup(SocketIOEvent evt){

		Debug.Log ("creating character/controller");
		
		CharacterType PlayType = (CharacterType)JsonToInt(evt.data.GetField("type").ToString(), "\"");

		bool isSimulated = false;

		Debug.Log ("Creating players controller and character:" + PlayType);

		if (PlayType == CharacterType.Blender) // case blender
		{
			GameObject prefab = Instantiate (BlenderControllerPrefab);
			PlayerBlenderController = prefab.GetComponent<BlenderController> ();
			PlayerBlenderController.CharacterObject = CreateCharacter(evt, isSimulated, BlenderPrefab) as Blender;
			Debug.Log ("Created Blender:" + PlayerBlenderController.CharacterObject);
			Blenders.Add (PlayerBlenderController.CharacterObject);

			// for blender 3rd person cam
			GameObject cam = Instantiate(ThirdCam, PlayerBlenderController.CharacterObject.transform.position + ThirdCam.transform.position, ThirdCam.transform.rotation);
			ThirdCamComp = cam.GetComponent<ThirdPersonCamera> ();
			ThirdCamComp.gameObject.SetActive(true);
			ThirdCamComp.GetComponent<ThirdPersonCamera>().Setup (PlayerBlenderController.CharacterObject.gameObject);
		}
		else if (PlayType == CharacterType.Killer)
		{
			GameObject prefab = Instantiate (KillerControllerPrefab);
			PlayerKillerController = prefab.GetComponent<KillerController> ();
			PlayerKillerController.CharacterObject = CreateCharacter(evt, isSimulated, KillerPrefab) as Killer;
			Debug.Log ("Created Killer:" + PlayerKillerController.CharacterObject);
			Killers.Add (PlayerKillerController.CharacterObject);

			// for killer 1st person cam
			GameObject cam = Instantiate(FirstCam);
			FirstCamComp = cam.GetComponent<FirstPersonCamera> ();
			FirstCamComp.gameObject.SetActive(true);
			FirstCamComp.gameObject.transform.position = PlayerKillerController.CharacterObject.HeadTransform.position;
			FirstCamComp.gameObject.transform.parent = PlayerKillerController.CharacterObject.HeadTransform;
		}

		SocketIOComp.Emit ("SERVER:CREATE_FOR_OTHERPLAYER");

		// create NPC blenders
		if (GlobalGameState.IsNPCBlenderMaster) {
			GlobalMapManager.CreateNPCBlenders (NPCCount);
		}
	}

	private void OnOtherUserCreated(SocketIOEvent evt){
		Debug.Log ("Creating other user " + evt.data);

		CharacterType PlayType = (CharacterType)JsonToInt(evt.data.GetField("type").ToString(), "\"");

		Debug.Log ("Creating players controller and character:" + PlayType);

		bool isSimulated = true;

		if (PlayType == CharacterType.Blender){
			Blender blender = CreateCharacter (evt, isSimulated, BlenderPrefab) as Blender;
			blender.Rb.isKinematic = true;
			blender.Rb.useGravity = false;
			Blenders.Add(blender);
		} else if (PlayType == CharacterType.Killer){
			Killer killer = CreateCharacter (evt, isSimulated, KillerPrefab) as Killer;
			killer.Rb.isKinematic = true;
			killer.Rb.useGravity = false;
			Killers.Add(killer);
		}
	}

	private void OnBlenderNPCCreate(SocketIOEvent evt){
		bool isSimulated = true;

		Blender blender = CreateCharacter (evt, isSimulated, BlenderNPCPrefab) as Blender;
		blender.IsNPC = true;
		blender.Rb.isKinematic = true;
		blender.Rb.useGravity = false;
		BlenderNPCs.Add(blender);
	}

	private void OnKillBlender(SocketIOEvent evt){
		string killername = JsonToString(evt.data.GetField("killername").ToString(), "\"");
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		BlenderController blenderCtrl = GetPlayerController () as BlenderController;
		if (blenderCtrl && 
			blenderCtrl.CharacterObject &&
			blenderCtrl.CharacterObject.id == id) 
		{
			blenderCtrl.CharacterObject.Kill (killername);
			bool IsByPlayerWill = false;
			LeaveGame (IsByPlayerWill);
		} 
		else 
		{
			Blender blender = FindBlenderByID (id);
			if (blender)
				blender.Kill (killername);
		}
	}

	private void OnKillerDie(SocketIOEvent evt){
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		Killer killer = FindKillerByID (id);
		if (killer)
			killer.Die ();
	
	}

	private void OnBlenderWalk(SocketIOEvent evt){
		//Debug.Log ("Moved data " + evt.data);

		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");
		float elapsedTime = JsonToFloat (evt.data.GetField ("elapsedTime").ToString (), "\"");

		MoveBlender (id, pos, elapsedTime);
	}

	private void OnBlenderRotate(SocketIOEvent evt){
		//Debug.Log ("Moved data " + evt.data);

		Vector2 rot = StringToVecter2( JsonToString(evt.data.GetField("rotation").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");
		float elapsedTime = JsonToFloat (evt.data.GetField ("elapsedTime").ToString (), "\"");

		RotateBlender (id, rot, elapsedTime);
	}

	private void OnKillerMove(SocketIOEvent evt){
		//Debug.Log ("Moved data " + evt.data);

		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		Vector2 rot = StringToVecter2( JsonToString(evt.data.GetField("rotation").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		MoveKiller (id, pos, rot);
	}

	private void OnOtherUserDisconnect(SocketIOEvent evt){
		
		Debug.Log ("disconnected user " + evt.data);

		LogText.text = JsonToString(evt.data.GetField("name").ToString(), "\"") + " has left";

		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		RemoveUser (id);
	}

	private void OnChatSend(SocketIOEvent evt){

		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");
		string name = JsonToString(evt.data.GetField("name").ToString(), "\"");

		//Character player = FindCharacterByID (id);

		ChatUI.ChatText.text = name + ":" + JsonToString(evt.data.GetField("chatmsg").ToString(), "\"");
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

	/*
		rotation come as Vector2 from server
		x : pitch : head rotation
		y : yaw   : body rotation
		bodyRotation : Vector3 (0f, yaw * SensitivityYaw, 0f);
		headRotation : Vector3 (pitch * SensitivityPitch, 0f, 0f);
	*/
	private void MoveKiller(string id, Vector3 position, Vector2 rotation){
		
		Killer killer = FindKillerByID (id);

		//Debug.Log ("vector length diff:" + Mathf.Abs((killer.transform.position - position).magnitude));

		killer.simulatedEndPos = position;

		killer.simulatedHeadEndLocalRot = Quaternion.Euler(new Vector3(rotation.x, 0f, 0f)); // head only pitch
		killer.simulatedBodyEndRot = Quaternion.Euler(new Vector3(0f, rotation.y, 0f)); // body only yaw
	}

	private void MoveBlender(string id, Vector3 position, float elapsedTime){

		Blender blender = FindBlenderByID (id);

		// if the data from server is older than latest applied packet then discard
		if (elapsedTime < blender.newestElapsedTimePosition) 
			return;

		blender.newestElapsedTimePosition = elapsedTime;
		blender.simulatedEndPos = position;
	}

	private void RotateBlender(string id, Vector2 rotation, float elapsedTime){

		Blender blender = FindBlenderByID (id);

		// if the data from server is older than latest applied packet then discard
		if (elapsedTime < blender.newestElapsedTimeRotation) 
			return;

		blender.newestElapsedTimeRotation = elapsedTime;
		blender.simulatedBodyEndRot = Quaternion.Euler(new Vector3(0f, rotation.y, 0f)); // body only yaw
	}

	private Character CreateCharacter(SocketIOEvent evt, bool IsSimulated, GameObject prefab){
		Debug.Log ("Creating player object: " + evt.data);

		string name = JsonToString( evt.data.GetField("name").ToString(), "\"");
		Vector3 pos = StringToVecter3( JsonToString(evt.data.GetField("position").ToString(), "\"") );
		Vector2 rot = StringToVecter2( JsonToString(evt.data.GetField("rotation").ToString(), "\"") );
		string id = JsonToString(evt.data.GetField("id").ToString(), "\"");

		Debug.Log ("Creating " + name + " " + pos + " "  + rot + " " + id);

		Quaternion yaw = (rot.y == 0) ? Quaternion.identity : Quaternion.Euler (new Vector3 (0f, rot.y, 0f));

		//Quaternion pitch = (rot.x == 0) ? Quaternion.identity : Quaternion.Euler (new Vector3 (rot.x, 0f, 0f));

		GameObject go;
		if (IsSimulated)
			go = Instantiate (prefab, pos, yaw);
		else
			go = Instantiate (prefab, pos, Quaternion.identity);

		Character playerObject = go.GetComponent<Character> ();

		// set basics
		playerObject.IsSimulated = IsSimulated;
		playerObject.id = id;
		//playerObject.txtUserName.text = name;

		go.name = name;
		go.transform.position = pos;

		//playerObject.HeadTransform.localRotation = pitch;

		Debug.Log ("character created:" + playerObject + "::" + go);

		return playerObject;
	}

	public void ClearController(){
		PlayerController pc = GetPlayerController ();

		if (pc)
			Destroy (pc.gameObject);
	}

	public void ClearAllPlayers(){

		Debug.Log("deleting all killers:" + Killers.Count);
		foreach (Killer player in Killers) {
			if (player)
				Destroy (player.gameObject);
		}

		Debug.Log("deleting all blenders" + Blenders.Count);
		foreach (Blender player in Blenders) {
			if (player)
				Destroy (player.gameObject);
		}
			
		Debug.Log("deleting all blender NPC" + BlenderNPCs.Count);
		foreach (Blender blender in BlenderNPCs) {
			if (blender)
				Destroy (blender.gameObject);
		}
			
		Blenders.Clear ();
		Killers.Clear ();
		BlenderNPCs.Clear ();
	}

	public void ClearScene(){
		
		ClearController ();

		ClearAllPlayers ();


		if (ThirdCamComp)
			Destroy (ThirdCamComp.gameObject);

		if (FirstCamComp)
			Destroy (FirstCamComp.gameObject);

		Destroy (CurrentScene);	
	}

	public void Disconnect(){
		ClearScene ();
	}

	public void HandleLeaveGame(){
		RemoveMyselfInLocal ();
		ClearController ();
	}

	public void RemoveMyselfInLocal(){
		BlenderController blenderCtrl = GetPlayerController () as BlenderController;
		if (blenderCtrl != null && blenderCtrl.CharacterObject)
			Destroy (blenderCtrl.CharacterObject.gameObject);

		KillerController killerCtrl = GetPlayerController () as KillerController;
		if (killerCtrl != null && killerCtrl.CharacterObject)
			Destroy (killerCtrl.CharacterObject.gameObject);
	}

	public void RemoveUser(string id){
		Killer killer = FindKillerByID(id);
		if (killer) {
			Killers.Remove (killer);
			Destroy (killer.gameObject);

			Debug.Log ("removed killer:" + killer);
			return;
		}

		Blender blender = FindBlenderByID(id);
		if (blender) {
			Blenders.Remove (blender);
			Destroy (blender.gameObject);

			Debug.Log ("removed blender:" + blender);
			return;
		}
	}

	public void LeaveGame(bool IsByPlayerWill){

		SocketIOComp.Emit ("SERVER:LEAVE_GAME");

		//GlobalGameState.Disconnect ();
		GlobalGameState.HandleLeaveGame();
		GlobalGameState.HideAllUI ();

		if (IsByPlayerWill)
			GlobalGameState.LobbyUI.ResetLobbyState ();
		GlobalGameState.LobbyUI.Show ();
		GlobalGameState.GameUI.Show ();

		// set spectate cam on!
		GlobalGameState.SpectateCam.gameObject.SetActive (true);
	}

	/*
	----------------------------------------------------------------------------------------------------------------
	UTILITY
	----------------------------------------------------------------------------------------------------------------
	*/

	private Character FindCharacterByID(string id){
		foreach (Character killer in Killers){
			if (killer.id == id)
				return killer;
		}
		foreach (Character blender in Blenders){
			if (blender.id == id)
				return blender;
		}
		foreach (Character blender in BlenderNPCs) {
			if (blender.id == id)
				return blender;
		}
		return null;
	}

	private Killer FindKillerByID(string id){
		foreach (Killer killer in Killers){
			if (killer.id == id)
				return killer;
		}
		return null;
	}

	private Blender FindBlenderByID(string id){
		foreach (Blender blender in Blenders){
			if (blender.id == id)
				return blender;
		}
		foreach (Blender blender in BlenderNPCs) {
			if (blender.id == id)
				return blender;
		}
		return null;
	}


	string JsonToString( string target, string s){

		string[] newString = Regex.Split(target,s);

		return newString[1];

	}

	float JsonToFloat( string target, string s){

		string[] newString = Regex.Split(target,s);

		return float.Parse(newString[1]);

	}

	int JsonToInt(string target, string s){

		string[] newString = Regex.Split(target,s);

		return int.Parse(newString[1]);
	}

	bool JsonToBool(string target, string s){

		string[] newString = Regex.Split(target,s);

		int value = int.Parse (newString [1]);

		if (value == 0)
			return false;
		else
			return true;
	}

	Vector3 StringToVecter3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,",");
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]));

		return newVector;
	}

	Vector2 StringToVecter2(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,",");
		newVector = new Vector2( float.Parse(newString[0]), float.Parse(newString[1]));

		return newVector;
	}

	public PlayerController GetPlayerController(){
		if (PlayerKillerController)
			return PlayerKillerController;
		else if (PlayerBlenderController)
			return PlayerBlenderController;

		return null;
	}
}
