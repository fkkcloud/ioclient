using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : IOGameBehaviour {

	[HideInInspector]
	public string id;

	public TextMesh txtUserName;
	public TextMesh txtChatMsg;

	[HideInInspector]
	public Vector3 simulatedEndPos;
	public Quaternion simulatedEndRot;

	[HideInInspector]
	public float simulationPosTimer = 1f;
	public float simulationRotTimer = 1f;

	[HideInInspector]
	public bool IsSimulated;

	public float simulationPosDamp = 1.86f; // this value varies between the speed of how object moves
	public float simulationRotDamp = 0.9f;

	// Use this for initialization
	void Start () {
		if (IsSimulated)
			simulatedEndPos = transform.position; // simulated transform position is always trying to set to simulatedEndPos
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!IsSimulated)
			return;

		//transform.position = simulatedEndPos;
		//return;

		float deltaTime = Time.fixedDeltaTime;

		if (simulationPosTimer <= 1f) {
			transform.position = Vector3.Lerp (transform.position, simulatedEndPos, simulationPosTimer);
			simulationPosTimer += Time.fixedDeltaTime * simulationPosDamp;
		}

		if (simulationRotTimer <= 1f) {
			transform.rotation = Quaternion.Lerp (transform.rotation, simulatedEndRot, simulationRotTimer);
			simulationRotTimer += deltaTime * simulationRotDamp;
		}
	}
}
