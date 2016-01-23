using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour {

    public GameObject nextPoint;
    Rigidbody body;
    public float speed = 10f;
    NavMeshAgent agn;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody>();
        agn = gameObject.GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 distance = nextPoint.transform.position - gameObject.transform.position;
        //body.velocity = distance.normalized * speed;
        //body.transform.forward = distance.normalized;

//		Vector3 targetDir = nextPoint.transform.position - transform.position;
//		if (Vector3.Angle (body.transform.forward, targetDir) > 45f) {
//			body.transform.rotation = Quaternion.LookRotation(Vector3.Slerp(body.transform.forward, targetDir, 0.4f));
//		} else {
			agn.destination = nextPoint.transform.position;
//		}

        //vision
        Vector3 toPlayer = MovementController.player.transform.position - gameObject.transform.position;
        if(Vector3.Angle(toPlayer, gameObject.transform.forward) < 45 && toPlayer.magnitude < 4)
        {
            Application.LoadLevel("Main");
        }
	}

    public void SetNext(GameObject next)
    {
        nextPoint = next;
    }
}
