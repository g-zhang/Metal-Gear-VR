using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public enum camState { def/*ault*/ = 0, sneak, hall};
	camState curCamState;

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

	int currentEdge;

	void Start() {
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		currentEdge = -1;
		defaultMainCamRotation = mainCam.transform.rotation;
		curCamState = camState.def;
	}

	void Update () {
		gameObjPos = gameObject.transform.position;
		playerPos = player.transform.position;

		// Default position
		if (curCamState == camState.def) {

			// Make sure main cam is in default position
			mainCam.transform.position = Vector3.Slerp(mainCam.transform.position, camPos, 0.2f);

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
		else if (curCamState == camState.sneak) {
			// Delay until camera is actually moving into sneakCam
			if (timeTilSneakCam < sneakCamDelay)
				timeTilSneakCam += Time.deltaTime;
			else {
				mainCam.transform.position = Vector3.Lerp (mainCam.transform.position, camPos, 0.2f);
				mainCam.transform.LookAt (sneakCamLookAtPosition);
			}
		}
	}

	public void activateSneakCam(int direction, Vector3 camMoveDirection) {
		// Set it up once and be done this is really hacky
		if (curCamState != camState.sneak || currentEdge != direction) {
			// Set states
			currentEdge = direction;
			curCamState = camState.sneak;

			// Load up camPos with init location
			camPos = camMoveDirection;
			// Bring cam down
			camPos.y -= 2.5f;

			// Figure out where sneak cam is going to look at
			sneakCamLookAtPosition = player.transform.position;

			// At left edge
			if (direction == 0) {
				// Move to the front of character
				camPos += player.transform.forward * 3;
				// Move to direction of the hallway
				camPos += (-0.5f * player.transform.right);

				sneakCamLookAtPosition += (-1 * player.transform.right);
			}
			// At right edge
			if (direction == 1) {
				// Move to the front of character
				camPos += player.transform.forward * 3;
				// Move to direction of the hallway
				camPos += (0.5f * player.transform.right);

				sneakCamLookAtPosition += player.transform.right;
			}
			// At both (1 block wide wall)
			if (direction == 3) {
				print ("both");

				camPos = (player.transform.forward * 3) + player.transform.position;
			}

			// Check if camPos will intersect a wall
			RaycastHit wallCheck;

			/*
			// Keeping this here for one commit for reference purposes
			// Check 3 meters above where cam would have moved and see if
			// 	the ray collides with anything
			if (Physics.Raycast (new Vector3 (camPos.x, camPos.y + 3, camPos.z), 
				new Vector3 (0f, -1f, 0f), out wallCheck, 4f)) {
				// If it collides with a wall, move camera higher
				if (wallCheck.collider.tag == "Wall") {
					print ("THIS CAM WILL BE IN A WALL");
					camPos.y += 2f;
				}
			}
			*/

			// Check if camPos will have a wall in between player and cam
			if (Physics.Raycast (player.transform.position, (camPos - player.transform.position),
				out wallCheck, Vector3.Magnitude(camPos - player.transform.position))) {
				// If it collides with a wall, move camera closer
				if (wallCheck.collider.tag == "Wall") {
					// print ("THIS IS GOING TO VIEW A WALL");
					camPos.x = wallCheck.point.x;
					camPos.z = wallCheck.point.z;
					camPos.y += 2f;
				}
			}
		}
//		Debug.DrawRay (new Vector3 (camPos.x, camPos.y + 3, camPos.z), 
//			new Vector3 (0f, -4f, 0f), Color.magenta);
//		Debug.DrawRay (player.transform.position, (camPos - player.transform.position), Color.cyan);
	}

	public void deactivateSneakCam() {
		curCamState = camState.def;
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		timeTilSneakCam = 0;

		mainCam.transform.rotation = defaultMainCamRotation;
	}
}