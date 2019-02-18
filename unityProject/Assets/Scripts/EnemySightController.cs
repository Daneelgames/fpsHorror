using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySightController : MonoBehaviour
{
    public float fovAngle = 100;
    public bool playerInSight = false;

    EnemyController ec;
    NavMeshAgent nav;
    Animator anim;
    PlayerController pc;
    GameManager gm;

    private void Start()
    {
        ec = GetComponent<EnemyController>();
        gm = GameManager.instance;
        pc = gm.pc;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        InvokeRepeating("CheckPlayer", 0.1f, 0.1f);
    }

    void CheckPlayer()
    {
        var direction = pc.transform.position - transform.position;
        var angle = Vector3.Angle(direction, transform.forward);

        if (angle < fovAngle * 0.5f)
        {
            RaycastHit hit;
            var distance = Vector3.Distance(transform.position + transform.up, pc.transform.position + transform.up);

            if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, distance))
            {
                if (hit.collider.gameObject == pc.gameObject)
                {
                    playerInSight = true;
                }
                else
                {
                    playerInSight = false;
                }
                ec.PlayerFound(playerInSight);
            }
        }
    }
}
