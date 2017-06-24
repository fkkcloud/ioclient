using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlenderNPCController : IOGameBehaviour {
	
	[HideInInspector]
	public NavMeshAgent navMeshAgent;

	[HideInInspector]
	public Blender blender;

	public MapManager mapManager;

	public enum BlenderState {Walking, Resting, Rotating, Attacking, Dead, Born};

	public BlenderState CurrentBlenderState = BlenderState.Born;


	/*
	 * TODO:
		1. when player get pushed from these AI, it also should update to server for broadcast!!
	*/

	// Use this for initialization
	void Start () {
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.isStopped = true;

		blender = GetComponent<Blender> ();

		navMeshAgent.SetDestination (mapManager.GameDestination.position);
		StartCoroutine (ChangeAction (1f));
	}

	IEnumerator ChangeAction(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		CurrentBlenderState = (BlenderState)Random.Range(0, 3); // inclusive | exclusive so it choose between 0, 1, 2

		Debug.Log ("Changed Action: " + CurrentBlenderState);

		switch (CurrentBlenderState) 
		{
		case BlenderState.Walking:
			StartWalking ();
			break;
		case BlenderState.Resting:
			TakeRest ();
			break;
		case BlenderState.Rotating:
			ChangeRotation ();
			break;
		case BlenderState.Attacking:
			// do a attack action
			break;
		default:
			break;
		}

		if (CurrentBlenderState != BlenderState.Dead) 
		{
			float nextActionDue = Random.Range (1.5f, 4f);
			StartCoroutine (ChangeAction (nextActionDue));
		}
	}

	void StartWalking()
	{
		if (GlobalGameState.IsNPCZombieMaster) {
			// send server info
		}

		navMeshAgent.isStopped = false;
	}

	void TakeRest(){
		if (GlobalGameState.IsNPCZombieMaster) {
			// send server info
		}
		navMeshAgent.isStopped = true;
	}

	void ChangeRotation(){
		// choose random position among the region (battle ground shrinking area)
		float randomAngle = Random.Range(0f, 360f);
		float randomDist = Random.Range (0f, 15f);

		Vector3 blenderDestination = mapManager.GameDestination.position + new Vector3 (Mathf.Cos (randomAngle) * randomDist, 0f, Mathf.Sin (randomAngle) * randomDist);

		if (GlobalGameState.IsNPCZombieMaster) {
			// send server info
		}

		navMeshAgent.SetDestination (blenderDestination);
	}
	
	void Update () {

		float threshold = navMeshAgent.speed * 0.1f;


		if (navMeshAgent.velocity.magnitude < threshold) {
			blender.Anim.SetBool ("Walk", false);
		} else if (navMeshAgent.velocity.magnitude > threshold && blender.Anim.GetBool("Walk") == false){
			blender.Anim.SetBool ("Walk", true);
		}


		if (!navMeshAgent.isStopped) {
			if (GlobalGameState.IsNPCZombieMaster) {
				// send server info
			}
		}
	}
}
