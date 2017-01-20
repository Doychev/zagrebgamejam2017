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
}