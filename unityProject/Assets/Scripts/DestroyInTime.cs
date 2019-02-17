using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    public float time = 1;

    private void Start()
    {
        Destroy(gameObject, time);
    }
}