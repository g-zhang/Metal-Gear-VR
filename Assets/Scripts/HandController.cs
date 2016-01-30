using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {

    HandController S;
    public GameObject RightHand;

    public AudioSource knockSound;
    public float handMoveDistance = .3f;
    public float knockSpeed = 5f;
    float knockAnimationTime;
    float currAnimationTime = 0f;
    float initHandPos;

    Vector3 knockLocation = Vector3.zero;
    public int knockFrameCount = 2;
    int curKnockFrameCount = 2;

    public bool _______________________________;
    public bool isKnocking = false;



    //GETTERS for Knocking status
    public bool isCurrentlyKnocking(out Vector3 _knockLocation)
    {
        _knockLocation = knockLocation;
        return isKnocking;
    }

    public bool isCurrentlyKnocking()
    {
        return isKnocking;
    }
    public Vector3 getKnockLocation()
    {
        return knockLocation;
    }
    //END GETTERS

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        initHandPos = RightHand.transform.localPosition.z;
        knockAnimationTime = (handMoveDistance / knockSpeed) * 2f;
	}


    void performKnocks()
    {
        if (isKnocking)
        {
            //Debug.DrawRay(knockLocation, Vector3.up * 5f, Color.red);
            if (curKnockFrameCount > 0)
            {
                curKnockFrameCount--;
            }
            else
            {
                isKnocking = false;
                curKnockFrameCount = knockFrameCount;
                knockLocation = Vector3.zero;
            }
        }

        if (currAnimationTime > 0)
        {
            RightHand.transform.localPosition = new Vector3(RightHand.transform.localPosition.x,
                                                                    RightHand.transform.localPosition.y,
                                                                    initHandPos - Mathf.PingPong(currAnimationTime * knockSpeed, handMoveDistance));

            //Debug.DrawRay(RightHand.transform.position, -transform.forward.normalized * .25f, Color.green);
            if(currAnimationTime <= knockAnimationTime / 2f
                && !isKnocking
                && Physics.Raycast(RightHand.transform.position, -transform.forward, .25f))
            {
                //set actuial knock state when the hand hits the wall
                isKnocking = true;
                knockLocation = RightHand.transform.TransformPoint(new Vector3(RightHand.transform.localPosition.x,
                                                                    RightHand.transform.localPosition.y,
                                                                    initHandPos));
                knockSound.Play();
            }
            currAnimationTime -= Time.deltaTime;
        }
        else
        {
            RightHand.transform.localPosition = new Vector3(RightHand.transform.localPosition.x,
                                                                    RightHand.transform.localPosition.y,
                                                                    initHandPos);
        }
    }
	
	// Update is called once per frame
	void Update () {
        performKnocks();

        //input
        if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.sneak)
        {
            print("Circle Button: Knock!");
            currAnimationTime = knockAnimationTime;
        }
        if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.run)
        {
            print("Circle Button: Punch!");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Square Button: Grab!");
        }

    }
}
