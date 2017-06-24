using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Killer : Character {

	public Transform HeadTransform;

	[HideInInspector]
	public Quaternion simulatedHeadEndLocalRot;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {

		base.Update ();

		if (!IsSimulated)
			return;

		float diffScaled = Mathf.InverseLerp (clientServerDistDiffMin, clientServerDistDiffMax, clientServerDistDiff);
		float simulationPosDuration = Mathf.Lerp (simulationPosDurationMin, simulationPosDurationMax, diffScaled);

		// position
		transform.position = Vector3.SmoothDamp(transform.position, simulatedEndPos, ref Velocity, simulationPosDuration * Time.deltaTime);
		
		// body rotation yaw
		transform.rotation = Quaternion.RotateTowards(transform.rotation, simulatedBodyEndRot, simulationRotDuration * Time.deltaTime);

		// head rotation pitch
		HeadTransform.localRotation = Quaternion.RotateTowards(HeadTransform.localRotation, simulatedHeadEndLocalRot, simulationRotDuration * Time.deltaTime);
	}
}
