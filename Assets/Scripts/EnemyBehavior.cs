using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public GameObject nextPoint;
    Rigidbody body;
	public float rotationSpeed = 0.1f;
    NavMeshAgent agn;

	public float pauseTime = 3f;
	public float timeTilMove;
	GameObject currentPoint;

	public AudioSource alertSound;

	Vector3 pointDirection;

	bool alerted = false;
	bool alertSoundPlayed = false;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody>();
        agn = gameObject.GetComponent<NavMeshAgent>();
		currentPoint = nextPoint;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Once the point changes...
		if (currentPoint != nextPoint) {
			// Start countdown timer
			timeTilMove = pauseTime;
			currentPoint = nextPoint;
		}

		// Count down until time to move
		if (timeTilMove > 0) {
			body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, pointDirection, rotationSpeed));
			timeTilMove -= Time.deltaTime;
		} 
		// Once timer runs down...
		else {
			Vector3 targetDir = nextPoint.transform.position - transform.position;
			if (Vector3.Angle (body.transform.forward, targetDir) > 45f) {
				body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, targetDir, rotationSpeed));
			} else {
				agn.destination = nextPoint.transform.position;
			}
		}
			
        // Enemy vision

		// If player is within the 90 degree vision cone and is 4 away...
        Vector3 toPlayer = MovementController.player.transform.position - gameObject.transform.position;
		if (Vector3.Angle (toPlayer, gameObject.transform.forward) < 45 && toPlayer.magnitude < 4) {
			// If player is not hidden behind something...
			RaycastHit hit;
			//Vector3 fwd = transform.TransformDirection (Vector3.forward);
			Debug.DrawRay (transform.position, toPlayer);
			if (Physics.Raycast (transform.position, toPlayer, out hit)) {
				print (hit.collider.name);
				if (hit.collider.name == "Snake") {
					//Application.LoadLevel("Main");
					if (!alertSoundPlayed) {
						alertSound.Play ();
						alertSoundPlayed = true;
					}
					print ("I SEE YOU!");
					agn.Stop ();
				}
			}
		} else {
			agn.Resume ();
			alertSoundPlayed = false;
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
