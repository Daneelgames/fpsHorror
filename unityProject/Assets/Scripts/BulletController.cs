using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float mSpeed = 10.0f;

    public GameObject explosion;
    public ParticleSystem particles;
    public LayerMask layerMask;
    ParticleSystem.EmissionModule particleEmission;
    bool dead = false;
    Vector3 mPrevPos;

    private void Awake()
    {
        particleEmission = particles.emission;
    }

    private void Start()
    {
        mPrevPos = transform.position;
    }

    private void Update()
    {
        mPrevPos = transform.position;
        transform.Translate(0.0f, 0.0f, mSpeed * Time.deltaTime);

        RaycastHit hit;

        if (Physics.Raycast(mPrevPos, (transform.position - mPrevPos).normalized,  out hit, (transform.position - mPrevPos).magnitude, layerMask))
        {
            Dead(hit.point);
        }
    }

    void Dead(Vector3 pos)
    {
        if (!dead)
        {
            Instantiate(explosion, pos, Quaternion.identity);
            Destroy(gameObject, 1);
            dead = true;
            particleEmission.enabled = false;
        }
    }
}