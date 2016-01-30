using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {
	public enum enemyState { def/*ault*/ = 0, alert, searching};
	enemyState curEnemyState;

    public GameObject nextPoint;
    Rigidbody body;
	public float rotationSpeed = 0.1f;
    NavMeshAgent agn;

	public float pauseTime = 3f;
	public float timeTilMove;
	GameObject currentPoint;

	public AudioSource alertSound;

	Vector3 pointDirection;

	bool alertSoundPlayed = false;

	Vector3 soundLocation;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody>();
        agn = gameObject.GetComponent<NavMeshAgent>();
		currentPoint = nextPoint;
		soundLocation = Vector3.zero;
		curEnemyState = enemyState.def;
	}
	
	// Update is called once per frame
	void Update () {
		// Default State
		if (curEnemyState == enemyState.def) {
			// Once the point changes...
			if (currentPoint != nextPoint) {
				// Start countdown timer
				timeTilMove = pauseTime;
				currentPoint = nextPoint;
			}

			// Count down until time to move
			if (timeTilMove > 0) {
				body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, pointDirection, rotationSpeed));
				timeTilMove -= Time.deltaTime;
			} 
			// Once timer runs down...
			else {
				Vector3 targetDir = nextPoint.transform.position - transform.position;
				if (Vector3.Angle (body.transform.forward, targetDir) > 45f) {
					body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, targetDir, rotationSpeed));
				} else {
					agn.destination = nextPoint.transform.position;
				}
			}
		} 
		// Alerted State
		else if (curEnemyState == enemyState.alert) {


		}
		// Searching State
		else if (curEnemyState == enemyState.searching) {

		}

		print (curEnemyState);

		// UNIVERSAL FOR ALL STATES

		// Enemy vision
		// If player is within the 90 degree vision cone and is 4 away...
		Vector3 toPlayer = MovementController.player.transform.position - gameObject.transform.position;
		if (Vector3.Angle (toPlayer, gameObject.transform.forward) < 45 && toPlayer.magnitude < 4) {
			// If player is not hidden behind something...
			RaycastHit hit;
			Debug.DrawRay (transform.position, toPlayer);
			if (Physics.Raycast (transform.position, toPlayer, out hit)) {
				if (hit.collider.name == "Snake") {
					if (!alertSoundPlayed) {
						print ("I SEE YOU!");
						alertSound.Play ();
						alertSoundPlayed = true;
					}
					agn.Stop ();
				}
			}
		} else {
			agn.Resume ();
			alertSoundPlayed = false;
		}

		// Enemy Hearing
		// Check to see if player is knocking
		print (HandController.S.isCurrentlyKnocking ());
		if (HandController.S.isCurrentlyKnocking ()) {
			
			soundLocation = MovementController.player.transform.position;

			// Check distance between self and sound location
			if (Vector3.Magnitude (soundLocation - body.transform.position) <= 5f) {
				// If the player made a sound withing 5 meters of enemy
				//	then make enemy alerted
				Debug.DrawLine (body.transform.position, soundLocation, Color.green, 1f);
				print ("What was that noise?");
				agn.destination = soundLocation;
				curEnemyState = enemyState.alert;
			} else {
				Debug.DrawLine (body.transform.position, soundLocation, Color.red, 1f);
			}
		}

	}

    public void SetNext(GameObject next)
    {
        nextPoint = next;
    }

	public void SetDirection (Vector3 dir) 
	{
		pointDirection = dir;
	}
}
