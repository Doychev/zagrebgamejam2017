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
    private bool showRadius = false;

    private Vector3 clickPosition;

    void Start () {
        radiusVisualization.transform.localScale = new Vector3(radiusMinScale, radiusMinScale);
        currentScale = radiusMinScale;
        currentAlpha = 0.0f;
        fadeRadiusVisualization(10);
    }

    void fadeRadiusVisualization(int fadeSpeed)
    {
        Color color = radiusVisualization.GetComponent<SpriteRenderer>().color;
        currentAlpha = Mathf.SmoothStep(currentAlpha, 0, Time.deltaTime * fadeSpeed);
        color.a = currentAlpha;
        radiusVisualization.GetComponent<SpriteRenderer>().color = color;
    }

    void Update () {

        if (showRadius)
        {
            fadeRadiusVisualization(2);
            currentScale = Mathf.SmoothStep(currentScale, radiusMaxScale, Time.deltaTime * 2);
            radiusVisualization.transform.localScale = new Vector3(currentScale, currentScale);
        } else
        {
            fadeRadiusVisualization(10);
            currentScale = Mathf.SmoothStep(currentScale, radiusMinScale, Time.deltaTime * 2);
            radiusVisualization.transform.localScale = new Vector3(currentScale, currentScale);
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
        showRadius = true;
        currentAlpha = 1.0f;
    }

    void OnMouseExit()
    {
        showRadius = false;
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
