
using UnityEngine;

namespace Game2D
{
    public interface IPlayerCharacter : IInteractableObject
    {
        Transform GetTransform();
        void TakeDamage(int damage, IAttackerObject attackingObject);
        int GetHealth();
        bool IsHiding { get; }
    }
}