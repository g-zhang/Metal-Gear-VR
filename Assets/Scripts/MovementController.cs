using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {
    static MovementController Snake;
    public bool bigBossMode = false; //enemies cannot see you when true 

	public float speed = 3.8f;
	public float slowSpeed = 1f;
    public float rotationSpeed = 0.4f;

    public float FPVRotationSpeedDeg = 2.5f;
    public float FPVRotationDecel = .6f;

    public float crouchHeight = .75f;
    public float sneakCrouchWallBuffer = 0.35f; //amount of distance to move the player away from the wall when crawling from a sneaked crouch position

    BoxCollider boxcollider;

    Rigidbody body;
	public enum movementState { run = 0, crawl, sneak };
	public movementState currState = movementState.run;
    public bool inCrouchMode = false; //snake in be in crouch in either sneak or crawl mode
    public static MovementController player;

	public float collided = .1f;
    public float collided2 = .2f;
	public float collideToStick = .1f;
	public enum movementLock { LR = 0, FB }
	public movementLock moveLock;
    public float raycastOffsetTop = .5f;
    public float raycastOffsetBottom = .3f;

    public bool FPVModeCrawlControl = false;

    public float inFPVModeCrawlTransition = 0f;
    public float ToFPVModeTransitionTime = .5f;
    public float FromFPVModeTransitionTime = 1.0f;

    Vector3 lastVel;
    Vector3 lastCrawlTurnVector;
    Vector3 lastForwardCrawlVector = Vector3.zero;
    float timeTillTurnUpdate = 1f / 60f; //turn controls in FPV crawl mode updates in 60 hz
    Vector3 defaultPlayerSize;
    Vector3 defaultBoxColliderSize;

    int wallLayerMask = 1 << 10; //mask for wall raycasting
    int floorLayerMask = 1 << 12;

    //performs a double raycast above and below the start point by offset amount
    //returns true if either raycast returns true
    public bool doubleRaycast(Vector3 start, Vector3 end, float distance)
    {
        Vector3 start1 = start - new Vector3(0, raycastOffsetBottom, 0);
        //Debug.DrawRay(start1, end);

        Vector3 start2 = start + new Vector3(0, raycastOffsetTop, 0);
        //Debug.DrawRay(start2, end);

        return Physics.Raycast(start1, end, distance, wallLayerMask) || Physics.Raycast(start2, end, distance, wallLayerMask);
    }

    public Vector3 findForwardCrawlVector()
    {
        Vector3 crawlForwardVector = Vector3.zero;
        body.transform.Rotate(new Vector3(-90f, 0f, 0f));
        crawlForwardVector = body.transform.forward;
        body.transform.Rotate(new Vector3(90f, 0f, 0f));
        return crawlForwardVector;
    }

    void Awake()
    {
        Snake = this;
    }

    // Use this for initialization
    void Start () {
		body = gameObject.GetComponent<Rigidbody>();
        boxcollider = gameObject.GetComponent<BoxCollider>();
        defaultBoxColliderSize = boxcollider.size;
        defaultPlayerSize = body.transform.localScale;
		player = this;
	}

	// Update is called once per frame
	void Update () {
        //grab the forwards vector for when the body is facedown (crawl state)
        Vector3 crawlForwardVector = findForwardCrawlVector();
        //Debug.DrawRay(body.transform.position, crawlForwardVector * 5f, Color.green);

        if (Input.GetKeyDown(KeyCode.I))
        {
            if(!bigBossMode)
            {
                bigBossMode = true;
                print("Invincibility Mode ACTIVATED!");
            } else
            {
                bigBossMode = false;
                print("Invincibility Mode DE-ACTIVATED!");
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            print("Circle Button: Punch!");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Square Button: Grab!");
        }
        if (Input.GetKey(KeyCode.W))
        {
            print("Triangle Button: FPV Cam");
        }

        Vector3 vel = Vector3.zero;
		if (Input.GetKey (KeyCode.UpArrow)) {
			vel.z += 1;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			vel.z -= 1;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			vel.x -= 1;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			vel.x += 1;
		}

		//perform wall stick sneak movements
		if (currState == movementState.sneak) {
			if (Vector3.Dot (vel, gameObject.transform.forward) > 0 && Vector3.Angle (vel, gameObject.transform.forward) < 90) {
				if (inCrouchMode) {
					currState = movementState.crawl;
					gameObject.transform.position += vel.normalized * sneakCrouchWallBuffer;
				} else {
					currState = movementState.run;
				}
				collided = collideToStick;
                collided2 = collideToStick * 2;
            } else if (!doubleRaycast (gameObject.transform.position, -body.transform.forward, (gameObject.transform.lossyScale.z / 2) + .2f)) {
				if (inCrouchMode) {
					currState = movementState.crawl;
					gameObject.transform.position += vel.normalized * sneakCrouchWallBuffer;
				} else {
					currState = movementState.run;
				}
				collided = collideToStick;
                collided2 = collideToStick * 2;
            } else if (inCrouchMode) {
				vel = Vector3.zero;
			} else if (moveLock == movementLock.LR) {
				vel.x = 0;
			} else if (moveLock == movementLock.FB) {
				vel.z = 0;
			} 

			// SNEAK CAM CODE START

			// If sneaking, then check to see if you're at an edge
			Vector3 leftPosition, rightPosition;
			bool leftEdge, rightEdge;
			RaycastHit leftHit, rightHit;
			rightPosition = body.transform.position + body.transform.right + (body.transform.forward * -1);
			leftPosition = body.transform.position + (-1 * body.transform.right) + (body.transform.forward * -1);
			rightPosition.y += 3f;
			leftPosition.y += 3f;

			// Debug rays
			Debug.DrawRay (rightPosition, new Vector3(0f,-4f,0f));
			Debug.DrawRay (leftPosition, new Vector3(0f,-4f, 0f));

			// 	change camera position to sneak cam position
			// If either of the raycasts don't hit anything, then player is at an edge
			leftEdge = Physics.Raycast (leftPosition, new Vector3(0f,-1f,0f), out leftHit, 4f);
			rightEdge = Physics.Raycast (rightPosition, new Vector3(0f,-1f,0f), out rightHit, 4f);

			if (leftEdge) {
				if (leftHit.collider.tag != "Floor")
					leftEdge = false;
			}
			if (rightEdge) {
				if (rightHit.collider.tag != "Floor")
					rightEdge = false;
			}

			// For cam direction purposes, move point forward
			leftPosition += body.transform.forward;
			rightPosition += body.transform.forward;

			if (leftEdge && rightEdge) {
				CameraController.S.GetComponent<CameraFollow> ().activateSneakCam (3, leftPosition+rightPosition);
			} else if (leftEdge) {
				CameraController.S.GetComponent<CameraFollow> ().activateSneakCam (0, leftPosition);
			} else if (rightEdge) {
				CameraController.S.GetComponent<CameraFollow> ().activateSneakCam (1, rightPosition);
			}
			// Otherwise, you're not at an edge
			else {
				// Change camera position back to normal
				CameraController.S.GetComponent<CameraFollow> ().deactivateSneakCam ();
			}

			// SNEAK CAM CODE END
		} 
		// Also make sure that cam is deactivated to default once you leave sneak
		else {
			CameraController.S.GetComponent<CameraFollow> ().deactivateSneakCam ();
		}

		// Save the last known button press direction
		if (vel != Vector3.zero)
			lastVel = vel;

        //CROUCH logic: transform snake back to normal if he moves
        if(inCrouchMode && vel != Vector3.zero)
        {
            body.transform.localScale = defaultPlayerSize;
            //check for nearby walls so we don't clip inside them
            Ray frontray = new Ray(body.transform.position, body.transform.forward);
            Ray buttray = new Ray(body.transform.position, -body.transform.forward);
            if (Physics.Raycast(frontray, body.transform.lossyScale.y / 2f, wallLayerMask))
            {
                Vector3 safePos = gameObject.transform.position;
                safePos += -body.transform.forward.normalized * ((body.transform.lossyScale.y / 2f) + .1f);
                safePos.y = .2f;
                body.transform.position = safePos;
            }
            else
            {
                body.transform.position = new Vector3(body.transform.position.x, 0.2f, body.transform.position.z);
            }

            body.transform.rotation = Quaternion.LookRotation(vel.normalized);
            body.transform.Rotate(new Vector3(90f, 0f, 0f));

            inCrouchMode = false;
        }

        //CRAWL box collider, shrink down the box collider when in crawl mode
        if(currState == movementState.crawl && !inCrouchMode)
        {
            boxcollider.size = new Vector3(boxcollider.size.x, 1f, boxcollider.size.z);
            boxcollider.center = new Vector3(boxcollider.center.x, .3f, boxcollider.center.z);
        } else
        {
            boxcollider.size = defaultBoxColliderSize;
            boxcollider.center = new Vector3(boxcollider.center.x, .15f, boxcollider.center.z);
        }

        //MOVEMENT of the Character
		// Speed and direction of movement depends on movementState
        if(inFPVModeCrawlTransition > 0 && currState == movementState.crawl)
        {
            inFPVModeCrawlTransition -= Time.deltaTime;
            body.velocity = lastForwardCrawlVector;
        } else if (currState == movementState.run) {
			body.velocity = vel.normalized * speed;
		} else if(!FPVModeCrawlControl) {
			body.velocity = vel.normalized * slowSpeed;
		} else 
        //first person crawl mode uses a different set of controls
        if(FPVModeCrawlControl)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                body.velocity = crawlForwardVector * slowSpeed;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                body.velocity = -crawlForwardVector * slowSpeed;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                lastCrawlTurnVector = (new Vector3(0f, 0f, FPVRotationSpeedDeg));
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                lastCrawlTurnVector = (new Vector3(0f, 0f, -FPVRotationSpeedDeg));
            }
            else
            {
                if(timeTillTurnUpdate <= 0)
                {
                    lastCrawlTurnVector *= FPVRotationDecel;
                }
            }

            if (timeTillTurnUpdate > 0)
            {
                timeTillTurnUpdate -= Time.deltaTime;
            }
            else
            {
                timeTillTurnUpdate = 1f / 60f;
                body.transform.Rotate(lastCrawlTurnVector);
            }
                

        }

        //ROTATIONS, face character towards the vector of movement:
        //set our foward direction if in run state (run rotation)
        if (currState == movementState.run)
        {
            if (Vector3.Angle(body.transform.forward, lastVel.normalized) > 45f)
            {
                body.velocity = Vector3.zero;
            }

            // Lerp for smooth rotation
            body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, lastVel.normalized, rotationSpeed));
            //body.transform.rotation = Quaternion.LookRotation(lastVel.normalized);	
        }
        else //set look direction in crawl mode, unless snake is backing up, or in FPV Crawl mode
        if (currState == movementState.crawl && !FPVModeCrawlControl && inFPVModeCrawlTransition <= 0)
        {
            if (vel != Vector3.zero)
            {
                body.transform.Rotate(new Vector3(90f, 0f, 0f));

                float velAngleFromCurrent = Vector3.Angle(body.transform.forward, vel);

                //dont turn snake around if its within 45 degrees
                if (Mathf.Abs(velAngleFromCurrent - 0f) <= 50f)
                {
                    //body.transform.Rotate(new Vector3(-90f, 0f, 0f));
                    body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                    body.transform.Rotate(new Vector3(90f, 180f, 0f));
                }
                else
                {
                    body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                    body.transform.Rotate(new Vector3(90f, 0f, 0f));
                }
            }
        }

        //Crouch toggle behavior
		if(Input.GetKeyDown(KeyCode.A))
		{
            if(inCrouchMode)
            {
                inCrouchMode = false;
                body.transform.localScale = defaultPlayerSize;
                currState = movementState.run;
            }
			else if (currState == movementState.run || currState == movementState.sneak)
			{
				//body.transform.Rotate(new Vector3(90f, 0f, 0f));

				//				body.transform.Rotate( Vector3.Lerp (Vector3.zero, new Vector3 (90f, 0f, 0f), 0.5f));
				//
				body.transform.position = new Vector3(body.transform.position.x, .25f, body.transform.position.z);

                Vector3 crouchSize = body.transform.localScale;
                crouchSize.y = crouchHeight;
                body.transform.localScale = crouchSize;
                inCrouchMode = true;

                //special case when sneaking against wall
                if(currState == movementState.sneak)
                {
                    currState = movementState.sneak;
                } else
                {
                    currState = movementState.crawl;
                }

			} else if(currState == movementState.crawl && !FPVModeCrawlControl && inFPVModeCrawlTransition <= 0)
			{
				body.transform.Rotate(new Vector3(-90f, 0f, 0f));
				body.transform.position = new Vector3(body.transform.position.x, .5f, body.transform.position.z);
				currState = movementState.run;
			}
		}

        //RAYCAST behaviors:
        //SNEAK Wall Detection
        if (vel != Vector3.zero)
        {
            //check for wall collision to stick to
            if (doubleRaycast(gameObject.transform.position, body.transform.forward, (gameObject.transform.lossyScale.z / 2) + 0.2f) && collided2 > 0)
            {
                collided2 -= Time.deltaTime;
            }
            else if (collided2 < 0 && currState == movementState.run)
            {
                // Flips character
                body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                gameObject.transform.forward *= -1;
                currState = movementState.sneak;

                if (gameObject.transform.forward.x != 0)
                {
                    moveLock = movementLock.LR;
                }
                else if (gameObject.transform.forward.z != 0)
                {
                    moveLock = movementLock.FB;
                }
            }
        }

        //CRAWL modes DETECTION
        if (currState == movementState.crawl && !inCrouchMode)
        {
            //Check FPVModeCrawl State, aka if snake crawls under enclosed space, switch mode and camera
            if (Physics.Raycast(gameObject.transform.position, -body.transform.forward, (gameObject.transform.lossyScale.z / 2) + 1.0f, wallLayerMask))
            {
                if (!FPVModeCrawlControl)
                {
                    // Should only be FPV-Crawl when you cannot stand up aka crawling in a 1 meter tall tunnel
                    CameraController.S.SwitchCameraTo(CameraType.crawl);
                    FPVModeCrawlControl = true;
                    inFPVModeCrawlTransition = ToFPVModeTransitionTime;
                    lastForwardCrawlVector = body.velocity;
                }
            } else {
                if(FPVModeCrawlControl)
                {
                    CameraController.S.SwitchCameraTo(CameraType.overhead);
                    FPVModeCrawlControl = false;
                    inFPVModeCrawlTransition = FromFPVModeTransitionTime;
                    lastForwardCrawlVector = body.velocity;
                }
            }

            if(!FPVModeCrawlControl && !inCrouchMode)
            {
                Ray headray = new Ray(gameObject.transform.position, crawlForwardVector);
                RaycastHit hit;
                //Debug.DrawRay(gameObject.transform.position, crawlForwardVector * ((gameObject.transform.lossyScale.y / 2) + 0.4f), Color.green);
                if (Physics.Raycast(headray, out hit, ((gameObject.transform.lossyScale.y / 2) + 0.4f), wallLayerMask) && collided > 0)
                {
                    collided -= Time.deltaTime;
                }
                else if (collided < 0)
                {
                    // Flips character
                    body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                    gameObject.transform.forward = -crawlForwardVector;
                    currState = movementState.sneak;

                    Vector3 newPos = hit.point;
                    if(hit.point != Vector3.zero)
                    {
                        newPos += -crawlForwardVector.normalized * ((gameObject.transform.lossyScale.y / 4));
                    } else
                    {
                        newPos = gameObject.transform.position;
                    }
                    print(hit.point);
                    
                    gameObject.transform.position = newPos;

                    Vector3 crouchSize = body.transform.localScale;
                    crouchSize.y = crouchHeight;
                    body.transform.localScale = crouchSize;
                    inCrouchMode = true;

                    if (gameObject.transform.forward.x != 0)
                    {
                        moveLock = movementLock.LR;
                    }
                    else if (gameObject.transform.forward.z != 0)
                    {
                        moveLock = movementLock.FB;
                    }

                    body.velocity = Vector3.zero;
                }

                Ray buttray = new Ray(gameObject.transform.position, -crawlForwardVector);
                RaycastHit hit2;
                Debug.DrawRay(gameObject.transform.position, -crawlForwardVector * (gameObject.transform.lossyScale.y / 4), Color.green);
                if (Physics.Raycast(buttray, out hit2, (gameObject.transform.lossyScale.y / 4), wallLayerMask) && collided2 > 0)
                {
                    collided2 -= Time.deltaTime;
                }
                else if (collided2 < 0)
                {
                    // Flips character
                    body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                    gameObject.transform.forward = crawlForwardVector;
                    currState = movementState.sneak;

                    Vector3 newPos = hit2.point;
                    if(newPos != Vector3.zero)
                    {
                        newPos += crawlForwardVector.normalized * ((gameObject.transform.lossyScale.y / 4));
                    } else
                    {
                        newPos = gameObject.transform.position;
                    }
                    gameObject.transform.position = newPos;

                    Vector3 crouchSize = body.transform.localScale;
                    crouchSize.y = crouchHeight;
                    body.transform.localScale = crouchSize;
                    inCrouchMode = true;

                    if (gameObject.transform.forward.x != 0)
                    {
                        moveLock = movementLock.LR;
                    }
                    else if (gameObject.transform.forward.z != 0)
                    {
                        moveLock = movementLock.FB;
                    }
                }
            }
        }
        //CRAWL modes DETECTION END
    }
}