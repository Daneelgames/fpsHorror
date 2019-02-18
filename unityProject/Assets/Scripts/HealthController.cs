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
            other.enabled = false;
            Damage(other.gameObject.transform.position);
        }
    }

    public void Damage(Vector3 explosionPosition)
    {
        if (health > 0)
        {
            health = 0;
            if (navMeshAgent)
                navMeshAgent.enabled = false;
            if (rb)
                rb.constraints = RigidbodyConstraints.None;

            if (enemy)
                enemy.CancelInvoke();
        }
        if (rb)
            rb.AddExplosionForce(500, explosionPosition, 3);
    }
}