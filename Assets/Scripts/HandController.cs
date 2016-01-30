using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {

    public static HandController S;
    public GameObject LeftHand;
    public GameObject RightHand;
    public float punchDistance = 1f;
    public float punchSwingOffset = .25f; //distance snakes punches towards center
    public float punchSpeed = 10f;
    float punchSideSpeed; 
    float punchAnimationTime;
    float currLeftAnimTime;
    float currRightAnimTime;

    public AudioSource knockSound;
    public float handMoveDistance = .3f;
    public float knockSpeed = 5f;
    float knockAnimationTime;
    float currAnimationTime = 0f;

    Vector3 initLeftHandPos;
    Vector3 initRightHandPos;

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
        initLeftHandPos = LeftHand.transform.localPosition;
        initRightHandPos = RightHand.transform.localPosition;
        knockAnimationTime = (handMoveDistance / knockSpeed) * 2f;
        punchAnimationTime = (punchDistance / punchSpeed) * 2f;
        punchSideSpeed = (punchSwingOffset) / punchAnimationTime * 2f;
        currLeftAnimTime = punchAnimationTime;
        currRightAnimTime = punchAnimationTime;
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
                                                                    initRightHandPos.z - Mathf.PingPong(currAnimationTime * knockSpeed, handMoveDistance));

            //Debug.DrawRay(RightHand.transform.position, -transform.forward.normalized * .25f, Color.green);
            if(currAnimationTime <= knockAnimationTime / 2f
                && !isKnocking
                && Physics.Raycast(RightHand.transform.position, -transform.forward, .25f))
            {
                //set actuial knock state when the hand hits the wall
                isKnocking = true;
                knockLocation = RightHand.transform.TransformPoint(new Vector3(RightHand.transform.localPosition.x,
                                                                    RightHand.transform.localPosition.y,
                                                                    initRightHandPos.z));
                knockSound.Stop();
                knockSound.Play();
            }
            currAnimationTime -= Time.deltaTime;
        }
        else if(MovementController.player.currState == MovementController.movementState.sneak)
        {
            RightHand.transform.localPosition = new Vector3(RightHand.transform.localPosition.x,
                                                                    RightHand.transform.localPosition.y,
                                                                    initRightHandPos.z);
        }
    }

    void LeftHandPunch()
    {
        if(currLeftAnimTime < punchAnimationTime)
        {
            LeftHand.transform.localPosition = new Vector3(initLeftHandPos.x + Mathf.PingPong(currLeftAnimTime * punchSideSpeed, punchSwingOffset),
                                                            LeftHand.transform.localPosition.y,
                                                            initLeftHandPos.z + Mathf.PingPong(currLeftAnimTime * punchSpeed, punchDistance));

            currLeftAnimTime += Time.deltaTime;
        }
        else
        {
            LeftHand.transform.localPosition = initLeftHandPos;
        }
    }

    void RightHandPunch()
    {
        if (currRightAnimTime < punchAnimationTime)
        {
            RightHand.transform.localPosition = new Vector3(initRightHandPos.x - Mathf.PingPong(currRightAnimTime * punchSideSpeed, punchSwingOffset),
                                                            RightHand.transform.localPosition.y,
                                                            initRightHandPos.z + Mathf.PingPong(currRightAnimTime * punchSpeed, punchDistance));

            currRightAnimTime += Time.deltaTime;
        }
        else if(MovementController.player.currState != MovementController.movementState.sneak)
        {
            RightHand.transform.localPosition = initRightHandPos;
        }
    }

    // Update is called once per frame
    void Update () {
        performKnocks();
        LeftHandPunch();
        RightHandPunch();

        //input
        if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.sneak)
        {
            print("Circle Button: Knock!");
            currAnimationTime = knockAnimationTime;
        }
        if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.run)
        {
            print("Circle Button: Punch!");
            currLeftAnimTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print("Square Button: Grab!");
            currRightAnimTime = 0;
        }

    }
}
