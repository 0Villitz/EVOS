
using UnityEngine;

namespace Game2D
{
    public class Ladder : MonoBehaviour, IInteractableObject
    {
        #region IInteractableObject
        public bool BlockGravity()
        {
            return true;
        }
        public bool BlockJump()
        {
            return true;
        }
        #endregion
    }
}