using UnityEngine;
using System.Collections;

public class WalkingPoint : MonoBehaviour {
    public GameObject nextPoint;
	public float pauseTime = 3f;

	float timeTilMove;

	// Use this for initialization
	void Start () {
		timeTilMove = pauseTime;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll) 
    {
        if(coll.gameObject.tag == "Enemy")
		{
			coll.gameObject.GetComponent<EnemyBehavior> ().SetNext (nextPoint);
        }
        
    }
}
