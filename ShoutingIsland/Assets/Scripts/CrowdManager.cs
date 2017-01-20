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

    public ObstacleSetup[] obstacles;

    private float lastVelChange;

    public void Awake()
    {
        Simulator.Instance.setTimeStep(Time.fixedDeltaTime);
        Simulator.Instance.SetNumWorkers(this.numWorkers);
        Simulator.Instance.setAgentDefaults(this.neighborDistance, this.maxNeighbours,
            this.timeHorizon, this.timeHorizonObst, this.radius, this.maxSpeed, this.velocity);

        this.humans = new Dictionary<int, Human>();

        for(int i = 0; i < this.obstacles.Length; i++)
        {
            List<Vector2> vert = new List<Vector2>(this.obstacles[i].vertices);
            Simulator.Instance.addObstacle(vert);
        }

        Simulator.Instance.processObstacles();

        for (int i = 0; i < 200; i++)
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

        for (int i = 0; i < this.obstacles.Length; i++)
        {
            for(int n = 0; n < this.obstacles[i].vertices.Length; n++)
            {
                if(n == this.obstacles[i].vertices.Length - 1)
                {
                    Debug.DrawLine(this.obstacles[i].vertices[n], this.obstacles[i].vertices[0]);
                }
                else
                {
                    Debug.DrawLine(this.obstacles[i].vertices[n], this.obstacles[i].vertices[n + 1]);
                }
            }
        }

        foreach (KeyValuePair<int, Human> kv in this.humans)
        {
            int i = kv.Key;
            Human human = kv.Value;

            human.transform.position = Simulator.Instance.getAgentPosition(i);

            human.DoYourShit();
            
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

[System.Serializable]
public class ObstacleSetup
{
    public Vector2[] vertices;
}