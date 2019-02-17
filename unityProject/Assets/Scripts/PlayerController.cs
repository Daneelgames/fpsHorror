using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    public float sensitivity = 2f;

    ActionAreaController actionAreaController;
    CharacterController characterController;

    [SerializeField]
    Transform leftWeaponHolder;
    [SerializeField]
    Transform rightWeaponHolder;

    public WeaponController weaponLeft;
    public WeaponController weaponRight;
    public HealthController healthController;
    public GameObject cam;

    float moveFB;
    float moveLR;

    float rotX;
    float rotY;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (healthController.health > 0)
        {
            Shooting();
            Move();
            Interactions();
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    public void SetActionAreaController(ActionAreaController area)
    {
        actionAreaController = area;
    }

    void Move()
    {
        moveFB = Input.GetAxisRaw("Vertical");
        moveLR = Input.GetAxisRaw("Horizontal");

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY += Input.GetAxis("Mouse Y") * sensitivity;

        if (rotY < -90) rotY = -90;
        else if (rotY > 90) rotY = 90;

        var movement = new Vector3(moveLR, 0, moveFB);
        transform.Rotate(0, rotX, 0);

        movement = transform.rotation * movement;
        characterController.Move(movement.normalized * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        cam.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1") && weaponLeft)
            weaponLeft.Shoot();

        if (Input.GetButtonDown("Fire2") && weaponRight)
            weaponRight.Shoot();
    }

    void Interactions()
    { 
        if (Input.GetButtonDown("LeftArm"))
        {
            if (weaponLeft)
            {
                weaponLeft.Trow();
                weaponLeft = null;
            }
            if (actionAreaController.interactableObjectControllers.Count > 0)
            {
                actionAreaController.Interact(leftWeaponHolder);
            }
        }
        if (Input.GetButtonDown("RightArm"))
        {
            if (weaponRight)
            {
                weaponRight.Trow();
                weaponRight = null;
            }
            if (actionAreaController.interactableObjectControllers.Count > 0)
            {
                actionAreaController.Interact(rightWeaponHolder);
            }
        }
    }

    public void PickUpWeapon(InteractableObjectController wpn, Transform hand)
    {
        wpn.weapon.PickUp();
        if (hand == leftWeaponHolder)
        {
            weaponLeft = wpn.weapon;
            weaponLeft.transform.SetParent(leftWeaponHolder);
            weaponLeft.transform.localEulerAngles = Vector3.zero;
            weaponLeft.transform.localPosition = Vector3.zero;
        }
        else
        {
            weaponRight = wpn.weapon;
            weaponRight.transform.SetParent(rightWeaponHolder);
            weaponRight.transform.localEulerAngles = Vector3.zero;
            weaponRight.transform.localPosition = Vector3.zero;
        }
    }
}