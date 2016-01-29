using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject player;
	public float cameraSpeed;
	public GameObject mainCam;

	// Time before camera moves to sneakCam
	public float sneakCamDelay;
	float timeTilSneakCam;
	Quaternion defaultMainCamRotation;
	Vector3 sneakCamLookAtPosition;

	Vector3 gameObjPos;
	Vector3 playerPos;
	Vector3 newPos;
	Vector3 camPos;

	bool sneakCam = false;
	int currentEdge;

	void Start() {
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		currentEdge = -1;
		defaultMainCamRotation = mainCam.transform.rotation;
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
				mainCam.transform.position = Vector3.Lerp (mainCam.transform.position, camPos, 0.2f);
				mainCam.transform.LookAt (sneakCamLookAtPosition);
			}
		}
	}

	// 
	public void activateSneakCam(int direction, Vector3 camMoveDirection) {
		// Set it up once and be done this is really hacky
		if (!sneakCam || currentEdge != direction) {
			//print ("Im at edge yo");
			currentEdge = direction;

			// Load up camPos with init location
			camPos = camMoveDirection;
			// Bring cam down
			camPos.y -= 2.5f;

			// Figure out where sneak cam is going to look at
			sneakCamLookAtPosition = player.transform.position;

			// At left edge
			if (direction == 0) {
				print ("left");

				// print ("camMoveDirection: " + camPos);
				// print ("player.transform.forward(" + player.transform.forward +
				//   ") * 3 = " + (player.transform.forward * 3));
				// print ("-1 * player.transform.right(" + (-1 * player.transform.right));

				// Move to the front of character
				camPos += player.transform.forward * 3;
				// Move to direction of the hallway
				camPos += (-0.5f * player.transform.right);

				sneakCamLookAtPosition += (-1 * player.transform.right);

				// print ("Edited camPos: " + camPos);
			}
			// At right edge
			if (direction == 1) {
				print ("right");

				// Move to the front of character
				camPos += player.transform.forward * 3;
				// Move to direction of the hallway
				camPos += (0.5f * player.transform.right);

				sneakCamLookAtPosition += player.transform.right;

			}
			// At both (1 block wide wall)
			if (direction == 3) {
				print ("both");

				// Move to the front of character
				camPos += player.transform.forward * 3;
			}
			sneakCam = true;
		}
	}

	public void deactivateSneakCam() {
		//print ("Im securely hidden");
		sneakCam = false;
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		timeTilSneakCam = 0;

		mainCam.transform.rotation = defaultMainCamRotation;
	}
}