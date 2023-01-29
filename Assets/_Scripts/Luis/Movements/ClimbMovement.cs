
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game2D
{
    public class ClimbMovement : IMovement2DAction
    {
        private readonly float _climbSpeed;
        private IGameUnit _gameUnit;
        
        public ClimbMovement(IGameUnit gameUnit, float climbSpeed)
        {
            _climbSpeed = climbSpeed;
            _gameUnit = gameUnit;
        }

        public UnitAnimations Execute(
            ref Vector2 direction2d,
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            MovementController mc = _gameUnit as MovementController;
            ;
            if (!CanClimb(interactableObjects))
            {
                return UnitAnimations.Idle;
            }

            movement.Set(
                movement.x,
                _climbSpeed * direction2d.y
            );

            return (movement.y > float.Epsilon)
                ? UnitAnimations.ClimbUp
                : (movement.y < -float.Epsilon)
                    ? UnitAnimations.ClimbDown
                    : UnitAnimations.Idle;
        }

        private bool CanClimb(List<IInteractableObject> interactableObjects)
        {
            return interactableObjects != null
                   && interactableObjects.Exists(
                       x => { return x is Ladder; }
                   );
        }
    }
}