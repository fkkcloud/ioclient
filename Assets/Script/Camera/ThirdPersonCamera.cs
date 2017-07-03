using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : IOGameBehaviour {

	private GameObject PlayerObject;
	private Vector3 diff;
	private Vector3 destination;

	public float angularSpeed;

	public BlenderJoystickCamera BlenderCamStick;

	private Vector3 currentOffset;

	[HideInInspector]
	public Vector3 CamVelocity = Vector3.zero;

	public float dampTime = 0.15f;

	// Use this for initialization
	void Start () {
		if(PlayerObject == null) {
			Debug.LogError ("Assign a target for the camera in Unity's inspector");
		}
	}

	public void Setup(GameObject target)
	{
		PlayerObject = target;
		diff = transform.position - PlayerObject.transform.position;

		currentOffset = diff;
	}

	/*
	// Update is called once per frame
	void Update () {
		if (PlayerObject) {
			destination = PlayerObject.transform.position + diff;

			transform.position = Vector3.SmoothDamp(transform.position, destination, ref CamVelocity, dampTime);

		} else if (!PlayerObject && gameObject.activeSelf) {
			gameObject.SetActive (false);
		}
	}*/

	void LateUpdate(){
		if (!BlenderCamStick)
			return;

		currentOffset = Quaternion.AngleAxis (BlenderCamStick.Yaw() * angularSpeed, Vector3.up) * currentOffset;
		transform.position = PlayerObject.transform.position + currentOffset; 
		transform.LookAt(PlayerObject.transform.position);
	}
}
