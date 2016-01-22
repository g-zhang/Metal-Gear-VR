using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject player;
	public float cameraSpeed;

	Vector3 camPos;
	Vector3 playerPos;

	void Update () {
		// camPos + playerPos are just shortcuts
		camPos = gameObject.transform.position;
		playerPos = player.transform.position;

		// Position of camera after checks
		Vector3 newPos = camPos;
		newPos.y = playerPos.y;

		// Set up new position of camera
		// If player is 1.5 meters left of the center
		if (camPos.x - playerPos.x > 0.5f) {
			newPos.x = playerPos.x + 0.5f;
		}
		// If player is 1.5 meters right of the center
		if (camPos.x - playerPos.x < -0.5f) {
			newPos.x = playerPos.x - 0.5f;
		}
		// If player is 1.5 meters below center
		if (camPos.z - playerPos.z > 0.5f) {
			newPos.z = playerPos.z + 0.5f;
		}
		// If player is 1.5 meters above center
		if (camPos.z - playerPos.z < -0.5f) {
			newPos.z = playerPos.z - 0.5f;
		}

		// Move camera
		gameObject.transform.position = newPos;
	}
}
