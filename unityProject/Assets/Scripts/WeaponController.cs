﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public bool range = true;
    public int ammo = 6;
    int ammoMax = 6;
    public float delay = 0.3f;
    float currentDelay = 0;
    public float delayMelee = 0.3f;
    float currentDelayMelee = 0;
    public GameObject bulletPrefab;
    public Transform shotHolder;
    public LayerMask layerMask;
    public Rigidbody rb;
    public Collider col;
    public bool pickedUp = false;

    RaycastHit hit;
    Vector3 mPrevPos;
    Animator weaponHolderAnim;
    GameManager gm;

    PlayerController pc;
    EnemyController ec;

    private void Start()
    {
        gm = GameManager.instance;
        ammoMax = ammo;
    }

    private void Update()
    {
        if (currentDelay > 0)
            currentDelay -= Time.deltaTime;

        if (currentDelayMelee > 0)
        {
            currentDelayMelee -= Time.deltaTime;
        }

        if(gameObject.layer == 14)
        {
            if (Physics.CapsuleCast(mPrevPos, transform.position, 0.1f, transform.forward, out hit, 1, layerMask))
            {
                gameObject.layer = 13;

                if (hit.collider.gameObject.layer == 9)
                {
                    foreach (HealthController health in gm.healthControllers)
                    {
                        if (health.gameObject.name == hit.collider.gameObject.name)
                        {
                            health.Damage(col);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void Shoot(string tag)
    {
        if (currentDelay <= 0)
        {
            ammo -= 1;
            if (ammo <= 0 && ec)
            {
                currentDelay = delay * 5;
                ammo = ammoMax;
            }
            else
                currentDelay = delay;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                if (weaponHolderAnim)
                    weaponHolderAnim.SetTrigger("Shot");

                Vector3 dir = hit.point - transform.position;
                Quaternion rot = transform.rotation;
                Vector3 eu;
                if (ec)
                {
                    Vector3 random = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
                    eu= transform.rotation.eulerAngles + random;
                }
                else
                    eu = transform.rotation.eulerAngles;
                GameObject clone = Instantiate(bulletPrefab, shotHolder.position, Quaternion.Euler(eu)) as GameObject;
                clone.tag = tag;
            }
            MakeSound(15f);
        }
    }

    public void Melee()
    {
        if (currentDelayMelee <= 0)
        {
            MakeSound(5f);
            currentDelayMelee = delayMelee;
            if (weaponHolderAnim)
                weaponHolderAnim.SetTrigger("Melee");
        }
    }


    void MakeSound(float distance)
    {
        foreach (HealthController health in gm.healthControllers)
        {
            if (health.enemy && Vector3.Distance(health.transform.position, transform.position) < distance)
            {
                health.enemy.HearSound(transform.position);
            }
        }
    }

    public void PickUp(Animator anim, PlayerController _pc, EnemyController _ec)
    {
        ec = _ec;
        pc = _pc;

        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.isKinematic = true;
        rb.useGravity = false;
        col.isTrigger = true;
        pickedUp = true;
        weaponHolderAnim = anim;
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
        Invoke("ResetLayer",1);
    }

    void ResetLayer()
    {
        if (gameObject.layer == 14)
        {
            gameObject.layer = 13;
        }
    }
}