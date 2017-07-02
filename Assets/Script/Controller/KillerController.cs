using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class KillerController : PlayerController {

	[HideInInspector]
	public Killer CharacterObject;

	private int KillerLife = 3;

	public KillerJoystickMove JoystickMove;
	public KillerJoystickCamera JoystickCam;

	public Image DamageUI;
	public Color DamageClrStart;
	public Color DamageClrPeak;

	public Image[] KillerLifeUI;

	[Space(10)]
	public float SensitivityX = 1f;
	public float SensitivityZ = 1f;

	[Space(10)]
	public float SensitivityYaw = 20f;
	public float SensitivityPitch = 20f;

	[Space(10)]
	public float KillDist = 20f;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (JoystickMove && JoystickCam && CharacterObject) {

			// debug ray for kill
			Vector3 fwd = CharacterObject.gameObject.transform.forward + new Vector3(0f, 0.5f, 0f);
			Debug.DrawRay(CharacterObject.gameObject.transform.position, fwd * KillDist, Color.red);
			
			float x = JoystickMove.Vertical ();
			float z = JoystickMove.Horizontal();

			float yaw = JoystickCam.Yaw ();
			float pitch = JoystickCam.Pitch ();

			bool IsDirtyPlayerPositionOnNetwork = false;
			bool IsDirtyPlayerRotationOnNetwork = false;

			//debug
			//Vector3 forward = CharacterObject.gameObject.transform.forward * 2f;
			//Debug.DrawRay(CharacterObject.gameObject.transform.position, forward, Color.green);
			
			GameObject playerGameObject = CharacterObject.gameObject;

			// position //------------------------------------------------------------
			// default when there is no position input from player
			Vector3 newPosition = playerGameObject.transform.position;

			if (x != 0f || z != 0f) {
				newPosition = playerGameObject.transform.position
					+ playerGameObject.transform.forward * x * SensitivityX
					+ playerGameObject.transform.right * z * SensitivityZ;

				CharacterObject.Rb.MovePosition (newPosition);
				//playerGameObject.transform.position = newPosition;
				IsDirtyPlayerPositionOnNetwork = true;
			}

			// rotation //------------------------------------------------------------
			// default when there is no rotation input from player
			Vector3 newBodyRotation = playerGameObject.transform.rotation.eulerAngles; 
			Vector3 newHeadRotation = CharacterObject.HeadTransform.localRotation.eulerAngles;

			if (yaw != 0f || pitch != 0f) {
				// reminder - Euler(pitch , yaw , roll)
				newBodyRotation = JoystickCam.BaseBodyRotation + new Vector3 (0f, yaw * SensitivityYaw, 0f);
				newHeadRotation = JoystickCam.BaseHeadLocalRotation + new Vector3 (pitch * SensitivityPitch, 0f, 0f);

				playerGameObject.transform.rotation = Quaternion.Euler (newBodyRotation);
				CharacterObject.HeadTransform.localRotation = Quaternion.Euler (newHeadRotation);

				IsDirtyPlayerRotationOnNetwork = true;
			}

			// network //------------------------------------------------------------
			if (IsDirtyPlayerPositionOnNetwork || IsDirtyPlayerRotationOnNetwork) {
				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["position"] = newPosition.x     + "," + newPosition.y      + "," + newPosition.z;
				data ["rotation"] = newHeadRotation.x + "," + newBodyRotation.y;

				//Debug.Log ("Attempting move:" + data["position"]);
				SocketIOComp.Emit("SERVER:MOVE_KILLER", new JSONObject(data));
			}
		}
	}

	public void TryKill(){
		RaycastHit objectHit;
		Vector3 fwd = CharacterObject.gameObject.transform.forward + new Vector3(0f, 0.5f, 0f);

		if (Physics.Raycast(CharacterObject.gameObject.transform.position, fwd, out objectHit, KillDist))
		{
			if (objectHit.collider == null || objectHit.collider.gameObject == null)
				return;
			
			Blender blender = objectHit.collider.gameObject.GetComponent<Blender>();
			if(blender != null){
				Debug.Log("Killing blender:" + blender.gameObject.name);

				blender.Kill (CharacterObject.gameObject.name);

				Dictionary<string, string> data = new Dictionary<string, string> ();
				data ["id"] = blender.id;
				SocketIOComp.Emit("SERVER:KILL_BLENDER", new JSONObject(data));

				if (blender.IsNPC) {
					KillerLife--;
					KillerLifeUI [KillerLife].gameObject.SetActive (false);

					LeanTween.value( DamageUI.gameObject, DamageClrStart, DamageClrPeak, .07f).setOnUpdate((Color val)=>{
						DamageUI.color = val;
					}).setLoopPingPong(1);

					if (KillerLife == 0) {
						KillerDie ();
					}
				}
			}
		}
	}
		

	public void KillerDie(){
		CharacterObject.Die();
		bool IsByPlayerWill = false;
		GlobalGameState.LeaveGame (IsByPlayerWill);
	}
}
