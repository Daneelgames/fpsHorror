using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthController : MonoBehaviour
{
    public int health = 1;
    public Rigidbody rb;
    public NavMeshAgent navMeshAgent;

    [HideInInspector]
    public EnemyController enemy;

    private void Start()
    {
        GameManager.instance.AddHealthController(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 11)
        {
            if ((enemy && other.gameObject.tag == "PlayerProjectile") || (!enemy && other.gameObject.tag == "EnemyProjectile"))
                Damage(other);
        }
    }

    public void Damage(Collider other)
    {
        if (health > 0)
        {
            other.enabled = false;
            health = 0;
            if (navMeshAgent)
                navMeshAgent.enabled = false;
            if (rb)
                rb.constraints = RigidbodyConstraints.None;

            if (enemy)
            {
                enemy.Dead();
            }
        }
        if (rb)
            rb.AddExplosionForce(500, other.gameObject.transform.position, 3);
    }
}