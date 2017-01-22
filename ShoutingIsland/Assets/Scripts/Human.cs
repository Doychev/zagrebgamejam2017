using UnityEngine;

public class Human : MonoBehaviour
{
    public float directionEffectTimeout;

    [HideInInspector]
    public Vector2 preferedVelocity;
    [HideInInspector]
    public bool isDead;
    [HideInInspector]
    public Vector2 currentVelocity, currentPosition;

    private float speed, effectSpeed;
    
    private Vector2 destination;
    private Vector3 interpolationVel;

    private bool isInDirectionEffect;
    private float directionEffectStartTime;
    private Vector2 directionEffectVector;


    private int currentWaypoint;
    private WalkingType walkingType;

    public void Start()
    {
        this.currentWaypoint = (int)Random.Range(0, CrowdManager.Instance.waypoints.Length - 0.001f);
        this.walkingType = this.generateRandomType();

        this.directionEffectTimeout += Random.Range(-1f, 2f);
        this.preferedVelocity = Random.insideUnitCircle * 2;
        this.transform.position = this.generateDestination();
        this.destination = this.generateDestination();
        this.isInDirectionEffect = false;
        this.isDead = false;

        this.speed = Random.Range(0.4f, 0.7f);
        this.effectSpeed = Random.Range(0.8f, 1f);

        CrowdManager.Instance.AddHuman(this);
    }

    public void DoYourShit()
    {
        Vector3 goalVector;

        if (this.isInDirectionEffect)
        {
            if (Time.time - this.directionEffectStartTime > this.directionEffectTimeout)
            {
                this.isInDirectionEffect = false;
                this.destination = this.generateDestination();
            }

            goalVector = this.directionEffectVector;
        }
        else
        {
            if (((Vector2)this.transform.position - this.destination).sqrMagnitude < 1f)
            {
                this.destination = this.generateDestination();
            }

            goalVector = this.destination - (Vector2)this.transform.position;
            goalVector = goalVector.normalized * this.speed;
        }

        this.preferedVelocity = Vector3.SmoothDamp(this.preferedVelocity, goalVector, ref this.interpolationVel, 0.3f);

        this.transform.position = this.currentPosition;
        this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(this.currentVelocity.y, this.currentVelocity.x));
    }

    public void GoInDirection(Vector2 direction)
    {
        this.isInDirectionEffect = true;
        this.directionEffectStartTime = Time.time;
        this.directionEffectVector = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * direction.normalized * this.effectSpeed;
    }

    public void Die()
    {
        this.isDead = true;
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    private Vector2 generateDestination()
    {
        if(Random.value > 0.85f)
        {
            this.walkingType = this.generateRandomType();
        }

        Vector2 dest;
        do
        {
            dest = this.calculateNextDesitnation();
        } while (!this.isDestinationAllowed(dest));

        return dest;
    }

    private Vector2 calculateNextDesitnation()
    {
        Vector2 rand = Random.onUnitSphere;

        if (this.walkingType == WalkingType.random)
        {
            return rand * 4;
        }
        else if(this.walkingType == WalkingType.waypointRandom)
        {
            Vector2 waypoint = CrowdManager.Instance.waypoints[(int)Random.Range(0, CrowdManager.Instance.waypoints.Length - 0.001f)];
            return waypoint + rand * 0.75f;
        }
        else if(this.walkingType == WalkingType.waypoint)
        {
            this.currentWaypoint++;
            if(this.currentWaypoint >= CrowdManager.Instance.waypoints.Length)
            {
                this.currentWaypoint = 0;
            }

            return CrowdManager.Instance.waypoints[this.currentWaypoint] + rand * 0.75f;
        }
        else if(this.walkingType == WalkingType.waypointOpposite)
        {
            this.currentWaypoint--;
            if(this.currentWaypoint < 0)
            {
                this.currentWaypoint = CrowdManager.Instance.waypoints.Length - 1;
            }

            return CrowdManager.Instance.waypoints[this.currentWaypoint] + rand * 0.75f;
        }
        else
        {
            return rand;
        }
    }

    private WalkingType generateRandomType()
    {
        float chance = Random.value;

        if(chance < 0.07f)
        {
            return WalkingType.random;
        }
        else if(chance < 0.2f)
        {
            return WalkingType.waypointRandom;
        }
        else if(chance < 0.6f)
        {
            return WalkingType.waypoint;
        }
        else
        {
            return WalkingType.waypointOpposite;
        }
    }

    private bool isDestinationAllowed(Vector2 dest)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(dest, 0.3f, LayerMask.GetMask("Island", "Obstacle"));

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

public enum WalkingType
{
    waypoint, waypointOpposite, waypointRandom, random
}