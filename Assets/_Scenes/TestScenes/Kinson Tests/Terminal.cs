using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        //panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if(hit.collider.tag == "Terminal")
            {
                OnClickTerminal();
            }
        }
    }

    public void OnClickTerminal()
    {
        //panel.SetActive(true);
        Debug.Log("Terminal Activated");
    }
}
