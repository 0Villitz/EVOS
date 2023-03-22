
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class AttackHorizontalAction : IUnitAction
    {
        private readonly int _attackDamage;
        
        public AttackHorizontalAction(int damage)
        {
            _attackDamage = damage;
        }

        public UnitMovement Execute(
            ref Vector2 direction2d,
            ref Vector2 movement,
            List<IInteractableObject> interactableObjects
        )
        {
            UnitMovement layerMovement = direction2d.x > float.Epsilon
                ? UnitMovement.MoveRight
                : direction2d.x < -float.Epsilon
                    ? UnitMovement.MoveLeft
                    : UnitMovement.Idle;

            movement.Set(0f, 0f);
            
            return UnitMovement.Attack | layerMovement;
        }
    }
    
}
