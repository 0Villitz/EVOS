using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource backgroundNoiseA;
    public AudioSource backgroundNoiseB;
    // Start is called before the first frame update
    void Start()
    {
        backgroundNoiseA.Play();
        backgroundNoiseB.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
