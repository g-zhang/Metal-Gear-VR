using UnityEngine;
using System.Collections;

public enum PlayerCombatState { punch1 = 0, punch2, kick, recovery };

public class HandController : MonoBehaviour {

    Rigidbody body;
    public static HandController S;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject RightLeg;

    public float punchCooldown = .05f;
    public float comboWindowTime = .2f; //window where another button press will continue the combo
    public float comboCooldown = .5f; //seconds of cooldown after executing full combo
    public float kickWindupTime = .05f; // seconds it takes for a kick to prepare
    public float grabCooldown = .1f;

    public float punchDistance = 1f;
    public float punchSwingOffset = .25f; //distance snakes punches towards center
    public float punchSpeed = 10f;
    float punchSideSpeed; //calculated in Start()
    float punchAnimationTime; //calculated in Start()
    float currLeftAnimTime;
    float currRightAnimTime;

    public float kickDistance = 2f;
    public float kickHeight = 1f;
    public float kickSideDistance = .25f;
    public float kickSpeed = 15f;
    float kickUpwardSpeed; //calculated in Start()
    float kickSideSpeed; //calculated in Start()
    float kickAnimationTime; //calculated in Start()
    float currKickAnimTime;

    public float punch1CharVelocity = .6f;
    public float punch2CharVelocity = -.3f;
    public float kickCharVelocity = -.4f;

    public float handMoveDistance = .3f;
    public float knockSpeed = 5f;
    float knockAnimationTime;
    float currAnimationTime = 0f;

    AudioSource playerAudio;
    public AudioClip knockSound;
    public AudioClip punchSound;
    public AudioClip kickSound;
    bool leftPunchSoundPlayed = false;
    bool rightPunchSoundPlayed = false;
    bool kickSoundPlayed = false;

    Vector3 initLeftHandPos;
    Vector3 initRightHandPos;
    Vector3 initRightLegPos;

    Vector3 knockLocation = Vector3.zero;
    public int knockFrameCount = 2;
    int curKnockFrameCount = 2;

    public bool _______________________________;
    public bool isKnocking = false;
    public bool isFighting = false;
    public bool isGrabbing = false;
    public PlayerCombatState currCombatState = PlayerCombatState.punch1;
    public float curCooldown = 0f;
    float curComboWindow = 0f;
    public int queuedHits = 0;



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
        body = gameObject.GetComponent<Rigidbody>();
        playerAudio = gameObject.GetComponent<AudioSource>();

        initLeftHandPos = LeftHand.transform.localPosition;
        initRightHandPos = RightHand.transform.localPosition;
        initRightLegPos = RightLeg.transform.localPosition;

        knockAnimationTime = (handMoveDistance / knockSpeed) * 2f;
        punchAnimationTime = (punchDistance / punchSpeed) * 2f;
        punchSideSpeed = (punchSwingOffset) / punchAnimationTime * 2f;

        kickAnimationTime = (kickDistance / kickSpeed) * 2f;
        kickUpwardSpeed = (kickHeight) / kickAnimationTime * 2f;
        kickSideSpeed = kickSideDistance / kickAnimationTime * 2f;
        
        currLeftAnimTime = punchAnimationTime;
        currRightAnimTime = punchAnimationTime;
        currKickAnimTime = kickAnimationTime;
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
                playerAudio.PlayOneShot(knockSound, 1f);
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
        if (currLeftAnimTime < punchAnimationTime)
        {
            LeftHand.transform.localPosition = new Vector3(initLeftHandPos.x + Mathf.PingPong(currLeftAnimTime * punchSideSpeed, punchSwingOffset),
                                                            LeftHand.transform.localPosition.y,
                                                            initLeftHandPos.z + Mathf.PingPong(currLeftAnimTime * punchSpeed, punchDistance));

            if(!leftPunchSoundPlayed && !isGrabbing)
            {
                playerAudio.PlayOneShot(punchSound, 1f);
                leftPunchSoundPlayed = true;
            }
            currLeftAnimTime += Time.deltaTime;
        }
        else
        {
            leftPunchSoundPlayed = false;
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
            if (!rightPunchSoundPlayed && !isGrabbing)
            {
                playerAudio.PlayOneShot(punchSound, 1f);
                rightPunchSoundPlayed = true;
            }
            currRightAnimTime += Time.deltaTime;
        }
        else if(MovementController.player.currState != MovementController.movementState.sneak)
        {
            RightHand.transform.localPosition = initRightHandPos;
            rightPunchSoundPlayed = false;
        }
    }

    void RightKick()
    {
        if (currKickAnimTime < kickAnimationTime)
        {
            RightLeg.transform.localPosition = new Vector3(initRightLegPos.x - Mathf.PingPong(currKickAnimTime * kickSideSpeed, kickSideDistance),
                                                            initRightLegPos.y + Mathf.PingPong(currKickAnimTime * kickUpwardSpeed, kickHeight),
                                                            initRightLegPos.z + Mathf.PingPong(currKickAnimTime * kickSpeed, kickDistance));
            if (!kickSoundPlayed)
            {
                playerAudio.PlayOneShot(kickSound, 1f);
                kickSoundPlayed = true;
            }
            currKickAnimTime += Time.deltaTime;
        }
        else
        {
            RightLeg.transform.localPosition = initRightLegPos;
            kickSoundPlayed = false;
        }
    }

    void CombatStateUpdate()
    {
        if(curComboWindow > 0f)
        {
            curComboWindow -= Time.deltaTime;
        } else
        {
            currCombatState = PlayerCombatState.punch1;
            isFighting = false;
        }

        if (curCooldown > 0f)
        {
            curCooldown -= Time.deltaTime;
        } else if (isGrabbing)
        {
            isGrabbing = false;
        }

        if(isGrabbing)
        {
            body.velocity = Vector3.zero;
        }

        if(isFighting)
        {
            if(isGrabbing)
            {
                isGrabbing = false;
            }

            //velocity states
            if(currCombatState == PlayerCombatState.punch2)
            {
                body.velocity = body.transform.forward.normalized * punch1CharVelocity;
            } else if(currCombatState == PlayerCombatState.kick)
            {
                body.velocity = body.transform.forward.normalized * punch2CharVelocity;
            } else if (currCombatState == PlayerCombatState.recovery)
            {
                body.velocity = body.transform.forward.normalized * kickCharVelocity;
            }
        }
    }

    void CombatButtonPress()
    {
        curComboWindow = comboWindowTime;
        if (curCooldown <= 0)
        {          
            if (currCombatState == PlayerCombatState.punch1)
            {
                isFighting = true;
                currLeftAnimTime = 0;
                currCombatState++;
                curCooldown = punchAnimationTime + punchCooldown;
            }
            else if (currCombatState == PlayerCombatState.punch2)
            {
                currRightAnimTime = 0;
                currCombatState++;
                curCooldown = punchAnimationTime + kickWindupTime;
            }
            else if (currCombatState == PlayerCombatState.kick)
            {
                currKickAnimTime = 0;
                currCombatState++;
                curCooldown = kickAnimationTime + comboCooldown;
            } else if(currCombatState == PlayerCombatState.recovery)
            {
                currCombatState = PlayerCombatState.punch1;
                isFighting = false;
                curCooldown = 0;
                queuedHits = 0;
            }
        } else if (currCombatState != PlayerCombatState.recovery)
        {
            queuedHits++;
        }
    }

    void GrabButtonPress()
    {
        if(curCooldown <= 0)
        {
            currLeftAnimTime = 0;
            currRightAnimTime = 0;
            curCooldown = punchAnimationTime + grabCooldown;
            isGrabbing = true;
        }
    }

    // Update is called once per frame
    void Update () {
        CombatStateUpdate();
        performKnocks();
        LeftHandPunch();
        RightHandPunch();
        RightKick();

        //input
        if (!MovementController.player.FPVModeControl)
        {
            //knocking
            if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.sneak)
            {
                currAnimationTime = knockAnimationTime;
            }
            //combat
            if (Input.GetKeyDown(KeyCode.S) && MovementController.player.currState == MovementController.movementState.run)
            {
                CombatButtonPress();
            }
            else if (queuedHits > 0)
            {
                queuedHits--;
                CombatButtonPress();
            }
            //grabbing
            if (Input.GetKeyDown(KeyCode.Q) && MovementController.player.currState == MovementController.movementState.run)
            {
                GrabButtonPress();
            }
        }
    }
}
