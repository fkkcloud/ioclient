using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blender : Character {

	// Use this for initialization
	protected override void Start () {
		base.Start();

		simulatedEndPos = transform.position; 
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		// position
		transform.position = Vector3.SmoothDamp(transform.position, simulatedEndPos, ref Velocity, simulationPosDamp * Time.deltaTime);

		if (!IsSimulated)
			return;

		// body rotation yaw
		transform.rotation = Quaternion.RotateTowards(transform.rotation, simulatedBodyEndRot, simulationRotDamp * Time.deltaTime);
	}
}
