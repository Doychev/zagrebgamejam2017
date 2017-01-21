using UnityEngine;

public class Human : MonoBehaviour
{
    public float directionEffectTimeout;

    [HideInInspector]
    public float velocity;
    [HideInInspector]
    public Vector2 preferedVelocity;
    
    private Vector2 destination;
    private Vector3 interpolationVel;

    private bool isInDirectionEffect;
    private float directionEffectStartTime;
    private Vector2 directionEffectVector;

    public void Start()
    {
        this.directionEffectTimeout += Random.Range(-1f, 2f);
        this.velocity = Random.Range(1f, 2f);
        this.preferedVelocity = Random.insideUnitCircle * 2;
        this.destination = Random.insideUnitCircle * 5;
        this.isInDirectionEffect = false;
        CrowdManager.Instance.AddHuman(this);
    }

    public void DoYourShit()
    {
        Vector3 goalVector;

        if (this.isInDirectionEffect)
        {
            if(Time.time - this.directionEffectStartTime > this.directionEffectTimeout)
            {
                this.isInDirectionEffect = false;
            }
            
            goalVector = this.directionEffectVector * this.velocity;
        }
        else
        {
            if (((Vector2)this.transform.position - this.destination).sqrMagnitude < 1f)
            {
                this.destination = Random.insideUnitCircle * 5;
            }

            goalVector = this.destination - (Vector2)this.transform.position;
            goalVector = goalVector.normalized * this.velocity;
        }

        this.preferedVelocity = Vector3.SmoothDamp(this.preferedVelocity, goalVector, ref this.interpolationVel, 0.3f);
    }

    public void GoInDirection(Vector2 direction)
    {
        this.isInDirectionEffect = true;
        this.directionEffectStartTime = Time.time;
        this.directionEffectVector = Quaternion.Euler(0, Random.Range(-20f, 20f), 0) * direction * this.velocity;
    }
}