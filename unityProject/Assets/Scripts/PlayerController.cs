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
    Animator leftWeaponHolderAnim;
    [SerializeField]
    Transform rightWeaponHolder;
    Animator rightWeaponHolderAnim;

    public WeaponController weaponLeft;
    public WeaponController weaponRight;
    public HealthController healthController;
    public GameObject cam;

    public MeleeColliderController leftMeleeCollider;
    public MeleeColliderController rightMeleeCollider;

    bool leftWeaponMelee = false;
    bool rightWeaponMelee = false;

    GameManager gm;

    public LayerMask clippingLayerMask;

    float moveFB;
    float moveLR;

    float rotX;
    float rotY;

    GameObject lwho;
    GameObject rwho;

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

        leftWeaponHolderAnim = leftWeaponHolder.GetComponent<Animator>();
        rightWeaponHolderAnim = rightWeaponHolder.GetComponent<Animator>();
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
        {
            if (!leftWeaponMelee)
                weaponLeft.Shoot("PlayerProjectile");
            else
            {
                StartCoroutine(MeleeAttack(leftMeleeCollider));
                weaponLeft.Melee();
            }
        }

        if (Input.GetButtonDown("Fire2") && weaponRight)
        {
            if (!rightWeaponMelee)
                weaponRight.Shoot("PlayerProjectile");
            else
            {
                StartCoroutine(MeleeAttack(rightMeleeCollider));
                weaponRight.Melee();
            }
        }
    }

    IEnumerator MeleeAttack(MeleeColliderController meleeColliderController)
    {
        meleeColliderController.Dangerous(gameObject, true);
        yield return new WaitForSeconds(0.1f);
        meleeColliderController.Dangerous(gameObject, false);
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
        if (Physics.Raycast(lwho.transform.position, cam.transform.forward, 1.5f, clippingLayerMask))
        {
            leftWeaponHolderAnim.SetBool("Clipping", true);
            leftWeaponMelee = true;
        }
        else
        {
            leftWeaponHolderAnim.SetBool("Clipping", false);
            leftWeaponMelee = false;
        }

        if (Physics.Raycast(rwho.transform.position, cam.transform.forward, 1.5f, clippingLayerMask))
        {
            rightWeaponHolderAnim.SetBool("Clipping", true);
            rightWeaponMelee = true;
        }
        else
        {
            rightWeaponHolderAnim.SetBool("Clipping", false);
            rightWeaponMelee = false;
        }
    }

    public void PickUpWeapon(InteractableObjectController wpn, Transform hand)
    {
        if (hand == leftWeaponHolder)
        {
            weaponLeft = wpn.weapon;
            weaponLeft.PickUp(leftWeaponHolderAnim, this, null);
            weaponLeft.transform.SetParent(leftWeaponHolder);
            weaponLeft.transform.localEulerAngles = Vector3.zero;
            weaponLeft.transform.localPosition = Vector3.zero;
        }
        else
        {
            weaponRight = wpn.weapon;
            weaponRight.PickUp(rightWeaponHolderAnim, this, null);
            weaponRight.transform.SetParent(rightWeaponHolder);
            weaponRight.transform.localEulerAngles = Vector3.zero;
            weaponRight.transform.localPosition = Vector3.zero;
        }
    }
}