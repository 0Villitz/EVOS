using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour, IPlayerInteractable
{
    private float _InteractCutoffDistance = 2f;
    public float InteractCutoffDistance => _InteractCutoffDistance;

    [SerializeField] AudioSource DoorOpenSound;
    [SerializeField] AudioSource DoorCloseSound;

    private void Start()
    {
        
    }

    public void Interact()
    {
        GetComponentInParent<DoorController>().InteractWithDoor();
    }
    public void OpenDoorAnimationAndSound()
    {
        GetComponent<Animator>().Play("Door_Open");
        if (DoorOpenSound != null && DoorOpenSound.clip != null)
        {
            DoorOpenSound.Play();
        }
    }

    public void CloseDoorAnimationAndSound()
    {
        GetComponent<Animator>().Play("Door_Closed");
        if (DoorCloseSound != null && DoorCloseSound.clip != null)
        {
            DoorCloseSound.Play();
        }
    }
}