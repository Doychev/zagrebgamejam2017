using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        for (int i = 0; i < coll.contacts.Length; i++)
        {
            Vector2 diff = Quaternion.Euler(0, 0, 90) * coll.contacts[i].normal;
            //coll.contacts[i].point + coll.contacts[i]
            Debug.Log(coll.contacts[i].normal);
            Debug.Log(coll.contacts[i].point - diff);
            Debug.Log(coll.contacts[i].point);
            Debug.Log(coll.contacts[i].point + diff);

            Debug.DrawLine(coll.contacts[i].point - diff, coll.contacts[i].point + diff, Color.green);
        }
        Destroy(gameObject);
    }
}
