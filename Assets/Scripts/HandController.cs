using UnityEngine;
using System.Collections;

public class HandController : MonoBehaviour {

    HandController S;
    public GameObject RightHand;

    public float handMoveDistance = .3f;
    public float knockSpeed;
    float initHandPos;

    bool isKnocking = false;
    bool isKnockingAnimation = false;
    Vector3 knockLocation = Vector3.zero;
    public int knockFrameCount = 2;
    int curKnockFrameCount = 2;



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
        initHandPos = RightHand.transform.localPosition.x;
	}


    void playKnockAnimation()
    {
        Rigidbody body = RightHand.GetComponent<Rigidbody>();
        Vector3 tempPos = body.transform.localPosition;
        tempPos.x = initHandPos + handMoveDistance * Mathf.Sin(knockSpeed * Time.time);
        body.transform.localPosition = tempPos;
    }
	
	// Update is called once per frame
	void Update () {
        if(isKnocking)
        {
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

        playKnockAnimation();


        //input
        if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.sneak)
        {
            print("Circle Button: Knock!");
            isKnocking = true;
            isKnockingAnimation = true;
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
