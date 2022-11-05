using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CatDoor : MonoBehaviour
{
    public int lvlToLoad;

    void Start()
    {

    }

    private void LoadLevel()
    {
        // Load the scene bu build index
        SceneManager.LoadScene(lvlToLoad);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadLevel();
        }
    }

}