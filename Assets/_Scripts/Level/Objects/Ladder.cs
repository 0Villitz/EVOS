
using UnityEngine;

namespace Game2D
{
    public class Ladder : MonoBehaviour, IClimbObject
    {
        #region IInteractableObject

        public Vector3 WorldPosition => transform.position;

        public void Interact(ICharacterController characterController)
        {
            characterController.GrabClimbObject(this);
        }

        #endregion
    }
}