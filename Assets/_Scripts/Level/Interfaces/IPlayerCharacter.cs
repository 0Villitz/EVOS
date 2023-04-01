
using UnityEngine;

namespace Game2D
{
    public interface IPlayerCharacter : IInteractableObject
    {
        Transform GetTransform();
        void TakeDamage(IAttackerObject attackingObject);
        int GetHealth();
        bool CanBeDetected();
    }
}