using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public HealthController healthController;
    public NavMeshAgent agent;

    public List<Transform> wps = new List<Transform>();

    private void Start()
    {
        healthController.enemy = this;
        Invoke("Move1", 0);
    }

    void Move1()
    {
        agent.SetDestination(wps[0].position);
        Invoke("Move2", 0.5f);
    }

    void Move2()
    {
        agent.SetDestination(wps[1].position);
        Invoke("Move1", 0.5f);
    }
}