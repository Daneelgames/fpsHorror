using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjectController : MonoBehaviour
{
    public WeaponController weapon;

    private void Start()
    {
        GameManager.instance.AddInteractableObject(this);
    }
}