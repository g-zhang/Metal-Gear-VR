using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {
	
	public enum enemyState { def/*ault*/ = 0, investigating, searching, knockout, stun};
	enemyState curEnemyState;

	public enum radar {sight = 0, alert};
	public SpriteRenderer radarIcon;
	public SpriteRenderer sightConeRenderer;
	public Sprite[] radarSprites;

    public GameObject nextPoint;
    Rigidbody body;
	public float rotationSpeed = 0.1f;
    NavMeshAgent agn;

	public float pauseTime = 3f;
	float timeTilMove;
	GameObject currentPoint;

	Vector3[] pointDirection;
	int pointDirectionSize;
	int directionsTurned;
	public bool staysSamePoint;

	public bool ____________________;
	public AudioSource alertSound;
	bool alertSoundPlayed = false;
	public float alertLookTime;
	float timeTilTurn;
	Vector3 turnDirection;
	int numTurns; // how many turns performed
	Vector3 soundLocation;

	public bool ______________________;
	// Knock out variables
	public AudioSource voiceSource;
	public enum voice {enemyNoiseAlert = 0, enemyPunched, enemyFlip, confused, foundSnake};
	public AudioClip[] voiceClips;
	public int curStamina;
	int maxStamina;
	public float downTime;
	float timeTilUp;

	public float stunTime;
	float timeTilNoStun;

	// Use this for initialization
	void Start () {
		directionsTurned = 0;
		pointDirectionSize = 0;

        body = gameObject.GetComponent<Rigidbody>();
        agn = gameObject.GetComponent<NavMeshAgent>();
		currentPoint = nextPoint;
		soundLocation = Vector3.zero;
		curEnemyState = enemyState.def;
		numTurns = 0;
		timeTilTurn = 0f;

		maxStamina = 7;
		curStamina = maxStamina;
		timeTilUp = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		agn.enabled = true; 
		// print (this.name + " init directionsTurned: " + (directionsTurned));

		// Knockout State
		if (curEnemyState == enemyState.knockout) {
			if (timeTilUp < downTime) {
				// Make them look like they fell over
				this.transform.localScale = new Vector3 (0.75f, 0.375f, 1.5f);
				radarIcon.transform.localScale = new Vector3 (0.067f, 0.1333f, 0.6667f);

				if (curStamina <= 0)
					EnemyPunctuation.S.displayIcon (puncType.starKnockout);

				// Turn of sight cone
				sightConeRenderer.enabled = false;
				agn.Stop ();

				timeTilUp += Time.deltaTime;
			}
			else {
				// Stand back up, loser
				this.transform.localScale = new Vector3 (0.75f, 1.5f, 0.375f);
				radarIcon.transform.localScale = new Vector3 (0.2667f, 0.1333f, 0.6667f);

				timeTilUp = 0f;
				agn.Resume ();
				if (curStamina <= 0)
					curStamina = 7; // Restore stamina
				sightConeRenderer.enabled = true;
				curEnemyState = enemyState.searching;
			}
		} 
		else if (curStamina <= 0) {
			playVoice (voice.enemyFlip);
			curEnemyState = enemyState.knockout;
			EnemyPunctuation.S.displayIcon (puncType.starKnockout);
			print ("IM DOWN");
		} 

		// DamagedState
		else if (curEnemyState == enemyState.stun) {
			if (timeTilNoStun < stunTime) {
				agn.Stop ();
				timeTilNoStun += Time.deltaTime;
			}
			else {
				agn.Resume();
				timeTilNoStun = 0f;
				curEnemyState = enemyState.searching;
			}
		}

		// Knockout takes priority lol sorry for the bad code
		else {
			// Default State
			if (curEnemyState == enemyState.def) {
				sightConeRenderer.sprite = radarSprites [(int)radar.sight];

				// If player "touches" enemy, make enemy start searching
				if (Vector3.Magnitude (transform.position - MovementController.player.transform.position) <= 0.5f)
					curEnemyState = enemyState.searching;

				// Once the point changes...
				if (currentPoint != nextPoint) {
					// Start countdown timer
					timeTilMove = pauseTime;
					currentPoint = nextPoint;
				}

				// Count down until time to move
				if (timeTilMove > 0) {
					if (staysSamePoint)
						body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, pointDirection[directionsTurned - 1], rotationSpeed));
					else 
						body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, pointDirection[directionsTurned], rotationSpeed));
					timeTilMove -= Time.deltaTime;
				} 
			// Once timer runs down...
				else {
					// If it hasn't gone through all the turns
					if (directionsTurned < (pointDirectionSize - 1) || (staysSamePoint && directionsTurned < pointDirectionSize)) {
						timeTilMove = pauseTime;
						directionsTurned += 1;
					} else {
						directionsTurned = 0;
						Vector3 targetDir = nextPoint.transform.position - transform.position;
						if (Vector3.Angle (body.transform.forward, targetDir) > 45f) {
							body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, targetDir, rotationSpeed));
						} else {
							agn.destination = nextPoint.transform.position;
						}
					}
				}
			} 

		// Alerted State
		else if (curEnemyState == enemyState.investigating) {
				// Change sightcone red
				sightConeRenderer.sprite = radarSprites [(int)radar.alert];

				// Once you reach the sound location (with some bounce room)
				if (Vector3.Magnitude (transform.position - agn.destination) <= 0.8f) {
					// Go into search state
					curEnemyState = enemyState.searching;
				}
			}

		// Searching State
		else if (curEnemyState == enemyState.searching) {
				// Stop moving
				agn.enabled = false;

				// Change sightcone red
				sightConeRenderer.sprite = radarSprites [(int)radar.alert];

				if (numTurns <= 3) {
					if (timeTilTurn < alertLookTime)
						timeTilTurn += Time.deltaTime;
					else {
						print ("Rotating...");
						if (numTurns == 0) {
							turnDirection = body.transform.right;
							playVoice (voice.confused);
							EnemyPunctuation.S.displayIcon (puncType.question);
						}
						if (numTurns == 1)
							turnDirection = body.transform.forward * -1;
						if (numTurns == 2) {
							turnDirection = body.transform.right;
							sightConeRenderer.sprite = radarSprites [(int)radar.sight];
						}
						numTurns++;
						timeTilTurn = 0f;
					}
					body.transform.rotation = Quaternion.LookRotation (Vector3.Slerp (body.transform.forward, turnDirection, rotationSpeed));
				} else {
					numTurns = 0;
					curEnemyState = enemyState.def;
					currentPoint = nextPoint;
				}
			}
			// UNIVERSAL FOR ALL STATES

			// Make player invincible
			if (!MovementController.player.bigBossMode) {
				// If player enters sight cone, game over
				EnemySight ();
			}

			// If player makes noise, investigate
			EnemyHearing ();
		}
	}

	void EnemySight() {
		// If player is within the 90 degree vision cone and is 4 away...
		Vector3 toPlayer = MovementController.player.transform.position - gameObject.transform.position;
		if (Vector3.Angle (toPlayer, gameObject.transform.forward) < 45 && toPlayer.magnitude < 4 && !MovementController.player.FPVModeCrawlControl) {
			// If player is not hidden behind something...
			RaycastHit hit;

			// Have the ray start from the enemy's "head"
			Vector3 enemyHeadLocation = transform.position;
			enemyHeadLocation.y += 0.65f;

			Debug.DrawRay (enemyHeadLocation, toPlayer);

			if (Physics.Raycast (enemyHeadLocation, toPlayer, out hit)) {
				if (hit.collider.name == "Snake") {
					activateGameOver ();
				}
			}
		} else { 
			agn.Resume ();
			alertSoundPlayed = false;
		}
	}

	void EnemyHearing() {
		// Check to see if player is knocking only if not searching
		if (curEnemyState != enemyState.searching) {
			if (HandController.S.isCurrentlyKnocking ()) {

				soundLocation = MovementController.player.transform.position;

				// Check distance between self and sound location
				if (Vector3.Magnitude (soundLocation - body.transform.position) <= 5f) {
					// If the player made a sound withing 5 meters of enemy
					//	then make enemy alerted
					Debug.DrawLine (body.transform.position, soundLocation, Color.green, 1f);
					// Stop voice overlap
					if (!voiceSource.isPlaying)
						playVoice (voice.enemyNoiseAlert);
					EnemyPunctuation.S.displayIcon (puncType.alert);
					agn.destination = soundLocation;
					curEnemyState = enemyState.investigating;
				} else {
					Debug.DrawLine (body.transform.position, soundLocation, Color.red, 1f);
				}
			}
		}
	}

    public void SetNext(GameObject next)
    {
        nextPoint = next;
    }

	public void SetDirection (Vector3[] dir) 
	{
		pointDirection = dir;
		pointDirectionSize = dir.Length;
	}

	// KnockOut
	void OnTriggerEnter(Collider other)
	{
		// print("Triggered: " + other.gameObject.name);

		if (curEnemyState != enemyState.knockout) {

			// If player walks into enemy
			if (other.name == "Snake") {
				curEnemyState = enemyState.searching;
			}

			// If enemy is grabbed
			if (HandController.S.isGrabbing) {
				if (other.tag == "Hand") {
					curStamina -= 5;
					playVoice (voice.enemyFlip);
					curEnemyState = enemyState.knockout;
				}
			}

			// If player is trying to fight enemy
			if (HandController.S.isFighting) {

				// If enemy is punched
				if (other.tag == "Hand" || other.tag == "Leg") {
					playVoice (voice.enemyPunched);
					curEnemyState = enemyState.stun;
					timeTilNoStun = 0f;
				}
				if (other.tag == "Hand")
					curStamina -= 1;
				if (other.tag == "Leg") {
					curEnemyState = enemyState.knockout;
					playVoice (voice.enemyFlip);
					curStamina -= 2;
				}
			}
		}
	}

	void playVoice(voice whichVoice) {
		voiceSource.clip = voiceClips [(int)whichVoice];
		voiceSource.Stop ();
		voiceSource.Play ();
	}

	void activateGameOver() {
		if (!alertSoundPlayed) {
			// print ("I SEE YOU!");
			EnemyPunctuation.S.displayIcon(puncType.redAlert);
			alertSound.Play ();
			playVoice (voice.foundSnake);
			alertSoundPlayed = true;
			LevelTargetMarker.S.activateGameOver ();
		}
		agn.Stop ();
	}
}