using UnityEngine;
using RVO;
using System.Collections.Generic;

public class CrowdManager : MonoBehaviour {
    public static CrowdManager Instance
    {
        get;
        private set;
    }

    public int numWorkers;
    public float neighborDistance;
    public int maxNeighbours;
    public float timeHorizon, timeHorizonObst, radius, maxSpeed;
    public Vector2 velocity;

    public Dictionary<int, Human> humans;

    public Human humanPrefab;

    public ObstacleSetup[] obstacles;

    public bool reverseIsland;
    public ObstacleSetup islandObstacle;

    public Vector2[] waypoints;

    private float lastVelChange;

    public void Awake()
    {
        CrowdManager.Instance = this;

        Simulator.Instance.setTimeStep(Time.fixedDeltaTime);
        Simulator.Instance.SetNumWorkers(this.numWorkers);
        Simulator.Instance.setAgentDefaults(this.neighborDistance, this.maxNeighbours,
            this.timeHorizon, this.timeHorizonObst, this.radius, this.maxSpeed, this.velocity);

        this.humans = new Dictionary<int, Human>();

        if(this.reverseIsland)
        {
            Vector2[] verts = new Vector2[this.islandObstacle.vertices.Length];
            int z = 0;
            for (int i = this.islandObstacle.vertices.Length - 1; i >= 0; i--)
            {
                verts[z++] = this.islandObstacle.vertices[i];
            }
            Simulator.Instance.addObstacle(new List<Vector2>(verts));
        }
        else
        {
            Simulator.Instance.addObstacle(new List<Vector2>(this.islandObstacle.vertices));
        }

        for (int i = 0; i < this.obstacles.Length; i++)
        {
            List<Vector2> vert = new List<Vector2>(this.obstacles[i].vertices);
            Simulator.Instance.addObstacle(vert);
        }

        Simulator.Instance.processObstacles();
        
        for (int i = 0; i < 75; i++)
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
        for (int n = 0; n < this.islandObstacle.vertices.Length; n++)
        {
            if (n == this.islandObstacle.vertices.Length - 1)
            {
                Debug.DrawLine(this.islandObstacle.vertices[n], this.islandObstacle.vertices[0], Color.red);
            }
            else
            {
                Debug.DrawLine(this.islandObstacle.vertices[n], this.islandObstacle.vertices[n + 1], Color.red);
            }
        }

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

            human.currentPosition = Simulator.Instance.getAgentPosition(i);
            human.currentVelocity = Simulator.Instance.getAgentVelocity(i);

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

    /// <summary>
    /// Give all actors in radius around central position a command to move in certain direction.
    /// </summary>
    /// <param name="position">Central position of effect.</param>
    /// <param name="radius">Radius of effect.</param>
    /// <param name="direction">Direct of movement. Has to be normalized.</param>
    public void AddDirectionEffect(Vector2 position, float radius, Vector2 direction)
    {
        foreach (KeyValuePair<int, Human> kv in this.humans)
        {
            int i = kv.Key;
            Human human = kv.Value;

            if (Vector2.Distance(human.transform.position, position) <= radius)
            {
                human.GoInDirection(direction);
            }
        }
    }

    public void KillWithinRange(Vector2[] positions, float radius)
    {
        List<int> toRemoveIDs = new List<int>();

        foreach (KeyValuePair<int, Human> kv in this.humans)
        {
            int i = kv.Key;
            Human human = kv.Value;

            for (int j = 0; j < positions.Length; j ++)
            {
                if (Vector2.Distance(human.transform.position, positions[j]) <= radius)
                {
                    human.Die();
                    Simulator.Instance.setAgentPrefVelocity(i, Vector2.zero);
                    Simulator.Instance.setAgentPostion(i, new Vector2(10000, 10000));
                    toRemoveIDs.Add(i);
                }
            }
        }

        for(int i = 0; i < toRemoveIDs.Count; i++)
        {
            this.humans.Remove(toRemoveIDs[i]);
        }
    }

    public int CountLivingHumans()
    {
        return this.humans.Count;
    }

    [ContextMenu("Grab points from collider")]
    public void GrabPoints()
    {
        PolygonCollider2D coll = this.GetComponent<PolygonCollider2D>();
        if(coll == null)
        {
            return;
        }

        this.islandObstacle = new ObstacleSetup();
        this.islandObstacle.vertices = coll.GetPath(0);
        GameObject.DestroyImmediate(coll);
    }

    public void OnDrawGizmosSelected()
    {
        for(int i = 0; i < this.waypoints.Length; i++)
        {
            Gizmos.DrawSphere(this.waypoints[i], 0.2f);
        }
    }
}

[System.Serializable]
public class ObstacleSetup
{
    public Vector2[] vertices;
}