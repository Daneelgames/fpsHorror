using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float normalSpeed = 3;
    public float runSpeed = 3;

    public enum Behavior {Wait, Patrol, Wander, Chase};
    public Behavior behavior = Behavior.Wait;
    public HealthController healthController;
    public NavMeshAgent agent;

    public List<Vector3> waypoints = new List<Vector3>();
    int nextWp = 0;

    ActionAreaController actionAreaController;

    private void Start()
    {
        healthController.enemy = this;
        ChooseBehavior();
    }

    private void Update()
    {
        if (behavior == Behavior.Patrol)
        { 
            if (agent.enabled && !agent.pathPending && agent.remainingDistance < 0.5f)
                GotoNextPoint();
        }
    }

    void ChooseBehavior()
    {
        switch (behavior)
        {
            case Behavior.Wait:
                agent.speed = 0;
                break;

            case Behavior.Wander:
                agent.speed = normalSpeed;
                Invoke("Wander", 0);
                break;

            case Behavior.Patrol:
                agent.speed = normalSpeed;
                agent.autoBraking = false;
                GotoNextPoint();
                break;
        }
    }

    void GotoNextPoint()
    {
        if (waypoints.Count == 0)
            return;

        agent.destination = waypoints[nextWp];
        nextWp = (nextWp + 1) % waypoints.Count;
    }

    void Wander()
    {
        Vector2 newPos = new Vector2(Random.Range(-18f, 18), Random.Range(-9f, 9f));
        agent.SetDestination(newPos);
        Invoke("Wander", Random.Range(1, 5));
    }


    public void SetActionAreaController(ActionAreaController area)
    {
        actionAreaController = area;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        if (waypoints.Count > 0)
        {
            foreach (Vector3 t in waypoints)
            {
                if (t != null)
                    Gizmos.DrawCube(t, Vector3.one / 2);
            }
        }
    }
}