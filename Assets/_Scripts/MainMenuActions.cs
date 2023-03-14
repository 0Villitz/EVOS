using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions: MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject MainMenu;

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
        MainMenu.SetActive(true);
        
    }

    public void OpenOptionsMenu()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);

    }

    public void QuitGame()
    {
        // TODO: Ask for confirmation
        GameManager.ManagerQuitGame();
    }

    // Options Menu Functions



}
