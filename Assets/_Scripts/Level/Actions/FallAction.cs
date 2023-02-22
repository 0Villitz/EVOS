
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class FallAction : IUnitAction
    {
        private readonly IFallUnit _gameUnit;
        private float _speed;
        private readonly float _gravitySpeed;

        public FallAction(IFallUnit gameUnit, float gravitySpeed)
        {
            _gameUnit = gameUnit;
            _gravitySpeed = gravitySpeed;
        }

        public UnitMovement Execute(
            ref Vector2 direction2d,
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            bool canUnitFall = _gameUnit.CanFall();
            bool canApplyGravity = (canUnitFall || movement.y > 0);

            float speed = canApplyGravity
                ? movement.y + _gravitySpeed
                : -1;

            movement.Set(
                movement.x,
                speed
            );

            return _gameUnit.IsMovingDownSlop()
                ? UnitMovement.Idle
                : UnitMovement.Falling;
        }
    }
}