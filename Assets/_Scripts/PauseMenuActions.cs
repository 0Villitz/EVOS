using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuActions : MonoBehaviour
{
    private GatherInput gI;
    private bool isPaused;
    public GameObject PauseMenu;
    public GameObject DarkenPanel;
   
    // Start is called before the first frame update
    void Start()
    {

        
        gI = GameObject.Find("_Player").GetComponent<GatherInput>();
        if (gI == null)
            Debug.Log("No Player Found");

        DarkenPanel.SetActive(false);
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle pause Menu
        if (gI.pause & !isPaused)
        {
            // Pause Game
            DarkenPanel.SetActive(true);
            PauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
            isPaused = true;
        }

        if (!gI.pause & isPaused)
        {
            // Resume / Unpause Game
            DarkenPanel.SetActive(false);
            PauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            isPaused = false;
        }

    }

    public void Resume()
    {
        DarkenPanel.SetActive(false);
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        // TODO: Ask for confirmation
        GameManager.ManagerLoadLevel(0);

    }
}
