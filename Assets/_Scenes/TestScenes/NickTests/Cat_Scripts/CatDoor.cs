using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CatDoor : MonoBehaviour
{
    // Int to load level by Build Index:
    public int lvlToLoad;

    // Can also load by name:
    //public string lvl;

    void Start()
    {

    }

    private void LoadLevel()
    {
        // Load the scene bu build index
        SceneManager.LoadScene(lvlToLoad);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<BoxCollider2D>().enabled = false;
            collision.GetComponent<GatherInput>().DisableControls();
            LoadLevel();
        }
    }

}