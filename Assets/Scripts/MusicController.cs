using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public enum musicClips {standard = 0, invincible};
	public AudioClip[] music;

	bool bigBossModeActive;

	// Use this for initialization
	void Start () {
		bigBossModeActive = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (MovementController.player.bigBossMode && !bigBossModeActive) {
			bigBossModeActive = true;
			GetComponent<AudioSource> ().clip = music [(int)musicClips.invincible];
			GetComponent<AudioSource> ().Play();
		}
		if (!MovementController.player.bigBossMode && bigBossModeActive) {
			bigBossModeActive = false;
			GetComponent<AudioSource> ().clip = music [(int)musicClips.standard];
			GetComponent<AudioSource> ().Play();
		}
	}	
}
