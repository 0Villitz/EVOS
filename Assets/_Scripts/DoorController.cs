using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEditor.UIElements;

public class DoorController : MonoBehaviour
{
    //These doors should be the children in the Doors Prefab.
    //Each set of 2 doors will interact with each other, taking the player to the other one.
    [SerializeField] private InteractableDoor firstDoor;
    [SerializeField] private InteractableDoor secondDoor;

    private BlackoutController blackout;
    private GameObject player;

    private bool isDoorLocked = false;
    public bool IsDoorLocked
    {
        get
        {
            return isDoorLocked;
        }
    }
    

    private bool canUseDoor = true;
    [SerializeField] public string interactionPrompt = "";
    private TextMeshProUGUI promptTextDisplay;
    private GameObject UIObject;
    // Start is called before the first frame update
    void Start()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            promptTextDisplay.text = interactionPrompt;
            //canUseDoor = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //remove prompt
            //canUseDoor = false;
        }
    }

    public void InteractWithDoor()
    {
        if (canUseDoor && !isDoorLocked)
        {
            InteractableDoor furthestDoor;

            float door1Distance = Vector3.Distance(player.transform.position, firstDoor.transform.position);
            float door2Distance = Vector3.Distance(player.transform.position, secondDoor.transform.position);

            furthestDoor = door1Distance > door2Distance ? firstDoor : secondDoor;

            OpenDoors();
            blackout.PlayBlackOutDoorTransition(furthestDoor);
        }
    }

    public void UnlockDoors()
    {
        isDoorLocked = false;
    }

    public void CloseDoorsAfterFade()
    {
        StartCoroutine(CloseDoorsAfterWait());
    }

    private IEnumerator CloseDoorsAfterWait(float time = 0.5f)
    {
        yield return new WaitForSeconds(time);

        CloseDoors();
    }

    public void OpenDoors()
    {
        firstDoor.OpenDoorAnimationAndSound();
        secondDoor.OpenDoorAnimationAndSound();
    }

    public void CloseDoors()
    {
        firstDoor.CloseDoorAnimationAndSound();
        secondDoor.CloseDoorAnimationAndSound();
    }

    public void Interact()
    {
        InteractWithDoor();
    }
}
