using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeColliderController : MonoBehaviour
{
    Collider col;
    GameObject owner;
    bool dangerous = false;

    List<HealthController> healthControllers = new List<HealthController>();
    List<HealthController> healthControllersDamaged = new List<HealthController>();

    private void Start()
    {
        col = GetComponent<Collider>();
        healthControllers = new List<HealthController>(GameManager.instance.healthControllers);
    }

    public void Dangerous(GameObject _owner, bool _dangerous)
    {
        owner = _owner;
        dangerous = _dangerous;
    }

    private void OnTriggerStay(Collider other)
    {
        if (dangerous)
        {
            foreach (HealthController health in healthControllers)
            {
                if (other.gameObject.name == health.gameObject.name && other.gameObject.name != owner.name)
                {
                    health.Damage(col);
                }
            }
        }
    }
}