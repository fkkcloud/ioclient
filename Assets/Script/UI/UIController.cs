using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : IOGameBehaviour {

	public GameObject[] UIObjects;

	public virtual void Show(){
		foreach (GameObject go in UIObjects) {
			go.SetActive (true);
		}

	}

	public virtual void Hide(){
		foreach (GameObject go in UIObjects) {
			go.SetActive (false);
		}
	}
}
