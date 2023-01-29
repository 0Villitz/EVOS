
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game2D
{
    public class FallMovement : IMovement2DAction
    {
        private readonly IGameUnit _gameUnit;
        private float _speed;
        private readonly float _gravity;
        private readonly float _gravityMultiplier;

        public FallMovement(IGameUnit gameUnit, float gravity, float gravityMultiplier)
        {
            _gameUnit = gameUnit;
            _gravity = gravity;
            _gravityMultiplier = gravityMultiplier;
        }

        public UnitAnimations Execute(
            ref Vector2 direction2d,
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            bool isGrounded = IsGrounded(movement.y, interactableObjects);

            float speed = isGrounded
                ? -1
                : movement.y + _gravity * _gravityMultiplier;

            movement.Set(
                movement.x,
                speed
            );

            return isGrounded
                ? UnitAnimations.Idle
                : UnitAnimations.Falling;
        }

        private bool IsGrounded(float yVelocity, List<IInteractableObject> interactableObjects)
        {
            return _gameUnit.IsGrounded
                   && yVelocity <= 0f
                   && (interactableObjects == null
                       || !interactableObjects.Exists(
                           x => { return x.BlockGravity(); }
                       )
                   );
        }
    }
}