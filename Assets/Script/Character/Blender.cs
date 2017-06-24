using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blender : Character {

	[HideInInspector]
	public Vector3 targetCharacterPosition;

	[HideInInspector]
	public float newestElapsedTimePosition = 0f;
	[HideInInspector]
	public float newestElapsedTimeRotation = 0f;

	// Use this for initialization
	protected override void Start () {
		base.Start();

		simulatedEndPos = transform.position; 
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (IsSimulated) {

			float diffScaled = Mathf.InverseLerp (clientServerDistDiffMin, clientServerDistDiffMax, clientServerDistDiff);
			float simulationPosDuration = Mathf.Lerp (simulationPosDurationMin, simulationPosDurationMax, diffScaled);

			// position
			transform.position = Vector3.SmoothDamp (transform.position, simulatedEndPos, ref Velocity, simulationPosDuration * Time.deltaTime);

			if (Velocity.magnitude < 0.05f) {
				Anim.SetBool ("Walk", false);
			}

			// body rotation yaw
			transform.rotation = Quaternion.RotateTowards (transform.rotation, simulatedBodyEndRot, simulationRotDuration * Time.deltaTime);
		}
	}
}
