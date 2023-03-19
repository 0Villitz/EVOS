using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutController : MonoBehaviour
{
    [SerializeField] private Image blackOutSquare;
    private GatherInput inputController;

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

    public IEnumerator DoorBlackout(InteractableDoor door, float fadeSpeed = 0.9f)
    {
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
