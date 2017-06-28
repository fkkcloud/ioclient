using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class MapManager : IOGameBehaviour {

	public Transform GameDestination;
	public GameObject NPCSpawnPoints;

	[HideInInspector]
	public GameObject[] NPCSpawnPointsArray;

	// Use this for initialization
	void Start () {
		NPCSpawnPointsArray = new GameObject[NPCSpawnPoints.transform.childCount];
		int i = 0;
		foreach (Transform child in NPCSpawnPoints.transform)
		{
			//child is your child transform
			NPCSpawnPointsArray[i] = child.gameObject;
			i++;
		}
	}

	public void CreateNPCBlenders(int count){
		RandomizeArray (NPCSpawnPointsArray);

		for (int i = 0; i < count; i++) 
		{
			Transform t = NPCSpawnPointsArray [i].transform;

			GameObject npcBlender = Instantiate (GlobalGameState.BlenderNPCPrefab, t.position, t.rotation);
			npcBlender.GetComponent<BlenderNPCController> ().blender.id = "npc-" + i.ToString ();

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["npcid"] = npcBlender.GetComponent<BlenderNPCController> ().blender.id;
			data ["name"] = npcBlender.GetComponent<BlenderNPCController> ().blender.id;
			data ["position"] = npcBlender.transform.position.x + "," + npcBlender.transform.position.y + "," + npcBlender.transform.position.z;
			data ["rotation"] = 0 + "," + npcBlender.transform.position.y;
			SocketIOComp.Emit("SERVER:BLENDER_NPC_CREATE", new JSONObject(data));
		}
	}

	static void RandomizeArray(GameObject[] arr)
	{
		for (int i = arr.Length - 1; i > 0; i--) 
		{
			int r = Random.Range(0,i);
			GameObject tmp = arr[i];
			arr[i] = arr[r];
			arr[r] = tmp;
		}
	}
}
