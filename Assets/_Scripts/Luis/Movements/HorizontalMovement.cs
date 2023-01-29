
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class HorizontalMovement : IMovement2DAction
    {
        private readonly float _movementSpeed;
        
        public HorizontalMovement(float movementSpeed)
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
                _movementSpeed * direction2d.x,
                movement.y
            );

            return direction2d.x > float.Epsilon
                ? UnitAnimations.MoveRight
                : direction2d.x < -float.Epsilon
                    ? UnitAnimations.MoveLeft
                    : UnitAnimations.Idle;
        }
    }
}