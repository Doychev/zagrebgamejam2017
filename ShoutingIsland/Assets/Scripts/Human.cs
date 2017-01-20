using UnityEngine;

public class Human : MonoBehaviour
{
    public float velocity;
    public Vector2 preferedVelocity;

    public Vector2 destination;
    
    public Vector3 interpolationVel;

    public void Start()
    {
        this.velocity = Random.Range(1, 2);
        this.preferedVelocity = Random.insideUnitCircle * 2;
        this.destination = Random.insideUnitCircle * 5;
        GameObject.FindObjectOfType<CrowdManager>().AddHuman(this);
    }

    public void DoYourShit()
    {
        if (((Vector2)this.transform.position - this.destination).sqrMagnitude < 1f)
        {
            this.destination = Random.insideUnitCircle * 5;
        }

        Vector3 goalVector = this.destination - (Vector2)this.transform.position;
        goalVector = goalVector.normalized * this.velocity;
        this.preferedVelocity = Vector3.SmoothDamp(this.preferedVelocity, goalVector, ref this.interpolationVel, 2f);
    }
}