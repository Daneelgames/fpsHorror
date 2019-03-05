using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float normalSpeed = 3;
    public float runSpeed = 3;

    public enum Behavior {Wait, Patrol, Wander, Chase, Inspect, Dead};
    public Behavior behavior = Behavior.Wait;
    public NavMeshAgent agent;

    public WeaponController weapon;
    public List<HealthController> healthControllers = new List<HealthController>();

    public List<Vector3> waypoints = new List<Vector3>();
    int nextWp = 0;

    public bool alert = false;
    public Animator anim;

    GameManager gm;
    PlayerController pc;
    ActionAreaController actionAreaController;

    [SerializeField]
    ParticleSystem particlesSeeThroughWalls;
    ParticleSystem.EmissionModule particlesEmit;
    RaycastHit hit;

    [SerializeField]
    LayerMask layerMask;

    EnemySightController enemySight;
    float explosionDelay = 0;

    public float rbMass = 0.5f;

    private void Start()
    {
        enemySight = GetComponent<EnemySightController>();
        weapon.PickUp(null, null, this);
        weapon.name += gameObject.name;
        gm = GameManager.instance;
        pc = gm.pc;
        ChooseBehavior();

        particlesEmit = particlesSeeThroughWalls.emission;
        InvokeRepeating("CheckIfMeshIsVisible", 0.1f, 0.1f);
    }

    private void Update()
    {
        if (behavior != Behavior.Dead)
        {
            if (behavior == Behavior.Patrol)
            {
                if (agent.enabled && !agent.pathPending && agent.remainingDistance < 0.5f)
                    GotoNextPoint();
            }
            else if (behavior == Behavior.Chase)
            {
                ChasePlayer();
            }
            else if (behavior == Behavior.Inspect)
            {
                if (agent.enabled && !agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    behavior = Behavior.Wait;
                    Invoke("Wander", Random.Range(1,5));
                }
            }

            if (agent.velocity.magnitude > 0)
                anim.SetBool("Run", true);
            else
                anim.SetBool("Run", false);
        }

        if (explosionDelay > 0)
            explosionDelay -= Time.deltaTime;
    }

    void CheckIfMeshIsVisible()
    {
        if (Physics.Raycast(transform.position + Vector3.up, (pc.transform.position - transform.position).normalized, out hit, Vector3.Distance(transform.position, pc.transform.position), layerMask))
        {
            if (hit.collider.gameObject == pc.gameObject) // if player see enemy
            {
                if (particlesEmit.enabled == true)
                    particlesEmit.enabled = false;
            }
            else // if not
            {
                if (behavior != Behavior.Dead && particlesEmit.enabled == false)
                    particlesEmit.enabled = true;
                else
                    particlesEmit.enabled = false;
            }
            if (behavior == Behavior.Dead && particlesEmit.enabled == true)
                particlesEmit.enabled = false;
        }
        else
        {
            particlesEmit.enabled = false;
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
                agent.autoBraking = true;
                Wander();
                break;

            case Behavior.Patrol:
                agent.speed = normalSpeed;
                agent.autoBraking = false;
                //GotoNextPoint();
                break;

            case Behavior.Chase:
                agent.speed = runSpeed;
                agent.autoBraking = false;
                break;

            case Behavior.Inspect:
                agent.speed = runSpeed;
                agent.autoBraking = false;
                break;
        }
    }

    void ChasePlayer()
    {
        if (!weapon.range)
            agent.SetDestination(pc.transform.position);
        else
        {
            if (enemySight.playerInSight)
            {
                agent.isStopped = true;

                Vector3 lTargetDir = pc.transform.position - transform.position;
                lTargetDir.y = 0.0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 1f);

                //transform.LookAt(pc.transform.position);
                if (pc.healthController.health > 0 && weapon)
                    weapon.Shoot("EnemyProjectile");
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(pc.transform.position);
            }
        }
    }

    void InspectSound(Vector3 soundPosition)
    {
        agent.destination = soundPosition;
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
            CancelInvoke("Wander");
            behavior = Behavior.Chase;
            ChooseBehavior();
            alert = true;
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

    public void HearSound(Vector3 soundPosition)
    {
        if (behavior != Behavior.Chase && agent.enabled)
        {
            CancelInvoke("Wander");
            behavior = Behavior.Inspect;
            ChooseBehavior();
            InspectSound(soundPosition);
        }
    }

    public void AddHealthController(HealthController hc)
    {
        healthControllers.Add(hc);
    }

    public void Explosion(Vector3 explosionOrigin)
    {
        if (explosionDelay <= 0)
        {
            explosionDelay = 0.5f;
            foreach (HealthController hc in healthControllers)
            {
                hc.Explosion(explosionOrigin);
            }
        }
    }

    public void Dead()
    {
        CancelInvoke();
        enemySight.CancelInvoke();
        weapon.Throw(10f);
        weapon = null;
        if (particlesEmit.enabled == true)
            particlesEmit.enabled = false;

        behavior = Behavior.Dead;
    }
}