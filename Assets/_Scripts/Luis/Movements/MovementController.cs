
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour, IGameUnit, IUnitState
    {
        private CharacterController _characterController;
        
        [SerializeField] private float _jumpSpeed = 5f;
        [SerializeField] private float _gravityMultiplier = 0.03f;
        private float _gravity = -9.81f;
        private float _verticalSpeed = 0f;
        
        [SerializeField] private float _movementSpeed = 5;
        private int _groundDirection = 0;
        private float _groundSpeed = 0f;

        private Vector2 _movementSpeed2D = Vector2.zero;
        private Vector2 _movementDirection = Vector2.zero;
            
        private List<IMovement2DAction> _movementActions;

        private PlayerController _playerController;
        
        #region IUnitState
        public void Initialize()
        {
            _characterController = GetComponent<CharacterController>();

            _movementActions = new List<IMovement2DAction>()
            {
                new HorizontalMovement(_movementSpeed),
                new JumpMovement(this, _jumpSpeed),
                new FallMovement(this, _gravity, _gravityMultiplier),
                new ClimbMovement(this, _movementSpeed)
            };
        }

        public UnitAnimations ProcessInput(InputData inputData)
        {
            _movementDirection.Set(
                _movementDirection.x + inputData.horizontal,
                _movementDirection.y + inputData.vertical
            );
            
            UnitAnimations frameUnitAnimation = UnitAnimations.Idle;
            
            foreach (IMovement2DAction movement2DAction in _movementActions)
            {
                frameUnitAnimation |= movement2DAction.Execute(
                    ref _movementDirection, 
                    ref _movementSpeed2D,
                    inputData.InteractableEntities
                );
            }
            
            Vector3 movement = (
                transform.forward * _movementSpeed2D.x
                + transform.up * _movementSpeed2D.y
            ) * Time.deltaTime;

            _characterController.Move(movement);

            return frameUnitAnimation;
        }
        #endregion

        #region IGameUnit
        public bool IsGrounded => _characterController.isGrounded;
        #endregion
    }
}