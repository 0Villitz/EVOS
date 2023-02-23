
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class JumpAction : IUnitAction
    {
        private readonly IJumpUnit _gameUnit;
        private readonly float _jumpSpeed;

        public JumpAction(IJumpUnit gameUnit, float jumpSpeed)
        {
            _gameUnit = gameUnit;
            _jumpSpeed = jumpSpeed;
        }

        public UnitMovement Execute(
            ref Vector2 direction2d, 
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            if (!_gameUnit.CanJump() || direction2d.y < 1)
            {
                return movement.y > 0
                    ? UnitMovement.Jump
                    : UnitMovement.Idle;
            }
            
            if (movement.y <= 0f && direction2d.y > 0)
            {
                movement.Set(
                    movement.x,
                    _jumpSpeed
                );
                
                direction2d.Set(direction2d.x, 0);
                
                return UnitMovement.Jump;
            }

            return UnitMovement.Idle;
        }
    }
}