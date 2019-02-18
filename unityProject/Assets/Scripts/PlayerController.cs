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

    GameManager gm;

    public LayerMask clippingLayerMask;

    float moveFB;
    float moveLR;

    float rotX;
    float rotY;

    GameObject lwho;
    GameObject rwho;
    Vector3 leftWeaponHolderPositionNoClip = new Vector3(-0.35f, -0.4f, 0.1f);
    Vector3 rightWeaponHolderPositionNoClip = new Vector3(0.35f, -0.4f, 0.1f);
    Vector3 weaponHolderEulearNoClip = new Vector3(-55f, 0, 0);

    private void Awake()
    {
        gm = GameManager.instance;
        gm.SetPlayer(this);
        characterController = GetComponent<CharacterController>();

        lwho = new GameObject();
        lwho.transform.SetParent(cam.transform);
        lwho.transform.localPosition = leftWeaponHolder.transform.localPosition;
        rwho = new GameObject();
        rwho.transform.SetParent(cam.transform);
        rwho.transform.localPosition = rightWeaponHolder.transform.localPosition;
    }

    private void Update()
    {
        if (healthController.health > 0)
        {
            Shooting();
            Move();
            Interactions();
            PreventWeaponClipping();
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

        if (rotY < -50) rotY = -50;
        else if (rotY > 50) rotY = 50;

        var movement = new Vector3(moveLR, 0, moveFB);
        transform.Rotate(0, rotX, 0);

        movement = transform.rotation * movement;
        characterController.Move(movement.normalized * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        cam.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
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
                weaponLeft.Throw(15);
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
                weaponRight.Throw(15);
                weaponRight = null;
            }
            if (actionAreaController.interactableObjectControllers.Count > 0)
            {
                actionAreaController.Interact(rightWeaponHolder);
            }
        }
    }

    void PreventWeaponClipping()
    {
        if (Physics.Raycast(lwho.transform.position, transform.forward, 0.75f, clippingLayerMask))
        {
            leftWeaponHolder.localPosition = leftWeaponHolderPositionNoClip;
            leftWeaponHolder.localEulerAngles = weaponHolderEulearNoClip;
        }
        else
        {
            leftWeaponHolder.localPosition = lwho.transform.localPosition;
            leftWeaponHolder.localEulerAngles = Vector3.zero;
        }

        if (Physics.Raycast(rwho.transform.position, transform.forward, 0.75f, clippingLayerMask))
        {
            rightWeaponHolder.localPosition = rightWeaponHolderPositionNoClip;
            rightWeaponHolder.localEulerAngles = weaponHolderEulearNoClip;
        }
        else
        {
            rightWeaponHolder.localPosition = rwho.transform.localPosition;
            rightWeaponHolder.localEulerAngles = Vector3.zero;
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