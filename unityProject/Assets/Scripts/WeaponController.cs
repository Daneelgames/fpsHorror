using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public bool range = true;

    public float delay = 0.3f;
    float currentDelay = 0;
    public GameObject bulletPrefab;
    public Transform shotHolder;
    public LayerMask layerMask;
    public Rigidbody rb;
    public Collider col;

    public bool pickedUp = false;

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
                Vector3 dir = hit.point - transform.position;
                //Quaternion rot = Quaternion.LookRotation(dir);
                Quaternion rot = transform.rotation;
                GameObject clone = Instantiate(bulletPrefab, shotHolder.position, rot) as GameObject;
            }
        }
    }

    public void PickUp()
    {
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        rb.useGravity = false;
        col.isTrigger = true;
        pickedUp = true;
    }

    public void Throw(float power)
    {
        gameObject.layer = 14;
        transform.SetParent(null);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.useGravity = true;
        col.isTrigger = false;
        rb.AddRelativeForce(Vector3.forward * power, ForceMode.Impulse);
        rb.AddTorque(Vector3.right * 300);
        pickedUp = false;
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
                        health.Damage(collision.collider);
                        return;
                    }
                }
            }
        }
    }
}