using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IOGameBehaviour {

	[HideInInspector]
	public string id;

	[HideInInspector]
	public Vector3 targetPosition;

	[HideInInspector]
	public float animationTime = 1f;

	[HideInInspector]
	public bool IsSimulated;


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if (!IsSimulated)
			return;
		
		transform.position = targetPosition;
		return;

		while (animationTime <= 1f) {
			transform.position = Vector3.Lerp (transform.position, targetPosition, animationTime);
			animationTime += Time.deltaTime * 2.2f;
		}
	}
}
