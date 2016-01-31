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
	public Quaternion defaultMainCamRotation;
	Quaternion hallMainCamRotation;
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
		// lol i cheated oops
		hallMainCamRotation = new Quaternion (0.7372774f, 0f, 0f, 0.6755902f);
		curCamState = camState.def;
	}

	void Update () {
		gameObjPos = gameObject.transform.position;
		playerPos = player.transform.position;


		// Sneak cam position
		if (curCamState == camState.sneak) {
			// Delay until camera is actually moving into sneakCam
			if (timeTilSneakCam < sneakCamDelay)
				timeTilSneakCam += Time.deltaTime;
			else {
				mainCam.transform.position = Vector3.Lerp (mainCam.transform.position, camPos, 0.2f);
				mainCam.transform.LookAt (sneakCamLookAtPosition);
			}
		} 
		// Default + hall position
		else {

			if (isInHall ())
				activateHallCam ();
			else
				deactivateHallCam ();

			// Make sure main cam is in default position
			mainCam.transform.position = Vector3.Lerp (mainCam.transform.position, camPos, 0.2f);

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

			// Check if camPos will have a wall in between player and cam
			if (Physics.Raycast (player.transform.position, (camPos - player.transform.position),
				out wallCheck, Vector3.Magnitude(camPos - player.transform.position))) {
				// If it collides with a wall, move camera closer
				if (wallCheck.collider.tag == "Wall") {
					camPos.x = wallCheck.point.x;
					camPos.z = wallCheck.point.z;
					camPos.y += 2f;
				}
			}
		}
	}

	public void deactivateSneakCam() {
		curCamState = camState.def;
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);
		timeTilSneakCam = 0;

		mainCam.transform.rotation = defaultMainCamRotation;
	}

	public bool isInHall() {
		// Raycast in 4 directionsby 1.5 meters
		bool up, down, left, right;
		RaycastHit upHit, downHit, leftHit, rightHit;
		bool inHall = false;

		Debug.DrawRay (player.transform.position, new Vector3 (0f, 0f, 1f), Color.blue);
		Debug.DrawRay (player.transform.position, new Vector3 (0f, 0f, -1f), Color.red);
		Debug.DrawRay (player.transform.position, new Vector3 (-1f, 0f, 0f), Color.cyan);
		Debug.DrawRay (player.transform.position, new Vector3 (1f, 0f, 0f), Color.green);

		up = Physics.Raycast (player.transform.position, new Vector3(0f, 0f, 1f), out upHit);
		down = Physics.Raycast (player.transform.position, new Vector3(0f, 0f, -1f), out downHit);
		left = Physics.Raycast (player.transform.position, new Vector3(-1f, 0f, 0f), out leftHit);
		right = Physics.Raycast (player.transform.position, new Vector3(1f, 0f, 0f), out rightHit);

		// If up and down hit things
		if (up && down) {
			if ((upHit.distance + downHit.distance) > 1.5f &&
			   (upHit.distance + downHit.distance) < 2.5f) {
				inHall = true;
			}
		}

		// If left and right hit things
		if (left && right) {
			if ((leftHit.distance + rightHit.distance) > 1.5f &&
			    (leftHit.distance + rightHit.distance) < 2.5f) {
				inHall = true;
			}
		}

		if (inHall) {
			// Only return true if there's a wall "down"
			return (downHit.collider.tag == "Wall");
		}
		return false;

	}
	public void activateHallCam() {
		curCamState = camState.hall;
		camPos = transform.position + new Vector3 (0f, 6.5f, -0.2f);
		mainCam.transform.rotation = Quaternion.Lerp(mainCam.transform.rotation, hallMainCamRotation, 0.1f);
	}
	public void deactivateHallCam() {
		curCamState = camState.def;
		camPos = transform.position + new Vector3 (0f, 6f, -1.5f);

		mainCam.transform.rotation = defaultMainCamRotation;
	}
}