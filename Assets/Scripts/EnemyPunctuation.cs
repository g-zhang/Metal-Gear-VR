using UnityEngine;
using System.Collections;

public class EnemyPunctuation : MonoBehaviour {

	bool isOn;
	public float appearTime;
	float timeTilOff;

	public bool billboard;

	public enum puncType {alert = 0, question, redAlert, size};
	public Sprite[] punc = new Sprite[(int)puncType.size];
	puncType curPuncType;

	// Use this for initialization
	void Start () {
		// Default sprite is off
		GetComponent<SpriteRenderer> ().enabled = false;
		isOn = false;

		timeTilOff = appearTime;
	}
	
	// Update is called once per frame
	void Update () {
		// Have the punctuation always point to camera
		if (billboard) {
			transform.LookAt (CameraController.S.cameras[(int)CameraType.overhead].transform.position);
		}

		if (isOn && timeTilOff > 0f) {
			GetComponent<SpriteRenderer> ().enabled = true;
			timeTilOff -= Time.deltaTime;
		} else {
			GetComponent<SpriteRenderer> ().enabled = false;
			isOn = false;
		}
	}
}
