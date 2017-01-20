using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalPoleManager : MonoBehaviour {

    public GameObject radiusVisualization;

    public float radiusMinScale, radiusMaxScale;
    public float timerMax;
    
    private float currentScale, currentAlpha;
    private float timer;

    private bool towerActive = false;

    private Vector3 clickPosition;

    void Start () {
        radiusMinScale = radiusVisualization.transform.localScale.x;
	}
	
	void Update () {
        if (radiusVisualization.activeInHierarchy)
        {
            currentScale = Mathf.SmoothStep(currentScale, radiusMaxScale, Time.deltaTime * 2);
            radiusVisualization.transform.localScale = new Vector3(currentScale, currentScale);

            Color color = radiusVisualization.GetComponent<SpriteRenderer>().color;
            currentAlpha = Mathf.SmoothStep(currentAlpha, 0, Time.deltaTime * 5);
            color.a = currentAlpha;
            radiusVisualization.GetComponent<SpriteRenderer>().color = color;
        }

        if (towerActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                towerActive = false;
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    void OnMouseEnter()
    {
        radiusVisualization.SetActive(true);
        currentAlpha = 1.0f;
    }

    void OnMouseExit()
    {
        radiusVisualization.SetActive(false);
        currentScale = radiusMinScale;
    }

    void OnMouseDown()
    {
        clickPosition = Input.mousePosition;
        //Debug.Log("start: " + lastMousePosition);
    }

    //void OnMouseDrag()
    //{
    //    Debug.Log("drag: " + Input.mousePosition);
    //    Vector3 distance = Input.mousePosition - lastMousePosition;
    //    Debug.Log("distance: " + distance);
    //}

    void OnMouseUp()
    {
        //Debug.Log("end: " + Input.mousePosition);
        Vector3 distance = Input.mousePosition - clickPosition;
        Debug.Log("distance: " + distance);

        timer = timerMax;
        towerActive = true;
        GetComponent<SpriteRenderer>().color = Color.green;
    }
}
