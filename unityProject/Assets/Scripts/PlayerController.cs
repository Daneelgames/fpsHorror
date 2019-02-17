using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 2;
    public float sensitivity = 2f;
    CharacterController characterController;

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
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if (Input.GetKeyDown("r"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Move()
    {
        moveFB = Input.GetAxisRaw("Vertical");
        moveLR = Input.GetAxisRaw("Horizontal");

        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        var movement = new Vector3(moveLR, 0, moveFB);
        transform.Rotate(0, rotX, 0);
        cam.transform.Rotate(-rotY, 0, 0);
        movement = transform.rotation * movement;
        characterController.Move(movement.normalized * speed * Time.deltaTime);
    }

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1"))
            weaponLeft.Shoot();

        if (Input.GetButtonDown("Fire2"))
            weaponRight.Shoot();
    }
}