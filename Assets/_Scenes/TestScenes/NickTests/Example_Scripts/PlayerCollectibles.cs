using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCollectibles : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    public int gemNumber;
    // Start is called before the first frame update
    void Start()
    {
        // I am not sure if this will work. It is different from the tutorial version.
        textComponent = GameObject.FindGameObjectsWithTag("CollectibleUI")[0].GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }

    private void UpdateText()
    {
        textComponent.text = gemNumber.ToString();
    }

    public void GemCollected()
    {
        gemNumber++;
        UpdateText();
    }
}
