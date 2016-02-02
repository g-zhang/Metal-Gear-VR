using UnityEngine;
using System.Collections;

public class SavedVariables : MonoBehaviour {

	public static SavedVariables S;

	public string lastSceneOpen = "";

	// Use this for initialization
	void Start () {
		S = this;
		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
