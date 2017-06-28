using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SocketIO;

/*
	walking ( ) 
	— 스테이트 체인지 를 알리고, 계속 걷는거 sync
	resting ( ) 
	— 스테이트 체인지를 알리고, no position sync
	attacking - 이 걸렸을때는 다음 스테이트 시동 걸지말고,  attacking state에서 애니메이션이 끝나면 걸어 줘야 한다.
	— 스테이트 체인지를 알리고, no position sync
	rotating - ( )
	— 스테이트 체인지를 알리고, no position sync
	died - 다음 스테이트 걸지 않는다. - (터져버리는 애니메이션)
	— 이경우만 모든 다른 클라이언트에서 다른 클라이언트로 전달할수 있다.
	eat ing- 가끔 eat를 해줘야 한다. (  ) 이 컬렸을때는 attacking 처럼 eating 애니메이셔이 끝나면 바로 다른 랜덤 스테이트 고고
	— 스테이트 체인지를 알리고, no position sync
*/

public class BlenderNPCController : IOGameBehaviour {

	[HideInInspector]
	public NavMeshAgent navMeshAgent;

	public Blender blender;

	Quaternion prevRot;

	// 0:Dead, 1:Idle, 2:Walking, 3:Rotating, 4:Eating
	public enum BlenderState {Dead, Idle, Walking, Rotating, Eating};

	public BlenderState CurrentBlenderState = BlenderState.Idle;

	/*
	 * TODO:
		1. when player get pushed from these AI, it also should update to server for broadcast!!
	*/

	// Use this for initialization
	void Start () {
		if (GlobalGameState.IsNPCZombieMaster)
			Init ();
	}

	public void Init(){
		Debug.Log (blender.id);

		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.isStopped = true;
		navMeshAgent.SetDestination (GlobalMapManager.GameDestination.position);

		prevRot = gameObject.transform.rotation;

		StartCoroutine (ChangeAction (1f));
	}

	IEnumerator ChangeAction(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		CurrentBlenderState = (BlenderState)Random.Range(1, 4); // inclusive | exclusive

		//Debug.Log ("Changed Action: " + CurrentBlenderState);

		switch (CurrentBlenderState) 
		{
		case BlenderState.Walking:
			DoWalking ();
			break;
		case BlenderState.Idle:
			DoResting ();
			break;
		case BlenderState.Rotating:
			DoChangeRotation ();
			break;
			/*
		case BlenderState.Eating:
			DoEating ();
			break;*/
		default:
			break;
		}

		if (CurrentBlenderState != BlenderState.Dead &&
			CurrentBlenderState != BlenderState.Eating) 
		{
			float nextActionDue = Random.Range (1.5f, 4f);
			StartCoroutine (ChangeAction (nextActionDue));
		}
	}

	void DoWalking()
	{
		navMeshAgent.isStopped = false;
	}

	void DoResting(){
		
		navMeshAgent.isStopped = true;
	}

	void DoChangeRotation(){
		// choose random position among the region (battle ground shrinking area)
		float randomAngle = Random.Range(0f, 360f);
		float randomDist = Random.Range (0f, 15f);

		Vector3 blenderDestination = GlobalMapManager.GameDestination.position + new Vector3 (Mathf.Cos (randomAngle) * randomDist, 0f, Mathf.Sin (randomAngle) * randomDist);

		navMeshAgent.SetDestination (blenderDestination);
		navMeshAgent.isStopped = false;
	}

	void DoEating(){
		StartCoroutine (EatSequence ());
	}

	IEnumerator EatSequence(){
		blender.Anim.SetBool ("Eat", true);

		float nextActionDue = Random.Range (1.5f, 4f);

		StartCoroutine (ChangeAction (nextActionDue));

		// start cancel eating a bit faster than ChangeAction!
		yield return new WaitForSeconds (nextActionDue - 0.2f);

		blender.Anim.SetBool ("Eat", false);
	}
	
	void Update () {

		/*
		 * TODO: this should control any clients NPC's action handle. e.g. eating, attacking and stuff
		 */

		if (!GlobalGameState.IsNPCZombieMaster)
			return;

		float threshold = navMeshAgent.speed * 0.1f;
		if (navMeshAgent.velocity.magnitude < threshold) 
		{
			blender.Anim.SetBool ("Walk", false);
		} 
		else if (navMeshAgent.velocity.magnitude > threshold && blender.Anim.GetBool("Walk") == false)
		{
			blender.Anim.SetBool ("Walk", true);
		}

		if (!navMeshAgent.isStopped) {
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["position"] = gameObject.transform.position.x + "," + gameObject.transform.position.y + "," + gameObject.transform.position.z;
			data ["elapsedTime"] = Time.timeSinceLevelLoad.ToString();
			data ["npcid"] = blender.id.ToString ();
			SocketIOComp.Emit("SERVER:BLENDER_NPC_MOVE", new JSONObject(data));
		}

		if (prevRot != gameObject.transform.rotation) {
			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["rotation"] = 0 + "," + gameObject.transform.rotation.eulerAngles.y;
			data ["elapsedTime"] = Time.timeSinceLevelLoad.ToString();
			data ["npcid"] = blender.id.ToString ();
			SocketIOComp.Emit("SERVER:BLENDER_NPC_ROTATE", new JSONObject(data));
			
			prevRot = gameObject.transform.rotation;
		}
	}
}
