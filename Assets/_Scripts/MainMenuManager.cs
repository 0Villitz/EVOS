using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private int mainSceneBuildIndex = 1;

    //FUNC: Start the game (call the fader to load the correct index)
    public void StartGame()
    {
        GameManager.ManagerLoadLevel(1);
        //SceneManager.LoadScene(mainSceneBuildIndex);
    }

    //FUNC: Open the options menu

    //FUNC: Open the credits

    //FUNC: Quit

    //
}
