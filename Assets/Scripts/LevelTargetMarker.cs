﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelTargetMarker : MonoBehaviour
{
	public static LevelTargetMarker S;
    Rigidbody body;
    float initialY;

	Scene currentScene;

    public float floatSpeed = 0.5f;
    public float floatHeight = 1f;
    public float rotateSpeed = 5f;

	void Awake() {
		currentScene = SceneManager.GetActiveScene ();

		S = this;
	}

    // Use this for initialization
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody>();
        initialY = body.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //make the marker hover up and down
        Vector3 tempPos = body.transform.position;
        tempPos.y = initialY + floatHeight * Mathf.Sin(floatSpeed * Time.time);
        body.transform.position = tempPos;

        //rotate marker
        body.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        quickLevelLoad();
    }

    void OnTriggerEnter(Collider other)
    {
        print("Triggered: " + other.gameObject.name);
        //level completion code when collider is the player
		if (other.gameObject.name == "Snake") {
			//SceneManager.LoadScene(Application.loadedLevel + 1);
			SceneManager.LoadScene (currentScene.buildIndex + 1);
		}
    }
		
	public void activateGameOver() {
		print ("Game over man ):");
		SavedVariables.S.lastSceneOpen = currentScene.name;
		SceneManager.LoadScene ("GameOver");
	}

    void quickLevelLoad()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
			DestroyObject (SavedVariables.S.gameObject);
            SceneManager.LoadScene("Level_01");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SceneManager.LoadScene("Level_02");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SceneManager.LoadScene("Level_03");
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            SceneManager.LoadScene("Level_C1");
        }
        if (Input.GetKey(KeyCode.Alpha5))
        {
            SceneManager.LoadScene("Level_C2");
        }
    }
}