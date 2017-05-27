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

	public float simulationDamp = 2.2f;


	// Use this for initialization
	void Start () {
		if (IsSimulated)
			simulatedEndPos = transform.position; // simulated transform position is always trying to set to simulatedEndPos
	}
	
	// Update is called once per frame
	void Update () {

		if (!IsSimulated)
			return;
		
		//transform.position = simulatedEndPos;
		//return;

		while (simulationTimer <= 1f) {
			transform.position = Vector3.Lerp (simulatedStartPos, simulatedEndPos, simulationTimer);
			simulationTimer += Time.deltaTime * simulationDamp;
		}
	}
}
