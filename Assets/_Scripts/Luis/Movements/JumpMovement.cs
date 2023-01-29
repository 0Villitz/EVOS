
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class JumpMovement : IMovement2DAction
    {
        private readonly IGameUnit _gameUnit;
        private readonly float _jumpSpeed;

        public JumpMovement(IGameUnit gameUnit, float jumpSpeed)
        {
            _gameUnit = gameUnit;
            _jumpSpeed = jumpSpeed;
        }

        public UnitAnimations Execute(
            ref Vector2 direction2d, 
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            if (!CanJump(interactableObjects))
            {
                return UnitAnimations.Idle;
            }
            
            if (_gameUnit.IsGrounded && movement.y <= 0f && direction2d.y > 0)
            {
                movement.Set(
                    movement.x,
                    _jumpSpeed
                );
            }

            direction2d.Set(direction2d.x, 0);

            return (movement.y > 0)
                ? UnitAnimations.Jump
                : UnitAnimations.Idle;
        }

        private bool CanJump(List<IInteractableObject> interactableObjects)
        {
            return interactableObjects == null
                   || !interactableObjects.Exists(
                       x => { return x.BlockJump(); }
                   );
        }
    }
}