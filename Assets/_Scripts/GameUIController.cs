using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUIController : MonoBehaviour
{

    private GatherInput gI;
    //private InputAction pauseInput;

    // Pause Menu
    public GameObject pauseMenu;
    public GameObject DarkenPanel;
    public GameObject ConfirmMainMenuQuit;
    public GameObject ConfirmQuit;
    public GameObject Buttons;

    // Button Hints
    public GameObject pauseHint;
    public GameObject resumeHint;
    public GameObject inputHints;

    // Pause state variables
    private bool isPaused = false;
    private bool shouldResume = false;

    // Nick W.: UI settings (NOTE / TO-DO: Move this into Gamemanager and make a save system)
    //private bool showHints = true;
    private bool freezeWhilePaused = true;

    private void Awake()
    {
        return;
        // Look at https://www.youtube.com/watch?v=3zEpfMbE30s&ab_channel=TestSubjectGaming
        // for help fixing the toggle issue.
    }


    // Start is called before the first frame update
    void Start()
    {

        
        gI = GameObject.Find("_Player").GetComponent<GatherInput>();
        if (gI == null)
            Debug.Log("No Player Found");

        // Set default states of UI elements
        DarkenPanel.SetActive(false);
        pauseMenu.SetActive(false);
        ConfirmQuit.SetActive(false);
        ConfirmMainMenuQuit.SetActive(false);

        // Set pause button hint to correct value
        pauseHint.SetActive(false);
        resumeHint.SetActive(true);

        Time.timeScale = 1.0f;

        
    }


    void Update()
    {
        // Toggle pause Menu using inputs
        // This logic is messed up. The GatherInput script toggles the pause boolean
        // variable every time i hit esc. need to detect that it has changed.

        if (!isPaused && gI.pause){
            PauseGame();
            isPaused = true;
        }

        if (!gI.pause && isPaused){
            ResumeGame();
            isPaused = false;
        }

        if (shouldResume && isPaused){
            ResumeGame();
            isPaused = false;
            shouldResume = false;
        }

        // Set hint according to pause state:
        if (isPaused){ 
            pauseHint.SetActive(false);
            resumeHint.SetActive(true);
        }
        else{ 
            pauseHint.SetActive(true);
            resumeHint.SetActive(false);
        }

    }

    public void PauseGame()
    {

        isPaused = true;

        // Enable default pause menu buttons
        pauseMenu.SetActive(true);
        DarkenPanel.SetActive(true);
        Buttons.SetActive(true);

        // Ensure the confirmation windows are not enabled
        ConfirmQuit.SetActive(false);
        ConfirmMainMenuQuit.SetActive(false);

        // Change pause button hint
        pauseHint.SetActive(false);
        resumeHint.SetActive(true);

        // Freeze time or not while paused according to setting
        if (freezeWhilePaused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }
        

    public void ResumeGame()
    {
        // Disable all pause menu UI elements               
        ConfirmQuit.SetActive(false);
        ConfirmMainMenuQuit.SetActive(false);
        DarkenPanel.SetActive(false);
        Buttons.SetActive(false);
        pauseMenu.SetActive(false);

        // Change pause button hint
        pauseHint.SetActive(true);
        resumeHint.SetActive(false);

        // Resume game
        shouldResume = true;
        isPaused = false;
        Time.timeScale = 1.0f;

    }

    // Enable or diasable controls hints
    public void EnableControlHints(bool enableHints)
    {
        if (enableHints)
            inputHints.SetActive(true);
        else
            inputHints.SetActive(false);
    }

    public void ConfirmQuitMainMenu()
    {

        ConfirmMainMenuQuit.SetActive(true);
        ConfirmQuit.SetActive(false);
        Buttons.SetActive(false);

    }

    public void QuitToMainMenu()
    {
        ResumeGame();
        GameManager.ManagerLoadLevel(0);

    }

    public void ConfirmQuitGame()
    {
        ConfirmQuit.SetActive(true);

        ConfirmMainMenuQuit.SetActive(false);
        Buttons.SetActive(false);
    }

    public void QuitGame()
    {
        GameManager.ManagerQuitGame();
    }
}
