using UnityEngine;
using RVO;
using System.Collections.Generic;

public class CrowdManager : MonoBehaviour {
    public int numWorkers;
    public float neighborDistance;
    public int maxNeighbours;
    public float timeHorizon, timeHorizonObst, radius, maxSpeed;
    public Vector2 velocity;

    public Dictionary<int, Human> humans;

    public Human humanPrefab;

    private float lastVelChange;

    public void Awake()
    {
        Simulator.Instance.setTimeStep(Time.fixedDeltaTime);
        Simulator.Instance.SetNumWorkers(this.numWorkers);
        Simulator.Instance.setAgentDefaults(this.neighborDistance, this.maxNeighbours,
            this.timeHorizon, this.timeHorizonObst, this.radius, this.maxSpeed, this.velocity);

        this.humans = new Dictionary<int, Human>();

        for (int i = 0; i < 100; i++)
        {
            Vector2 pos = Random.insideUnitCircle * 5;
            Human h = GameObject.Instantiate<Human>(this.humanPrefab, pos, Quaternion.identity);
        }
    }

    public void FixedUpdate()
    {
        Simulator.Instance.doStep();
    }

    public void Update()
    {
        foreach (KeyValuePair<int, Human> kv in this.humans)
        {
            int i = kv.Key;
            Human human = kv.Value;

            human.transform.position = Simulator.Instance.getAgentPosition(i);

            if(((Vector2) human.transform.position - human.destination).sqrMagnitude < 1f)
            {
                human.destination = Random.insideUnitCircle * 5;
            }

            Vector3 goalVector = human.destination - (Vector2) human.transform.position;
            goalVector = goalVector.normalized * human.velocity;
            human.preferedVelocity = Vector3.SmoothDamp(human.preferedVelocity, goalVector, ref human.interpolationVel, 2f);
            Simulator.Instance.setAgentPrefVelocity(i, human.preferedVelocity);
        }
    }

    public void AddHuman(Human human)
    {
        int i = Simulator.Instance.addAgent(human.transform.position);
        Simulator.Instance.setAgentPrefVelocity(i, human.preferedVelocity);
        this.humans.Add(i, human);
        this.lastVelChange = Time.time;
    }
}
