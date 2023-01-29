
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public interface IMovement2DAction
    {
        UnitAnimations Execute(
            ref Vector2 direction2d, 
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        );
    }
}