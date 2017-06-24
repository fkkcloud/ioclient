using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blender : Character {

	public Vector3 targetCharacterPosition;

	public float newestElapsedTimePosition = 0f;
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

			// position
			transform.position = Vector3.SmoothDamp (transform.position, simulatedEndPos, ref Velocity, simulationPosDamp * Time.deltaTime);

			if (Velocity.magnitude < 0.1f) {
				Anim.SetBool ("Walk", false);
			}

			// body rotation yaw
			transform.rotation = Quaternion.RotateTowards (transform.rotation, simulatedBodyEndRot, simulationRotDamp * Time.deltaTime);
		}
	}
}
