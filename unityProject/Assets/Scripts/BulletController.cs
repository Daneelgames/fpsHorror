using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject explosion;
    public float movespeed = 5;
    public float distanceToTravel;

    public ParticleSystem particles;
    ParticleSystem.EmissionModule particleEmission;

    [HideInInspector]
    public Vector3 currentTarget;
    [HideInInspector]
    public LayerMask layerMask;

    float originalSpeed = 5;
    Ray ray;
    RaycastHit hit;
    bool dead = false;

    private void Awake()
    {
        particleEmission = particles.emission;
        originalSpeed = movespeed;
    }

    /*
    void UpdateDistance()
    {
        ray = new Ray(transform.position, transform.TransformVector(Vector3.forward));
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            if (Vector3.Distance(currentTarget,hit.point) > 0.5f)
            {
                distanceToTravel = Vector3.Distance(transform.position, hit.point);
                float newBulletSpeed = originalSpeed;
                movespeed = distanceToTravel / newBulletSpeed;
                currentTarget = hit.point;
            }
        }
    }
    */

    void FixedUpdate()
    {
        if (distanceToTravel <= 0.01f) // have we arrived our target?
        {
            if (!dead)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(gameObject,1);
                dead = true;
                particleEmission.enabled = false;
            }
        }
        else
        {
            transform.Translate(Vector3.forward * movespeed);
            distanceToTravel -= movespeed;
        }
    }
}