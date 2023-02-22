
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

        private UnitMovement _lastUnitMovement = UnitMovement.Idle;

        private Dictionary<State, UnitMovement[]> _actionsToStateMap;
        private IUnitState _activeState;

        private Dictionary<int, IInteractableObject> _interactableObjects = new Dictionary<int, IInteractableObject>();

        private InputData _inputData = new InputData();
        private CharacterController _characterController;

        #region Monobehavior

        void Awake()
        {
            _characterController = GetComponent<CharacterController>();

            _actionsToStateMap = new Dictionary<State, UnitMovement[]>()
            {
                [State.FreeMovement] = new[]
                {
                    UnitMovement.MoveHorizontal,
                    UnitMovement.Jump,
                    UnitMovement.Falling
                },
                [State.Climbing] = new[]
                {
                    UnitMovement.MoveHorizontal,
                    UnitMovement.Climb,
                }
            };

            _activeState = GetComponent<ActionController>();
            _activeState.Initialize();
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

            if (!_actionsToStateMap.TryGetValue(_currentState, out UnitMovement[] actionTypes)
                || actionTypes == null
               )
            {
                Debug.LogError("Missing list of " + nameof(UnitMovement) + " for state " + _currentState);
            }
            else
            {
                UnitMovement frameUnitMovement = UnitMovement.Idle;
                switch (_currentState)
                {
                    case State.FreeMovement:
                    case State.Climbing:
                        frameUnitMovement =
                            _activeState.ProcessInput(actionTypes, _inputData, _characterController);
                        if ((frameUnitMovement & UnitMovement.Jump) == UnitMovement.Jump)
                        {
                            frameUnitMovement &= ~UnitMovement.Falling;
                        }

                        break;

                    default:
                        Debug.LogError("State " + _currentState + " not implemented");
                        break;
                }

                if (frameUnitMovement != _lastUnitMovement)
                {
                    // TODO: Trigger animation here.  We can make animation triggers
                    // names the same as the values in UnitMovement
                }

                _lastUnitMovement = frameUnitMovement;
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