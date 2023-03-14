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
        // Toggle pause Menu
        if (gI.pause & !isPaused)
        {
            isPaused = true;
            PauseMenu.SetActive(true);
            Time.timeScale = 0.0f;
        }
        if (!gI.pause & isPaused)
        {
            isPaused = false;
            PauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
        }

    }

    public void QuitToMainMenu()
    {
        // TODO: Ask for confirmation
        GameManager.ManagerLoadLevel(0);

    }
}
