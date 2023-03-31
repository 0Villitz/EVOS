using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour, IPlayerInteractable, IPlayerRespawn
{
    private float interactCutoffDistance = 5f;
    public float InteractCutoffDistance => interactCutoffDistance;

    [SerializeField] AudioSource VentOpenSound;
    [SerializeField] AudioSource VentCloseSound;

    private void Start()
    {

    }

    public Transform GeTransform() => this.transform;
    public void Interact()
    {
        GetComponentInParent<VentController>().InteractWithVent();
    }
    public void OpenVentAnimation()
    {
        GetComponent<Animator>().Play("Vent_Open");
        if (VentOpenSound != null && VentOpenSound.clip != null)
        {
            VentOpenSound.Play();
        }
    }

    public void CloseVentAnimation()
    {
        GetComponent<Animator>().Play("Vent_Closed");
        if (VentCloseSound != null && VentCloseSound.clip != null)
        {
            VentCloseSound.Play();
        }
    }
}
