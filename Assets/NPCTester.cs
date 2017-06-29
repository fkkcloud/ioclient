using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTester : IOGameBehaviour {

	public void OnChangedBool(bool isOn){
		GlobalGameState.IsNPCBlenderMaster = isOn;
	}
}
