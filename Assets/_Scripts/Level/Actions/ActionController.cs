
using System.Collections.Generic;
using Game2D.GamePhysics;
using UnityEngine;

namespace Game2D
{
    public class ActionController : MonoBehaviour, IUnitState, IFallUnit, IJumpUnit
    {
        private CharacterController _characterController;

        [SerializeField] private float _jumpSpeed = 5f;
        public float JumpSpeed => _jumpSpeed;
        
        [SerializeField] private float _gravityMultiplier = 0.03f;
        private float _gravity = -9.81f;

        [SerializeField] private float _movementSpeed = 5;
        public float MovementSpeed => _movementSpeed;
        
        [SerializeField] private int _attackDamage = 100;
        public int AttackDamage => _attackDamage;
        
        private int _groundDirection = 0;
        private float _groundSpeed = 0f;

        private Vector2 _movementSpeed2D = Vector2.zero;
        private Vector2 _movementDirection = Vector2.zero;

        private Dictionary<UnitMovement, IUnitAction> _actionsMap;

        private PlayerController _playerController;

        private InputData _inputData = null;

        private float _gravitySpeed;
        public float GravitySpeed => _gravitySpeed;

        [SerializeField] private CharacterAnimation _characterAnimation;
        
        private UnitMovement _lastUnitMovement = UnitMovement.Idle;
        
        #region IUnitState
        
        public void Initialize(Dictionary<CharacterActionState, UnitMovement[]> unitMovementMap)
        {
            _gravitySpeed = _gravity * _gravityMultiplier;

            _actionsMap = new Dictionary<UnitMovement, IUnitAction>();
            if (unitMovementMap != null)
            {
                foreach (UnitMovement [] unitMovements in unitMovementMap.Values)
                {
                    foreach (UnitMovement unitMovement in unitMovements)
                    {
                        if (!_actionsMap.TryGetValue(unitMovement, out IUnitAction action)
                            || action == null
                           )
                        {
                            _actionsMap[unitMovement] = ActionBuilder.Build(unitMovement, this);
                        }
                    }
                }
            }
        }

        public UnitMovement ProcessInput(UnitMovement [] actionTypes, InputData inputData, Component component)
        {
            _characterController = component as CharacterController;
            _inputData = inputData;
            UnitMovement frameUnitMovement = UnitMovement.Idle;
            
            if (_characterController != null)
            {
                _movementDirection.Set(
                    _inputData.horizontal,
                    _inputData.vertical
                );

                for (int i = 0; i < actionTypes.Length; i++)
                {
                    UnitMovement movementType = actionTypes[i];
                    if (_actionsMap.TryGetValue(movementType, out IUnitAction action)
                        && action != null
                        )
                    {
                        frameUnitMovement |= action.Execute(
                            ref _movementDirection,
                            ref _movementSpeed2D,
                            _inputData.InteractableEntities
                        );
                    }
                    else
                    {
                        Debug.LogError("Missing action type: " + movementType);
                    }
                }

                Vector3 movement = (
                    _characterController.transform.forward * _movementSpeed2D.x
                    + _characterController.transform.up * _movementSpeed2D.y
                ) * Time.deltaTime;

                _characterController.Move(movement);
            }

            _inputData = null;
            
            if (frameUnitMovement != _lastUnitMovement)
            {
                if (_characterAnimation != null)
                {
                    _characterAnimation.TriggerAnimation(frameUnitMovement);
                }

                _lastUnitMovement = frameUnitMovement;
            }
            
            return frameUnitMovement;
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

        #region IFallUnit

        public bool IsMovingDownSlop()
        {
            return IsGoingDownSlop();
        }

        public bool CanFall()
        {
            return !(_characterController?.isGrounded ?? false);
        }

        #endregion

        #region IJumpUnit

        public bool CanJump()
        {
            return !CanFall() || IsGoingDownSlop();
        }

        #endregion

    }
}