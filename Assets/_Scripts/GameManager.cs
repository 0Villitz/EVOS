using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton
public class GameManager : MonoBehaviour
{
    private static GameManager GM;
    private Fader fader;
    void Awake()
    {

        // Make sure there is only ever one copy of this script:
        if (GM == null)
        {
            GM = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public static void RegisterFader(Fader fD)
    {
        if (GM == null)
        {
            return;
        }
        GM.fader = fD;
    }

    public static void ManagerLoadLevel(int index)
    {
        Debug.Log("ManagerLoadLevel: Index number " + index);
        if (GM == null)
        {
            return;
        }
        GM.fader.SetLevel(index);
    }

    public static void ManagerRestartLevel()
    {
        if (GM == null)
            return;
        GM.fader.RestartLevel();
    }

    public static void ManagerQuitGame()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
