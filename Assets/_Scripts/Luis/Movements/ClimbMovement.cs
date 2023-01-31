
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class ClimbMovement : IMovement2DAction
    {
        private readonly float _movementSpeed;
        
        public ClimbMovement(float movementSpeed)
        {
            _movementSpeed = movementSpeed;
        }
        
        public UnitAnimations Execute(
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
                ? UnitAnimations.ClimbUp
                : direction2d.y < -float.Epsilon
                    ? UnitAnimations.ClimbDown
                    : UnitAnimations.Climb;
        }
    }
}
