using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToButton : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        panel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        panel.SetActive(false);
    }

}
