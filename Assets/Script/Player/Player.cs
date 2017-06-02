using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : IOGameBehaviour {

	[HideInInspector]
	public string id;

	public Vector3 simulatedStartPos;

	public TextMesh txtUserName;
	public TextMesh txtChatMsg;

	[HideInInspector]
	public Vector3 simulatedEndPos;

	[HideInInspector]
	public float simulationTimer = 1f;

	[HideInInspector]
	public bool IsSimulated;

	public float simulationDamp = 2f;


	// Use this for initialization
	void Start () {
		if (IsSimulated)
			simulatedEndPos = transform.position; // simulated transform position is always trying to set to simulatedEndPos
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!IsSimulated)
			return;

		gameObject.transform.Rotate (new Vector3 (2f, 0f, 0f));
		
		//transform.position = simulatedEndPos;
		//return;

		if (simulationTimer <= 1f) {
			transform.position = Vector3.Lerp (transform.position, simulatedEndPos, simulationTimer);
			//transform.position = Vector3.MoveTowards (transform.position, simulatedEndPos, simulationTimer);
			simulationTimer += Time.fixedDeltaTime * simulationDamp;
		}
	}
}
