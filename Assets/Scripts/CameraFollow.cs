using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject player;
	public float cameraSpeed;
	public GameObject mainCam;

	// Time before camera moves to sneakCam
	public float sneakCamDelay;
	float timeTilSneakCam;

	Vector3 gameObjPos;
	Vector3 playerPos;
	Vector3 newPos;
	Vector3 camPos;

	bool sneakCam = false;
	int currentEdge;

	void Start() {
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		currentEdge = -1;
	}

	void Update () {
		// camPos + playerPos are just shortcuts
		gameObjPos = gameObject.transform.position;
		playerPos = player.transform.position;

		// Default position
		if (!sneakCam) {

			// Make sure main cam is in default position
			mainCam.transform.position = Vector3.Slerp(mainCam.transform.position, camPos, 0.2f);

//			Quaternion targetRotation = Quaternion.LookRotation (transform.position - mainCam.transform.position);
//			float str = Mathf.Min (0.5f * Time.deltaTime, 1f);
//			mainCam.transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, str);


			// Position of cameraContainer after checks
			newPos = gameObjPos;
			newPos.y = playerPos.y;

			// Set up new position of cameraContainer
			// If player is 1.5 meters left of the center
			if (gameObjPos.x - playerPos.x > 0.5f) {
				newPos.x = playerPos.x + 0.5f;
			}
			// If player is 1.5 meters right of the center
			if (gameObjPos.x - playerPos.x < -0.5f) {
				newPos.x = playerPos.x - 0.5f;
			}
			// If player is 1.5 meters below center
			if (gameObjPos.z - playerPos.z > 0.5f) {
				newPos.z = playerPos.z + 0.5f;
			}
			// If player is 1.5 meters above center
			if (gameObjPos.z - playerPos.z < -0.5f) {
				newPos.z = playerPos.z - 0.5f;
			}

			// Move camera
			gameObject.transform.position = newPos;
		} 
		// Sneak cam position
		else {
			// Delay until camera is actually moving into sneakCam
			if (timeTilSneakCam < sneakCamDelay)
				timeTilSneakCam += Time.deltaTime;
			else {
				mainCam.transform.position = Vector3.Slerp (mainCam.transform.position, camPos, 0.2f);
			}
		}
	}

	// 
	public void activateSneakCam(int direction, Vector3 camMoveDirection) {
		// Set it up once and be done this is really hacky
		if (!sneakCam || currentEdge != direction) {
			print ("Im at edge yo");
			currentEdge = direction;
			// At left edge
			if (direction == 0) {
				print ("left");
				camPos = new Vector3 (playerPos.x - 3f, playerPos.y, playerPos.z - 3f);
			}
			// At right edge
			if (direction == 1) {
				print ("right");
				camPos = new Vector3 (playerPos.x - 3f, playerPos.y, playerPos.z - 3f);
			}
			// At both (1 block wide wall)
			if (direction == 3) {
				print ("both");
				camPos = new Vector3 (playerPos.x - 3f, playerPos.y, playerPos.z - 3f);
			}
			sneakCam = true;

			camPos = camMoveDirection;
		}
	}

	public void deactivateSneakCam() {
		//print ("Im securely hidden");
		sneakCam = false;
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		timeTilSneakCam = 0;
	}
}