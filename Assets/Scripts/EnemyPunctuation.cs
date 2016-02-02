using UnityEngine;
using System.Collections;

public enum puncType {alert = 0, question, redAlert, starKnockout, size};

public class EnemyPunctuation : MonoBehaviour {

	public static EnemyPunctuation S;

	bool isOn;
	public float appearTime;
	float timeTilOff;

	public bool billboard;

	public Sprite[] punc = new Sprite[4];
	puncType curPuncType;

	// Use this for initialization
	void Start () {
		S = this;

		// Default sprite is off
		this.GetComponent<SpriteRenderer> ().enabled = false;
		isOn = false;

		timeTilOff = appearTime;
	}
	
	// Update is called once per frame
	void Update () {
		// Have the punctuation always point to camera
		if (billboard) {
			this.transform.LookAt (CameraController.S.cameras[(int)CameraType.overhead].transform.position);
		}

		if (isOn && timeTilOff > 0f) {
			this.GetComponent<SpriteRenderer> ().enabled = true;
			timeTilOff -= Time.deltaTime;
		} else {
			this.GetComponent<SpriteRenderer> ().enabled = false;
			timeTilOff = appearTime;
			isOn = false;
		}
	}

	public void displayIcon(puncType icon) {
		this.GetComponent<SpriteRenderer> ().sprite = punc [(int)icon];
		isOn = true;
	}
}
