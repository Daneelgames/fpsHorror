using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public float delay = 0.3f;
    float currentDelay = 0;
    public GameObject bulletPrefab;
    public Transform shotHolder;
    public LayerMask layerMask;
    public Rigidbody rb;
    public Collider col;

    GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }

    private void Update()
    {
        if (currentDelay > 0)
            currentDelay -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (currentDelay <= 0)
        {
            currentDelay = delay;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                // get direction/rotation, instantiate projectile
                Vector3 dir = hit.point - transform.position;
                Quaternion rot = Quaternion.LookRotation(dir);
                GameObject clone = Instantiate(bulletPrefab, shotHolder.position, rot) as GameObject;

                // set projectile variables
                BulletController bullet;
                bullet = clone.GetComponent<BulletController>();
                bullet.distanceToTravel = Vector3.Distance(shotHolder.position, hit.point);
                float newBulletSpeed = bullet.movespeed;
                bullet.movespeed = bullet.distanceToTravel / newBulletSpeed;
                bullet.layerMask = layerMask;
                bullet.currentTarget = hit.point;
            }
        }
    }

    public void PickUp()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        rb.useGravity = false;
        col.isTrigger = true;
    }

    public void Trow()
    {
        gameObject.layer = 14;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = true;
        col.isTrigger = false;
        rb.AddRelativeForce(Vector3.forward * 30, ForceMode.Impulse);
        rb.AddTorque(Vector3.right * 300);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.layer == 14)
        {
            gameObject.layer = 13;

            if (collision.gameObject.layer == 9)
            {
                foreach (HealthController health in gm.healthControllers)
                {
                    if (health.gameObject.name == collision.gameObject.name)
                    {
                        health.Damage(transform.position);
                        return;
                    }
                }
            }
        }
    }
}