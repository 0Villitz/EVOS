using UnityEngine;

namespace Game2D
{
    public interface IClimbObject : IInteractableObject
    {
        Vector3 WorldPosition { get; }
    }
}