using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Fader : MonoBehaviour
{
    private Animator anim;
    private int lvlToLoad;

    void Start()
    {
        anim = GetComponent<Animator>();

        GameManager.RegisterFader(this);
    }

    public void SetLevel(int lvl)
    {
        lvlToLoad = lvl;
        anim.SetTrigger("Fade");
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(lvlToLoad);
    }

    private void Restart()
    {
        SetLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        Invoke( "Restart" , 1.5f );
    }

}



  
