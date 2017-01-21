using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalPoleManager : MonoBehaviour {

    public GameObject radiusVisualization;
    public GameObject arrowVisualisation;

    public float radius;
    
    private float startScale, targetScale;
    private float timeStart;

    private bool towerActive = false;

    private bool _showRadius;
    private bool showRadius {
        set
        {
            this.radiusVisualization.SetActive(value);
            this._showRadius = value;
        }
        get
        {
            return this._showRadius;
        }
    }

    private bool _isDragging;
    private bool isDragging
    {
        set
        {
            this.arrowVisualisation.SetActive(value);
            this._isDragging = value;
        }
        get
        {
            return this._isDragging;
        }
    }

    void Start() {
        this.radiusVisualization.transform.localScale = new Vector3(this.radius * 10, this.radius * 10);
        this.startScale = this.radius - 0.2f;
        this.targetScale = this.radius + 0.2f;
        this.showRadius = false;
    }

    void Update () {
        if(this.showRadius)
        {
            float delta = (Time.time - this.timeStart) / 2f;

            if(delta < 1)
            {
                this.radius = Mathf.SmoothStep(this.startScale, this.targetScale, delta);
                this.radiusVisualization.transform.localScale = new Vector3(this.radius * 10, this.radius * 10);
            }
            else
            {
                float b = this.targetScale;
                this.targetScale = this.startScale;
                this.startScale = b;
                this.timeStart = Time.time;
            }
        }

        if(this.isDragging)
        {
            this.arrowVisualisation.transform.position = this.transform.position;
            Vector3 diff = this.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
            this.arrowVisualisation.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        }
    }

    void OnMouseEnter()
    {
        this.showRadius = true;
    }

    void OnMouseExit()
    {
        this.showRadius = false;
    }

    void OnMouseDown()
    {
        this.isDragging = true;
    }

    void OnMouseUp()
    {
        this.isDragging = false;

        Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        
        towerActive = true;
        GetComponent<SpriteRenderer>().color = Color.green;
        CrowdManager.Instance.AddDirectionEffect(transform.position, this.radius, distance.normalized);
    }
}
