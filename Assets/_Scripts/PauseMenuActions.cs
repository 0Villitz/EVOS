using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuActions : MonoBehaviour
{
    private GatherInput gI;
    private bool isPaused;
    private bool shouldResume;
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
        
        isPaused = false;
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle pause Menu using inputs
        if (gI.pause && !isPaused)
        {
            PauseGame();
            isPaused = true;
            
        }
        
        if (!gI.pause && isPaused)
        {
            ResumeGame();
            isPaused = false;
            
        }
        
        if (shouldResume && isPaused)
        {
            ResumeGame();
            isPaused = false;
            shouldResume = false;
        }

    }

    public void PauseGame()
    {

        Debug.Log("Pause Game");
        
        isPaused = true;
        DarkenPanel.SetActive(true);
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }
        

    public void ResumeGame()
    {
        shouldResume = true;
        
        Debug.Log("Resume Game");
        PauseMenu.SetActive(false);
        
        isPaused = false;
        DarkenPanel.SetActive(false);
        Time.timeScale = 1.0f;

    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quit Button Pressed");
        // TODO: Ask for confirmation
        ResumeGame();
        GameManager.ManagerLoadLevel(0);

    }
}
