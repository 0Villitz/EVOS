
using System.Collections.Generic;
using Game2D.GamePhysics;
using Game2D.Inventory;
using UnityEngine;

namespace Game2D
{
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            None,
            FreeMovement,
            Climbing
        }

        [SerializeField] private State _currentState = State.FreeMovement;

        private UnitAnimations _lastUnitAnimation = UnitAnimations.Idle;

        private Dictionary<State, UnitAnimations[]> _actionsToStateMap;
        private IUnitState _activeController;

        private Dictionary<int, IInteractableObject> _interactableObjects = new Dictionary<int, IInteractableObject>();

        private InputData _inputData = new InputData();
        private CharacterController _characterController;

        #region Monobehavior

        void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            _actionsToStateMap = new Dictionary<State, UnitAnimations[]>()
            {
                [State.FreeMovement] = new[]
                {
                    UnitAnimations.MoveHorizontal,
                    UnitAnimations.Jump,
                    UnitAnimations.Falling
                },
                [State.Climbing] = new[]
                {
                    UnitAnimations.MoveHorizontal,
                    UnitAnimations.Climb,
                }
            };

            _activeController = GetComponent<MovementController>();
            _activeController.Initialize();
        }

        void Update()
        {
            _inputData.ResetInputs();

            ProcessHorizontalInput();
            ProcessVerticalInput();
            ProcessObjectInteraction();

            switch (_currentState)
            {
                case State.FreeMovement:
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
                            _currentState = State.Climbing;
                        }
                    }

                    break;

                case State.Climbing:
                    if (!_inputData.interactWithEntities)
                    {
                        GamePhysicsHelper.IgnoreLayerCollision(
                            GamePhysicsHelper.Layers.Player,
                            GamePhysicsHelper.Layers.Platform,
                            false
                        );
                        _currentState = State.FreeMovement;
                    }

                    break;
            }

            if (!_actionsToStateMap.TryGetValue(_currentState, out UnitAnimations[] actionTypes)
                || actionTypes == null
               )
            {
                Debug.LogError("Missing list of " + nameof(UnitAnimations) + " for state " + _currentState);
            }
            else
            {
                UnitAnimations frameUnitAnimation = UnitAnimations.Idle;
                switch (_currentState)
                {
                    case State.FreeMovement:
                    case State.Climbing:
                        frameUnitAnimation =
                            _activeController.ProcessInput(actionTypes, _inputData, _characterController);
                        if ((frameUnitAnimation & UnitAnimations.Jump) == UnitAnimations.Jump)
                        {
                            frameUnitAnimation &= ~UnitAnimations.Falling;
                        }

                        break;

                    default:
                        Debug.LogError("State " + _currentState + " not implemented");
                        break;
                }

                if (frameUnitAnimation != _lastUnitAnimation)
                {
                    // TODO: Trigger animation here.  We can make animation triggers
                    // names the same as the values in UnitAnimations
                }

                _lastUnitAnimation = frameUnitAnimation;
            }
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

        private void ProcessHorizontalInput()
        {
            switch (_currentState)
            {
                case State.FreeMovement:
                case State.Climbing:
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

        private void ProcessVerticalInput()
        {
            switch (_currentState)
            {
                case State.FreeMovement:
                    if (Input.GetKeyDown(KeyCode.W))
                    {
                        _inputData.SetVertical(1);
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        _inputData.SetVertical(-1);
                    }

                    break;
                case State.Climbing:
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

        private void ProcessObjectInteraction()
        {
            switch (_currentState)
            {
                case State.FreeMovement:
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

        public void AddToInventory(IInventoryItem inventoryItem)
        {
            
        }

        public void GrabClimbObject(IClimbObject climbObject)
        {
            float xDistance = climbObject.WorldPosition.x - transform.position.x;
            _characterController.Move(new Vector3(xDistance, 0, 0));
        }
    }
}