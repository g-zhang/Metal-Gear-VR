using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {
	public float speed = 3.8f;
	public float slowSpeed = 1f;
    public float rotationSpeed = 0.4f;

    Rigidbody body;
	public enum movementState { run = 0, crawl, sneak };
	public movementState currState = movementState.run;
	public static MovementController player;

	public float collided = .1f;
	public float collideToStick = .1f;
	public enum movementLock { LR = 0, FB }
	public movementLock moveLock;
    public float raycastOffsetTop = .5f;
    public float raycastOffsetBottom = .3f;

    Vector3 lastVel;

    //performs a double raycast above and below the start point by offset amount
    //returns true if either raycast returns true
    public bool doubleRaycast(Vector3 start, Vector3 end, float distance)
    {
        Vector3 start1 = start - new Vector3(0, raycastOffsetBottom, 0);
        //Debug.DrawRay(start1, end);

        Vector3 start2 = start + new Vector3(0, raycastOffsetTop, 0);
        //Debug.DrawRay(start2, end);

        return Physics.Raycast(start1, end, distance) || Physics.Raycast(start2, end, distance);
    }

	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody>();
		player = this;
	}

	// Update is called once per frame
	void Update () {
		Vector3 vel = Vector3.zero;
		if (Input.GetKey (KeyCode.W)) {
			vel.z += 1;
		}
		if (Input.GetKey (KeyCode.S)) {
			vel.z -= 1;
		}
		if (Input.GetKey (KeyCode.A)) {
			vel.x -= 1;
		}
		if (Input.GetKey (KeyCode.D)) {
			vel.x += 1;
		}

		//perform wall stick sneak movements
		if(currState == movementState.sneak)
		{
			if(Vector3.Dot(vel, gameObject.transform.forward) > 0 && Vector3.Angle(vel, gameObject.transform.forward) < 90) {
				currState = movementState.run;
				collided = collideToStick;
			}
			else if(!doubleRaycast(gameObject.transform.position, -body.transform.forward, (gameObject.transform.lossyScale.z / 2) + .2f))
			{
				currState = movementState.run;
				collided = collideToStick;
			}
			else if (moveLock == movementLock.LR)
			{
				vel.x = 0;
			}
			else if(moveLock == movementLock.FB)
			{
				vel.z = 0;
			}
		}

		// Save the last known button press direction
		if (vel != Vector3.zero) 
			lastVel = vel;

        //MOVEMENT of the Character
		// Speed of movement depends on movementState
		if (currState == movementState.run) {
			body.velocity = vel.normalized * speed;
		} else {
			body.velocity = vel.normalized * slowSpeed;
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
        else //set look direction in crawl mode, unless snake is backing up
        if (currState == movementState.crawl)
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

		if(vel != Vector3.zero)
		{
			//check for wall collision to stick to
			if(doubleRaycast(gameObject.transform.position, body.transform.forward, (gameObject.transform.lossyScale.z / 2) + 0.2f) && collided > 0)
			{
				collided -= Time.deltaTime;
			}
			else if(collided < 0 && currState == movementState.run)
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

		if(Input.GetKeyDown(KeyCode.Q))
		{
			if (currState == movementState.run)
			{
				body.transform.Rotate(new Vector3(90f, 0f, 0f));

				//				body.transform.Rotate( Vector3.Lerp (Vector3.zero, new Vector3 (90f, 0f, 0f), 0.5f));
				//
				body.transform.position = new Vector3(body.transform.position.x, 0.25f, body.transform.position.z);
				currState = movementState.crawl;
			} else if(currState == movementState.crawl)
			{
				body.transform.Rotate(new Vector3(-90f, 0f, 0f));
				body.transform.position = new Vector3(body.transform.position.x, .75f, body.transform.position.z);
				currState = movementState.run;
				CameraController.S.SwitchCameraTo(CameraType.overhead);
			}
		}

        //CAMERA movement logic

        //check if snake crawls under obstacles
        if(currState == movementState.crawl)
        {
            if (Physics.Raycast(gameObject.transform.position, -body.transform.forward, (gameObject.transform.lossyScale.z / 2) + 1.0f))
            {
                // Should only be FPV-Crawl when you cannot stand up aka crawling in a 1 meter tall tunnel
                CameraController.S.SwitchCameraTo(CameraType.crawl);
            } else
            {
                CameraController.S.SwitchCameraTo(CameraType.overhead);
            }
        }
	}
}