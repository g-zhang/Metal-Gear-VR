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

	void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.tag == "Enemy")
        {
			if (timeTilMove > 0) {
				timeTilMove -= Time.deltaTime;
			} else {
				timeTilMove = pauseTime;
				other.gameObject.GetComponent<EnemyBehavior> ().SetNext (nextPoint);
			}
        }
        
    }
}
