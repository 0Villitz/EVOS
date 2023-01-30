
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game2D
{
    public class FallMovement : IMovement2DAction
    {
        private readonly IFallMovementUnit _gameUnit;
        private float _speed;
        private readonly float _gravitySpeed;

        public FallMovement(IFallMovementUnit gameUnit, float gravitySpeed)
        {
            _gameUnit = gameUnit;
            _gravitySpeed = gravitySpeed;
        }

        public UnitAnimations Execute(
            ref Vector2 direction2d,
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            bool canUnitFall = _gameUnit.CanFall();
            bool canApplyGravity = canUnitFall && movement.y < 0;

            float speed = canUnitFall && canApplyGravity
                ? movement.y + _gravitySpeed
                : -1;

            movement.Set(
                movement.x,
                speed
            );

            return _gameUnit.IsMovingDownSlop()
                ? UnitAnimations.Idle
                : UnitAnimations.Falling;
        }

        private bool CanFall(float yVelocity)
        {
            return _gameUnit.CanFall()
                   && yVelocity <= 0f;
        }
    }
}