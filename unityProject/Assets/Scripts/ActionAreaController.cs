using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAreaController : MonoBehaviour
{
    [SerializeField]
    PlayerController pc;
    [SerializeField]
    EnemyController ec;

    public List<InteractableObjectController> interactableObjectControllers = new List<InteractableObjectController>();
    GameManager gm;

    float delay = 0.2f;

    private void Start()
    {
        gm = GameManager.instance;
        transform.SetParent(null);

        if (pc)
            pc.SetActionAreaController(this);
        else if (ec)
            ec.SetActionAreaController(this);
    }

    private void Update()
    {
        if (delay > 0)
            delay -= Time.deltaTime;

        if (pc)
            transform.position = pc.transform.position;
        else if (ec)
            transform.position = ec.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            foreach(InteractableObjectController ioc in gm.interactableObjectControllers)
            {
                if (ioc.gameObject.name == other.gameObject.name)
                {
                    if (ioc.weapon.pickedUp == false)
                    {
                        interactableObjectControllers.Add(ioc);
                        return;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 13)
        {
            foreach (InteractableObjectController ioc in interactableObjectControllers)
            {
                if (ioc.gameObject.name == other.gameObject.name)
                {
                    interactableObjectControllers.Remove(ioc);
                    interactableObjectControllers.Sort();
                    return;
                }
            }
        }
    }

    public void Interact(Transform arm)
    {
        if (delay <= 0)
        {
            delay = 0.2f;
            InteractableObjectController obj = interactableObjectControllers[0]; // pick closest interactable object
            float distance = 10;
            foreach (InteractableObjectController ioc in interactableObjectControllers)
            {
                var newDistance = Vector3.Distance(ioc.transform.position, transform.position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    obj = ioc;
                }
            }

            pc.PickUpWeapon(obj, arm);
            interactableObjectControllers.Remove(obj);
            if (interactableObjectControllers.Count > 0)
                interactableObjectControllers.Sort();
        }
    }
}