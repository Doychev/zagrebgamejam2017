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

    [HideInInspector]
    public bool isDead;

    public void Start()
    {
        this.directionEffectTimeout += Random.Range(-1f, 2f);
        this.velocity = Random.Range(1f, 2f);
        this.preferedVelocity = Random.insideUnitCircle * 2;
        this.destination = Random.insideUnitCircle * 5;
        this.isInDirectionEffect = false;
        this.isDead = false;
        CrowdManager.Instance.AddHuman(this);
    }

    public void DoYourShit()
    {
        Vector3 goalVector;

        if (isDead)
        {
            this.velocity = 0f;
            this.preferedVelocity = Vector2.zero;
            this.destination = this.transform.position;
            return;
        }

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
                this.destination = this.generateDestination();
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

    public void Die()
    {
        this.isDead = true;
    }

    private Vector2 generateDestination()
    {
        Vector2 dest;
        do
        {
            dest = Random.insideUnitCircle * 5;
        } while (!this.isDestinationAllowed(dest));

        return dest;
    }

    private bool isDestinationAllowed(Vector2 dest)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(dest, 0.2f, LayerMask.GetMask("Island", "Obstacle"));

        bool allowed = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                return false;
            }
            else if(colliders[i].gameObject.layer == LayerMask.NameToLayer("Island"))
            {
                allowed = true;
            }
        }

        return allowed;
    }
}