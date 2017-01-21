using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningSpriteHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a -= 0.015f;
        GetComponent<SpriteRenderer>().color = color;

        if (color.a < 0.01f)
        {
            Destroy(gameObject);
        }
	}
}
