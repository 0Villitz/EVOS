
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class ClimbAction : IUnitAction
    {
        private readonly float _movementSpeed;
        
        public ClimbAction(float movementSpeed)
        {
            _movementSpeed = movementSpeed;
        }
        
        public UnitMovement Execute(
            ref Vector2 direction2d, 
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            movement.Set(
                movement.x,
                _movementSpeed * direction2d.y
            );

            return direction2d.y > float.Epsilon
                ? UnitMovement.ClimbUp
                : direction2d.y < -float.Epsilon
                    ? UnitMovement.ClimbDown
                    : UnitMovement.Climb;
        }
    }
}
