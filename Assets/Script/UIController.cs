using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : IOGameBehaviour {

	public GameObject UIObject;

	public virtual void Show(){
		UIObject.SetActive (true);
	}

	public virtual void Hide(){
		UIObject.SetActive (false);
	}
}
