using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions: MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject CreditsMenu;
    public GameObject MainMenu;
    public GameObject ConfirmQuitWindow;

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
        OptionsMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
        ConfirmQuitWindow.SetActive(false);

    }

    public void OpenOptionsMenu()
    {
        MainMenu.SetActive(false);
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        ConfirmQuitWindow.SetActive(false);

    }
    public void OpenCreditsMenu()
    {
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        ConfirmQuitWindow.SetActive(false);

    }

    public void ConfirmQuit()
    {
        ConfirmQuitWindow.SetActive(true);
    }

    public void QuitGame()
    {
        GameManager.ManagerQuitGame();
    }

}
