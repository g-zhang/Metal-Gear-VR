using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour {
    public float speed = 10f;
    Rigidbody body;
    public enum movementState { run = 0, crawl, sneak };
    public movementState currState = movementState.run;
    public static MovementController player;

    public float collided = .1f;
    public float collideToStick = .1f;
    public enum movementLock { LR = 0, FB }
    public movementLock moveLock;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody>();
        player = this;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 vel = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            vel.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vel.z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vel.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vel.x += 1;
        }

        //perform wall stick sneak movements
        if(currState == movementState.sneak)
        {
            if(Vector3.Dot(vel, gameObject.transform.forward) > 0 && Vector3.Angle(vel, gameObject.transform.forward) < 90) {
                currState = movementState.run;
                collided = collideToStick;
            }
            else if(!Physics.Raycast(gameObject.transform.position, -body.transform.forward, (gameObject.transform.lossyScale.z / 2) + .2f))
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

        body.velocity = vel.normalized * speed;
        if(vel != Vector3.zero)
        {
            //gameObject.transform.forward = vel.normalized;

            //set our foward direction if in run state
            if (currState != movementState.sneak && currState != movementState.crawl)
            {
                body.transform.rotation = Quaternion.LookRotation(vel.normalized);
            }

            //set look direction in crawl mode, unless snake is backing up
            if(currState == movementState.crawl)
            {
                body.transform.Rotate(new Vector3(90f, 0f, 0f));
                float velAngleFromCurrent = Vector3.Angle(body.transform.forward, vel);
                if (Mathf.Approximately(velAngleFromCurrent, 180f) || Mathf.Approximately(velAngleFromCurrent, 0f))
                {
                    body.transform.Rotate(new Vector3(-90f, 0f, 0f));
                } else
                {
                    body.transform.rotation = Quaternion.LookRotation(vel.normalized);
                    body.transform.Rotate(new Vector3(90f, 0f, 0f));
                }
            }

            //check for wall collision to stick to
            if(Physics.Raycast(gameObject.transform.position, body.transform.forward, (gameObject.transform.lossyScale.z / 2) + .2f) && collided > 0)
            {
                collided -= Time.deltaTime;
            }
            else if(collided < 0 && currState == movementState.run)
            {
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
                body.transform.position = new Vector3(body.transform.position.x, 0.25f, body.transform.position.z);
                currState = movementState.crawl;
            } else if(currState == movementState.crawl)
            {
                body.transform.Rotate(new Vector3(-90f, 0f, 0f));
                body.transform.position = new Vector3(body.transform.position.x, 1f, body.transform.position.z);
                currState = movementState.run;
            }
        }
    }
}
