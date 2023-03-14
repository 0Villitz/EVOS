using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuActions : MonoBehaviour
{
    private GatherInput gI;
    private bool isPaused;
    public GameObject Fader;
    public GameObject PauseMenu;
    // Start is called before the first frame update
    void Start()
    {

        gI = GameObject.Find("_Player").GetComponent<GatherInput>();
        if (gI == null)
            Debug.Log("No Player Found");
        

        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Change this to use the new input system.
        if (gI.pause)
        {
            //Pause();
            TogglePause();
        }
    }

    private void Pause()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void TogglePause()
    {
        // Refrence to pause menu?
        if (isPaused)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            //AudioListener.pause = false;
            isPaused = false;

        }
        if (!isPaused)
        {

            PauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            //AudioListener.pause = true;
            isPaused = true;
        }

    }

    public void QuitToMainMenu()
    {
        // TODO: Ask for confirmation
        GameManager.ManagerLoadLevel(0);

    }
}
