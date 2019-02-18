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

    public WeaponController weapon;

    public List<Vector3> waypoints = new List<Vector3>();
    int nextWp = 0;

    public bool alert = false;

    GameManager gm;
    PlayerController pc;
    ActionAreaController actionAreaController;

    private void Start()
    {
        weapon.PickUp();
        weapon.name += gameObject.name;
        gm = GameManager.instance;
        pc = gm.pc;
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
                Wander();
                break;

            case Behavior.Patrol:
                agent.speed = normalSpeed;
                agent.autoBraking = false;
                GotoNextPoint();
                break;

            case Behavior.Chase:
                agent.speed = runSpeed;
                agent.autoBraking = false;
                ChasePlayer();
                break;
        }
    }

    void ChasePlayer()
    {
        if (!weapon.range)
            agent.SetDestination(pc.transform.position);
    }

    void GotoNextPoint()
    {
        if (waypoints.Count == 0)
            return;

        if (agent.enabled)
            agent.destination = waypoints[nextWp];
        nextWp = (nextWp + 1) % waypoints.Count;
    }

    void Wander()
    {
        Vector2 newPos = new Vector2(Random.Range(-18f, 18), Random.Range(-9f, 9f));
        if (agent.enabled)
            agent.SetDestination(newPos);
        Invoke("Wander", Random.Range(1, 5));
    }

    public void PlayerFound(bool found)
    {
        if (found)
        {
            behavior = Behavior.Chase;

            CancelInvoke();
            ChooseBehavior();
            alert = true;
        }
        else if (!found && behavior == Behavior.Chase)
        {
            behavior = Behavior.Wander;
            Invoke("ChooseBehavior", 1);
        }
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

    public void Dead()
    {
        CancelInvoke();
        weapon.Throw(10f);
    }
}