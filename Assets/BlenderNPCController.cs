using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlenderNPCController : MonoBehaviour {
	
	[HideInInspector]
	public NavMeshAgent navMeshAgent;

	[HideInInspector]
	public Blender blender;

	public MapManager mapManager;

	// Use this for initialization
	void Start () {
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.isStopped = true;

		blender = GetComponent<Blender> ();

		navMeshAgent.SetDestination (mapManager.GameDestination.position);
		StartCoroutine (StartWalking (1f));
	}

	IEnumerator StartWalking(float restTime)
	{
		yield return new WaitForSeconds(restTime);
		navMeshAgent.isStopped = false;
	}

	/*
		1. let these 3 functions to play randomly occur and simulated! - test blencer controller for player to find who is the right zombie player
		2. when player get pushed from these AI, it also should update to server for broadcast!!
	*/


	void TakeRest(){
		navMeshAgent.isStopped = true;
		float restTime = Random.Range (0.2f, 3.4f);
		StartCoroutine (StartWalking (restTime));
	}

	void ChangeRotation(){
		// choose random position among the region (battle ground shrinking area)
		navMeshAgent.SetDestination (mapManager.GameDestination.position);
	}
	
	// Update is called once per frame
	void Update () {

		float threshold = navMeshAgent.speed * 0.1f;

		if (navMeshAgent.velocity.magnitude < threshold) {
			blender.Anim.SetBool ("Walk", false);
		} else if (navMeshAgent.velocity.magnitude > threshold && blender.Anim.GetBool("Walk") == false){
			blender.Anim.SetBool ("Walk", true);
		}
	}
}
