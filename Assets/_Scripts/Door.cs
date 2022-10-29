using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.UIElements;

public class Door : MonoBehaviour
{
    [SerializeField] public string interactionPrompt = "";
    private TextMeshProUGUI promptTextDisplay;
    private GameObject UIObject;
    // Start is called before the first frame update
    void Start()
    {
        UIObject = GameObject.Find("_UI");
        promptTextDisplay = UIObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            promptTextDisplay.text = interactionPrompt;

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //remove prompt

        }
    }
}
