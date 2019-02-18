using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerController pc;

    public List<HealthController> healthControllers = new List<HealthController>();
    public List<InteractableObjectController> interactableObjectControllers = new List<InteractableObjectController>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Restart();
        }
    }

    public void SetPlayer(PlayerController _pc)
    {
        pc = _pc;
    }

    void Restart()
    {
        healthControllers.Clear();
        interactableObjectControllers.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddHealthController(HealthController health)
    {
        healthControllers.Add(health);
    }

    public void AddInteractableObject(InteractableObjectController interactableObjectController)
    {
        interactableObjectControllers.Add(interactableObjectController);
    }
}