using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour {

    public float killingRadius;
    public GameObject hitVisualizationPrefab;

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
            Vector2[] points = { coll.contacts[i].point - diff, coll.contacts[i].point, coll.contacts[i].point + diff };
            for (int j = 0; j < points.Length; j++)
            {
                GameObject hitPoint = Instantiate(hitVisualizationPrefab, points[j], Quaternion.identity);
            }
            CrowdManager.Instance.KillWithinRange(points, killingRadius);
        }

        Destroy(gameObject);
        ScoreManager.Instance.UpdateScore();
    }
}
