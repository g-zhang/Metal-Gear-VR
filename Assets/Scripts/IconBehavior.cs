using UnityEngine;
using System.Collections;

public class IconBehavior : MonoBehaviour {

	public GameObject player;
	public bool isPlayerSightCone;
//	public int flickerPerSecond;
//	public float offTime;

//	float timeTilFlicker = 0;
//	float timeTilOn = 0;

	// Use this for initialization
	void Start () {
		if (isPlayerSightCone) {
			gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);

		if (isPlayerSightCone) {
			if (MovementController.player.FPVModeControl) {
				gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}
			else {
				gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
		}

//		// If it's time to flicker
//		if (timeTilFlicker <= 0) {
//			// If it should still be off
//			if (timeTilOn <= offTime) {
//				this.GetComponent<SpriteRenderer> ().enabled = false;
//				timeTilOn += Time.deltaTime;
//			} 
//			// If it shoudl turn back on
//			else {
//				timeTilFlicker = 1/flickerPerSecond - offTime;
//				timeTilOn = 0;
//				this.GetComponent<SpriteRenderer> ().enabled = true;
//			}
//		} else {
//			timeTilFlicker -= Time.deltaTime;
//		}
	}
}
