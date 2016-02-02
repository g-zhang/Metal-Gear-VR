using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class menuController : MonoBehaviour {

	public bool isSuccess;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.S))
		{
			if (isSuccess) {
				DestroyObject (SavedVariables.S.gameObject);
				SceneManager.LoadScene ("Level_01");
			} else {
				if (SavedVariables.S.lastSceneOpen == "Level_01")
					DestroyObject (SavedVariables.S.gameObject);
				SceneManager.LoadScene (SavedVariables.S.lastSceneOpen);
			}
		}
	}
}
