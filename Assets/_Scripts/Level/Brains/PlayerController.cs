
using System.Collections.Generic;
using Game2D.GamePhysics;
using UnityEngine;

namespace Game2D
{
    public class PlayerController : BaseController
    {
        protected override void Initialize()
        {
            _actionsToStateMap = new Dictionary<CharacterActionState, UnitMovement[]>()
            {
                [CharacterActionState.FreeMovement] = new[]
                {
                    UnitMovement.MoveHorizontal,
                    UnitMovement.Jump,
                    UnitMovement.Falling
                },
                [CharacterActionState.Climbing] = new[]
                {
                    UnitMovement.MoveHorizontal,
                    UnitMovement.Climb,
                }
            };
            
            base.Initialize();
        }
        
        #region Monobehavior

        void Awake()
        {
            Initialize();
        }

        void Update()
        {
            ProcessInputs();

            switch (_currentState)
            {
                case CharacterActionState.FreeMovement:
                    if (_inputData.interactWithEntities)
                    {
                        bool interactingWithClimObject = _inputData.InteractableEntities?.Exists(
                            x => x is IClimbObject
                        ) ?? false;

                        if (interactingWithClimObject)
                        {
                            GamePhysicsHelper.IgnoreLayerCollision(
                                GamePhysicsHelper.Layers.Player,
                                GamePhysicsHelper.Layers.Platform,
                                true
                            );
                            _currentState = CharacterActionState.Climbing;
                        }
                    }

                    break;

                case CharacterActionState.Climbing:
                    if (!_inputData.interactWithEntities)
                    {
                        GamePhysicsHelper.IgnoreLayerCollision(
                            GamePhysicsHelper.Layers.Player,
                            GamePhysicsHelper.Layers.Platform,
                            false
                        );
                        _currentState = CharacterActionState.FreeMovement;
                    }

                    break;
            }

            ProcessState();
        }

        private void OnTriggerEnter(Collider other)
        {
            IInteractableObject interactableObject =
                other.gameObject.GetComponent<IInteractableObject>();

            if (interactableObject != null)
            {
                _inputData.AddInteractableEntity(interactableObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractableObject interactableObject =
                other.gameObject.GetComponent<IInteractableObject>();

            if (interactableObject != null)
            {
                _inputData.RemoveInteractableEntity(interactableObject);
            }
        }

        #endregion

        protected override void ProcessHorizontalInput()
        {
            switch (_currentState)
            {
                case CharacterActionState.FreeMovement:
                case CharacterActionState.Climbing:
                    if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
                    {
                        _inputData.SetHorizontal(0);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        _inputData.SetHorizontal(1);
                    }
                    else if (Input.GetKey(KeyCode.A))
                    {
                        _inputData.SetHorizontal(-1);
                    }

                    break;
            }
        }

        protected override void ProcessVerticalInput()
        {
            switch (_currentState)
            {
                case CharacterActionState.FreeMovement:
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        _inputData.SetVertical(1);
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        _inputData.SetVertical(-1);
                    }

                    break;
                case CharacterActionState.Climbing:
                    if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
                    {
                        _inputData.SetVertical(0);
                    }
                    else if (Input.GetKey(KeyCode.W))
                    {
                        _inputData.SetVertical(1);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        _inputData.SetVertical(-1);
                    }

                    break;
            }
        }

        protected override void ProcessObjectInteraction()
        {
            switch (_currentState)
            {
                case CharacterActionState.FreeMovement:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        _inputData.EnableInteractionWithEntities();
                        List<IInteractableObject> interactableEntities = _inputData.InteractableEntities;
                        if (interactableEntities != null)
                        {
                            foreach (IInteractableObject interactableEntity in interactableEntities)
                            {
                                interactableEntity.Interact(this);
                            }
                        }
                    }

                    break;
            }
        }

        public override void GrabClimbObject(IClimbObject climbObject)
        {
            float xDistance = climbObject.WorldPosition.x - transform.position.x;
            _characterController.Move(new Vector3(xDistance, 0, 0));
        }
    }
}