
using System.Collections.Generic;
using Game2D.GamePhysics;
using UnityEngine;

namespace Game2D
{
    public class MovementController : MonoBehaviour, IUnitState, IFallMovementUnit, IJumpMovementUnit
    {
        private CharacterController _characterController;

        [SerializeField] private float _jumpSpeed = 5f;
        [SerializeField] private float _gravityMultiplier = 0.03f;
        private float _gravity = -9.81f;

        [SerializeField] private float _movementSpeed = 5;
        private int _groundDirection = 0;
        private float _groundSpeed = 0f;

        private Vector2 _movementSpeed2D = Vector2.zero;
        private Vector2 _movementDirection = Vector2.zero;

        private Dictionary<UnitAnimations, IMovement2DAction> _actionsMap;

        private PlayerController _playerController;

        private InputData _inputData = null;

        private float _gravitySpeed;

        #region IUnitState

        public void Initialize()
        {
            _gravitySpeed = _gravity * _gravityMultiplier;

            _actionsMap = new Dictionary<UnitAnimations, IMovement2DAction>()
            {
                [UnitAnimations.MoveHorizontal] = new HorizontalMovement(_movementSpeed),
                [UnitAnimations.Jump] = new JumpMovement(this, _jumpSpeed),
                [UnitAnimations.Falling] = new FallMovement(this, _gravitySpeed),
                [UnitAnimations.Climb] = new ClimbMovement(_movementSpeed),
            };
        }

        public UnitAnimations ProcessInput(UnitAnimations [] actionTypes, InputData inputData, Component component)
        {
            _characterController = component as CharacterController;
            _inputData = inputData;
            UnitAnimations frameUnitAnimation = UnitAnimations.Idle;
            
            if (_characterController != null)
            {
                _movementDirection.Set(
                    _inputData.horizontal,
                    _inputData.vertical
                );

                for (int i = 0; i < actionTypes.Length; i++)
                {
                    UnitAnimations animationType = actionTypes[i];
                    if (_actionsMap.TryGetValue(animationType, out IMovement2DAction action)
                        && action != null
                        )
                    {
                        frameUnitAnimation |= action.Execute(
                            ref _movementDirection,
                            ref _movementSpeed2D,
                            _inputData.InteractableEntities
                        );
                    }
                    else
                    {
                        Debug.LogError("Missing action type: " + animationType);
                    }
                }

                Vector3 movement = (
                    _characterController.transform.forward * _movementSpeed2D.x
                    + _characterController.transform.up * _movementSpeed2D.y
                ) * Time.deltaTime;

                _characterController.Move(movement);
            }

            _inputData = null;
            
            return frameUnitAnimation;
        }

        #endregion

        private bool IsGoingDownSlop()
        {
            if (
                GamePhysicsHelper.RayCast(
                    transform.position,
                    transform.TransformDirection(Vector3.down),
                    Mathf.Infinity,
                    new GamePhysicsHelper.Layers[]
                    {
                        GamePhysicsHelper.Layers.Ground, 
                        GamePhysicsHelper.Layers.Platform
                    },
                    out RaycastHit hit
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
            return !(_characterController?.isGrounded ?? false);
        }

        #endregion

        #region IJumpMovementUnit

        public bool CanJump()
        {
            return !CanFall() || IsGoingDownSlop();
        }

        #endregion
    }
}