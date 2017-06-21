using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : IOGameBehaviour {

	private GameObject PlayerObject;
	private Vector3 diff;
	private Vector3 destination;

	[HideInInspector]
	public Vector3 CamVelocity = Vector3.zero;

	public float dampTime = 0.15f;

	// Use this for initialization
	void Start () {
		
	}

	public void Setup(GameObject target)
	{
		PlayerObject = target;
		diff = transform.position - PlayerObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerObject) {
			destination = PlayerObject.transform.position + diff;

			transform.position = Vector3.SmoothDamp(transform.position, destination, ref CamVelocity, dampTime);

		} else if (!PlayerObject && gameObject.activeSelf) {
			gameObject.SetActive (false);
		}
	}
}
