using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthController : MonoBehaviour
{
    public int health = 1;
    public Rigidbody rb;
    public NavMeshAgent navMeshAgent;
    public EnemyController enemy;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (enemy)
        {
            enemy.AddHealthController(this);
            gameObject.name = gameObject.name + enemy.gameObject.name;
            rb.mass = enemy.rbMass;
        }
            
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
        if (health > 0 && enemy && enemy.behavior != EnemyController.Behavior.Dead)
        {
            health = 0;
            enemy.anim.enabled = false;

            if (other.gameObject.layer == 11)
                other.enabled = false;

            if (navMeshAgent)
                navMeshAgent.enabled = false;
            if (rb)
                rb.constraints = RigidbodyConstraints.None;

            if (enemy)
            {
                enemy.Dead();
            }
        }
        
        if (enemy)
            enemy.Explosion(other.gameObject.transform.position);
    }

    public void Explosion(Vector3 explosionOrigin)
    {
        if (rb)
            rb.AddExplosionForce(1000, explosionOrigin - Vector3.up, 10);
    }
}