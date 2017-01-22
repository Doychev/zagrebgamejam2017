using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalPoleManager : MonoBehaviour {

    public GameObject radiusVisualization;
    public GameObject head;

    public SignalAnimator signalIndicator;

    public float radius;
    
    private float startScale, targetScale;
    private float timeStart;
    
    private Vector3 signalVector;

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
            this.signalIndicator.gameObject.SetActive(value);
            this._isDragging = value;
        }
        get
        {
            return this._isDragging;
        }
    }

    private AudioSource audioSource;
    private AudioClip clip;
    private bool recordingAudio;
    private int readHead;

    void Start() {
        this.radiusVisualization.transform.localScale = new Vector3(this.radius * 2, this.radius * 2);
        this.startScale = this.radius - 0.2f;
        this.targetScale = this.radius + 0.2f;
        this.showRadius = false;

        this.audioSource = this.GetComponent<AudioSource>();
    }

    void Update () {
        if(this.showRadius)
        {
            float delta = (Time.time - this.timeStart) / 2f;

            if(delta < 1)
            {
                this.radius = Mathf.SmoothStep(this.startScale, this.targetScale, delta);
                this.radiusVisualization.transform.localScale = new Vector3(this.radius * 2, this.radius * 2);
            }
            else
            {
                float b = this.targetScale;
                this.targetScale = this.startScale;
                this.startScale = b;
                this.timeStart = Time.time;
            }
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

    public void OnMouseDrag()
    {
        Vector2 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        Vector3 directionAngle = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x) - 90);

        this.head.transform.eulerAngles = directionAngle;
    }

    void OnMouseUp()
    {
        this.isDragging = false;

        this.signalVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;

        StartCoroutine(SignalGenerator());
    }

    IEnumerator SignalGenerator()
    {
        ScoreManager.Instance.startGame();

        //this.head.GetComponent<SpriteRenderer>().color = Color.green;
        float signalTimerStart = Time.time;

        while (Time.time - signalTimerStart < 8f)
        {
            this.SendSignal();
            yield return new WaitForSeconds(2.5f);
        }
        //this.head.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void SendSignal()
    {
        if(this.audioSource.clip != null)
        {
            this.audioSource.Play();
        }

        // TODO: start signalanimator animation

        CrowdManager.Instance.AddDirectionEffect(transform.position, this.radius, signalVector);
    }
}
