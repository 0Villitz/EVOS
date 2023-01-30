
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour, IGameUnit, IUnitState, IFallMovementUnit, IJumpMovementUnit
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

        private InputData _inputData = null;

        private float _gravitySpeed;

        #region IUnitState

        public void Initialize()
        {
            _characterController = GetComponent<CharacterController>();

            _gravitySpeed = _gravity * _gravityMultiplier;

            _movementActions = new List<IMovement2DAction>()
            {
                new HorizontalMovement(_movementSpeed),
                new JumpMovement(this, _jumpSpeed),
                new FallMovement(this, _gravitySpeed)
            };
        }

        public UnitAnimations ProcessInput(InputData inputData)
        {
            _inputData = inputData;

            _movementDirection.Set(
                _inputData.horizontal,
                _inputData.vertical
            );

            UnitAnimations frameUnitAnimation = UnitAnimations.Idle;

            foreach (IMovement2DAction movement2DAction in _movementActions)
            {
                frameUnitAnimation |= movement2DAction.Execute(
                    ref _movementDirection,
                    ref _movementSpeed2D,
                    _inputData.InteractableEntities
                );
            }

            Vector3 movement = (
                transform.forward * _movementSpeed2D.x
                + transform.up * _movementSpeed2D.y
            ) * Time.deltaTime;

            _characterController.Move(movement);

            _inputData = null;
            
            return frameUnitAnimation;
        }

        #endregion

        #region IGameUnit

        public bool IsGrounded => _characterController.isGrounded;

        #endregion

        private bool IsGoingDownSlop()
        {
            // Ground Layer = 7
            int layerMask = 1 << 7;

            RaycastHit hit;
            if (
                Physics.Raycast(
                    transform.position,
                    transform.TransformDirection(Vector3.down),
                    out hit,
                    Mathf.Infinity,
                    layerMask
                )
            )
            {
                GroundObject _groundObject = hit.collider.GetComponent<GroundObject>();
                return (_groundObject != null && hit.distance <= _groundObject.MaxPlayerDistance);
            }

            return false;
        }

        #region IFallMovementUnit

        public bool IsMovingDownSlop()
        {
            return IsGoingDownSlop();
        }

        public bool CanFall()
        {
            bool isGrounded = _characterController.isGrounded;

            if (!isGrounded && _inputData != null && _inputData.interactWithEntities)
            {
                isGrounded = _inputData.InteractableEntities?.Exists(
                    x => x.BlockGravity()
                ) ?? false;
            }

            return !isGrounded;
        }

        #endregion

        #region IJumpMovementUnit

        public bool CanJump()
        {
            bool canJump = !CanFall() || IsGoingDownSlop();
            if (canJump)
            {
                canJump = (
                    _inputData == null
                    || !_inputData.interactWithEntities
                    || (_inputData.InteractableEntities?.Exists(
                        x => x.BlockJump()
                    ) ?? false)
                );
            }

            return canJump;
        }

        #endregion
    }
}