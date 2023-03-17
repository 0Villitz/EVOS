using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions: MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject mainMenu;
    public GameObject confirmQuitWindow;
    public GameObject mainMenuButtons;
    public GameObject evosLogo;

    private void Start()
    {
        Time.timeScale = 1.0f;
        // Make sure we start from the main menu
        OpenMainMenu();
    }

    //FUNC: Start the game (call the fader to load the correct index)
    public void StartGame()
    {
        // Refer to the build index ID in "File > Build Settings"
        // 0 = Main Menu
        // 1 = Main Scene
        // 3+ = Test Scenes
        GameManager.ManagerLoadLevel(1);
        //SceneManager.LoadScene(mainSceneBuildIndex);
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        mainMenuButtons.SetActive(true);
        evosLogo.SetActive(true);

        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        confirmQuitWindow.SetActive(false);

    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);

        mainMenu.SetActive(false);
        creditsMenu.SetActive(false);
        confirmQuitWindow.SetActive(false);

    }
    public void OpenCreditsMenu()
    {
        creditsMenu.SetActive(true);

        evosLogo.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);
        confirmQuitWindow.SetActive(false);

    }

    public void ConfirmQuit()
    {
        confirmQuitWindow.SetActive(true);
        evosLogo.SetActive(false);

        mainMenuButtons.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(false);

    }

    public void QuitGame()
    {
        GameManager.ManagerQuitGame();
    }

}
