
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class HorizontalAction : IUnitAction
    {
        private readonly float _movementSpeed;
        
        public HorizontalAction(float movementSpeed)
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
                _movementSpeed * direction2d.x,
                movement.y
            );

            return direction2d.x > float.Epsilon
                ? UnitMovement.MoveRight
                : direction2d.x < -float.Epsilon
                    ? UnitMovement.MoveLeft
                    : UnitMovement.Idle;
        }
    }
}