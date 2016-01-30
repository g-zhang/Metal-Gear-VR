using UnityEngine;
using System.Collections;

public class IconBehavior : MonoBehaviour {

	public GameObject player;
	public bool billboard;
//	public int flickerPerSecond;
//	public float offTime;

//	float timeTilFlicker = 0;
//	float timeTilOn = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);

		if (billboard) {
			transform.LookAt (CameraController.S.cameras[(int)CameraType.overhead].transform.position);
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
