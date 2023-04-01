using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutController : MonoBehaviour
{
    [SerializeField] private Image blackOutSquare;
    private GatherInput inputController;

    [SerializeField] private TMP_Text _label;

    public TMP_Text Label => _label;
    
    // Start is called before the first frame update
    void Start()
    {
        inputController = FindObjectOfType<GatherInput>();
    }

    public void StandardFadeOut()
    {
        StartCoroutine(StandardFadeOutCo());
    }

    private IEnumerator StandardFadeOutCo(float fadeSpeed = 0.9f)
    {
        Color objectColor = blackOutSquare.color;
        float fadeAmount;

        while (blackOutSquare.color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }
    }

    public void StandardFadeIn()
    {
        StartCoroutine(StandardFadeInCo());
    }

    private IEnumerator StandardFadeInCo(float fadeSpeed = 0.9f)
    {
        Color objectColor = blackOutSquare.color;
        float fadeAmount;

        while (blackOutSquare.color.a > 0)
        {
            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }
    }

    public void PlayBlackOutDoorTransition(InteractableDoor door)
    {
        StartCoroutine(DoorBlackout(door));
    }

    public IEnumerator BlackoutScreen(float fadeSpeed)
    {
        float fadeAmount = Mathf.Clamp01(blackOutSquare.color.a);
        while (fadeAmount <= 1 && fadeAmount >= 0)
        {
            fadeAmount+= (fadeSpeed * Time.deltaTime);

            blackOutSquare.color = new Color(
                blackOutSquare.color.r,
                blackOutSquare.color.g,
                blackOutSquare.color.b,
                Mathf.Clamp01(fadeAmount)
            );
            yield return null;
        }
    }
    
    public IEnumerator DoorBlackout(InteractableDoor door, float fadeSpeed = 0.9f)
    {
        inputController.CurrentControlType = GatherInput.ControlType.None;
        
        Color objectColor = blackOutSquare.color;
        float fadeAmount;

        yield return new WaitForSeconds(0.25f);

        inputController.CurrentControlType = GatherInput.ControlType.None;

        while (blackOutSquare.color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }

        inputController.transform.position = door.transform.position;

        yield return new WaitForSeconds(1f);

        door.GetComponentInParent<DoorController>().CloseDoorsAfterFade();

        while (blackOutSquare.color.a > 0)
        {
            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.color = objectColor;
            yield return null;
        }

        inputController.CurrentControlType = GatherInput.ControlType.Player;
    }

    
}
