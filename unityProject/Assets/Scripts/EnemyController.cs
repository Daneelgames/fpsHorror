using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum Behavior {Wait, Patrol, Wander, Chase};
    public Behavior behavior = Behavior.Wait;
    public HealthController healthController;
    public NavMeshAgent agent;

    public List<Transform> wps = new List<Transform>();

    ActionAreaController actionAreaController;

    private void Start()
    {
        healthController.enemy = this;
        Invoke("Move1", 0);
    }

    void Move1()
    {
        Vector2 newPos = new Vector2(Random.Range(-18f, 18), Random.Range(-9f, 9f));
        agent.SetDestination(newPos);
        Invoke("Move2", 0.5f);
    }

    void Move2()
    {
        Vector2 newPos = new Vector2(Random.Range(-18f, 18), Random.Range(-9f, 9f));
        agent.SetDestination(newPos);
        Invoke("Move1", 0.5f);
    }

    public void SetActionAreaController(ActionAreaController area)
    {
        actionAreaController = area;
    }
}