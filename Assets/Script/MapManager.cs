using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class MapManager : IOGameBehaviour {

	public Transform GameDestination;

	[HideInInspector]
	public MapData mapData;

	[HideInInspector]
	public GameObject[] NPCSpawnPointsArray;


	public void Init(MapData mapDataArg){
		mapData = mapDataArg;
		NPCSpawnPointsArray = new GameObject[mapData.NPCSpawnPointParent.transform.childCount];
		int i = 0;
		foreach (Transform child in mapData.NPCSpawnPointParent.transform)
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

			string npcid = "npcid_" + i.ToString ();
			npcBlender.GetComponent<BlenderNPCController> ().blender.id = npcid;
			npcBlender.GetComponent<Blender> ().IsNPC = true;

			Dictionary<string, string> data = new Dictionary<string, string> ();
			data ["npcid"] = npcid;
			data ["name"] = npcid;
			data ["position"] = npcBlender.transform.position.x + "," + npcBlender.transform.position.y + "," + npcBlender.transform.position.z;
			data ["rotation"] = 0 + "," + npcBlender.transform.position.y;
			SocketIOComp.Emit("SERVER:BLENDER_NPC_CREATE", new JSONObject(data));

			GlobalGameState.BlenderNPCs.Add(npcBlender.GetComponent<Blender>());
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
