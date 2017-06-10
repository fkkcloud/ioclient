using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : IOGameBehaviour {

	[HideInInspector]
	public string id;

	public TextMesh txtUserName;
	public TextMesh txtChatMsg;

	public Transform HeadTransform;

	[HideInInspector]
	public Vector3 simulatedEndPos;
	public Quaternion simulatedHeadEndLocalRot;
	public Quaternion simulatedBodyEndRot;

	[HideInInspector]
	public float simulationPosTimer = float.MaxValue;
	public float simulationRotTimer = float.MaxValue;

	[HideInInspector]
	public bool IsSimulated;

	public float simulationPosDamp = 1.86f; // this value varies between the speed of how object moves
	public float simulationRotDamp = 0.9f;

	// Use this for initialization
	void Start () {
		if (IsSimulated){
			simulatedEndPos = transform.position; // simulated transform position is always trying to set to simulatedEndPos
			simulatedHeadEndLocalRot = HeadTransform.localRotation;
			simulatedBodyEndRot = transform.rotation;
		}
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
			// body rotation
			transform.rotation = Quaternion.Lerp (transform.rotation, simulatedBodyEndRot, simulationRotTimer);

			// head rotation
			HeadTransform.localRotation = Quaternion.Lerp (HeadTransform.localRotation, simulatedHeadEndLocalRot, simulationRotTimer);

			// i update
			simulationRotTimer += deltaTime * simulationRotDamp;
		}
	}
}
