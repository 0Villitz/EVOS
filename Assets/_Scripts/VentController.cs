using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VentController : MonoBehaviour
{
    //These doors should be the children in the Doors Prefab.
    //Each set of 2 doors will interact with each other, taking the player to the other one.
    [SerializeField] private Vent firstVent;
    [SerializeField] private Vent secondVent;

    private GatherInput inputController;

    private BlackoutController blackout;
    private GameObject player;

    private bool isVentLocked = false;
    public bool IsVentLocked
    {
        get
        {
            return isVentLocked;
        }
    }


    private bool canUseDoor = true;
    [SerializeField] public string interactionPrompt = "";
    private TextMeshProUGUI promptTextDisplay;
    private GameObject UIObject;
    // Start is called before the first frame update
    void Start()
    {

        inputController = FindObjectOfType<GatherInput>();

        UIObject = GameObject.Find("_UI");
        if (UIObject != null)
        {
            promptTextDisplay = UIObject.GetComponent<TextMeshProUGUI>();
        }

        if (blackout == null)
        {
            blackout = FindObjectOfType<BlackoutController>();
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }


    public void InteractWithVent()
    {
        if (canUseDoor && !isVentLocked)
        {
            Vent furthestVent;

            float door1Distance = Vector3.Distance(player.transform.position, firstVent.transform.position);
            float door2Distance = Vector3.Distance(player.transform.position, secondVent.transform.position);

            furthestVent = door1Distance > door2Distance ? firstVent : secondVent;
            inputController.CurrentControlType = GatherInput.ControlType.None;
            OpenVents();
            StartCoroutine(WaitAndTeleportToOtherVent(furthestVent));
        }
    }

    private IEnumerator WaitAndTeleportToOtherVent(Vent furthestVent)
    {
        yield return new WaitForSeconds(2.5f);
        player.transform.position = furthestVent.transform.position;
        inputController.CurrentControlType = GatherInput.ControlType.Player;
        yield return new WaitForSeconds(.5f);
        CloseVents();
    }

    public void UnlockDoors()
    {
        isVentLocked = false;
    }

    public void CloseDoorsAfterFade()
    {
        StartCoroutine(CloseDoorsAfterWait());
    }

    private IEnumerator CloseDoorsAfterWait(float time = 0.5f)
    {
        yield return new WaitForSeconds(time);

        CloseVents();
    }

    public void OpenVents()
    {
        firstVent.OpenVentAnimation();
        secondVent.OpenVentAnimation();
    }

    public void CloseVents()
    {
        firstVent.CloseVentAnimation();
        secondVent.CloseVentAnimation();
    }

    public void Interact()
    {
        InteractWithVent();
    }
}
