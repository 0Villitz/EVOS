
using UnityEngine;
public interface IPlayerInteractable
{
    float InteractCutoffDistance { get; }
    
    void Interact();
}
