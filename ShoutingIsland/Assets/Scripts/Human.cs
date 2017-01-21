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
        this.velocity = Random.Range(1, 2);
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
            
            goalVector = this.directionEffectVector;
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

        this.preferedVelocity = Vector3.SmoothDamp(this.preferedVelocity, goalVector, ref this.interpolationVel, 2f);
    }

    public void GoInDirection(Vector2 direction)
    {
        this.isInDirectionEffect = true;
        this.directionEffectStartTime = Time.time;
        this.directionEffectVector = direction * this.velocity;
    }
}