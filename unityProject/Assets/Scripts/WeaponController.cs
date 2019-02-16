using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject bullet;
    public Transform shotHolder;

    public void Shoot()
    {
        GameObject newBullet = Instantiate(bullet, shotHolder.position, Quaternion.identity);
        newBullet.transform.eulerAngles = transform.eulerAngles;
    }
}