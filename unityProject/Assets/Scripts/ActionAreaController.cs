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
                    interactableObjectControllers.Add(ioc);
                    return;
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
        InteractableObjectController obj = interactableObjectControllers[0]; // pick closest interactable object
        float distance = 10;
        foreach(InteractableObjectController ioc in interactableObjectControllers)
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
        interactableObjectControllers.Sort();
    }
}