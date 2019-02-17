using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform shotHolder;

    public LayerMask layerMask;

    public void Shoot()
    {
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