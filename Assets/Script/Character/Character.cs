using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : IOGameBehaviour {

	[HideInInspector]
	public string id;

	public TextMesh txtUserName;
	public TextMesh txtChatMsg;

	[HideInInspector]
	public Vector3 simulatedEndPos;
	public Quaternion simulatedBodyEndRot;

	[HideInInspector]
	public bool IsSimulated;

	// this value varies between the speed of how object moves
	public float simulationPosDamp = 1.86f; 
	public float simulationRotDamp = 0.9f;

	// Use this for initialization
	protected virtual void Start () {
		if (IsSimulated){
			
			// simulated transform position is always trying to set to simulatedEndPos
			simulatedEndPos = transform.position; 
			simulatedBodyEndRot = transform.rotation;
		}
	}
	
	// Update is called once per frame
	protected virtual void Update () {


	}
}
