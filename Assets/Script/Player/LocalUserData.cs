using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalUserData : MonoBehaviour {

	static string KeyUserName = "UserName";

	static public string GetUserName(){
		if (PlayerPrefs.HasKey (KeyUserName) && PlayerPrefs.GetString (KeyUserName) != "")
			return PlayerPrefs.GetString (KeyUserName);
		else
			return "";
	}

	static public void SetUserName(string UserName){
		PlayerPrefs.SetString (KeyUserName, UserName);
	}

}
