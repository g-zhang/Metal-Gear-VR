using UnityEngine;
using System.Collections;

public class WalkingPoint : MonoBehaviour {
    public GameObject nextPoint;
	// What direction the enemy will face at that point
	public Vector3 direction; 

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll) 
    {
        if(coll.gameObject.tag == "Enemy")
		{
			coll.gameObject.GetComponent<EnemyBehavior> ().SetNext (nextPoint);
			coll.gameObject.GetComponent<EnemyBehavior> ().SetDirection (direction);
        }
        
    }
}
