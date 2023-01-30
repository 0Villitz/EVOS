
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            FreeMovement,
            Climbing
        }

        private State _currentState = State.FreeMovement;
        private UnitAnimations _lastUnitAnimation = UnitAnimations.Idle;
        
        private Dictionary<State, IUnitState> _stateToControllerMap;
        private IUnitState _activeController;
        
        private Dictionary<int, IInteractableObject> _interactableObjects = new Dictionary<int, IInteractableObject>();
        
        private InputData _inputData = new InputData();
        private CharacterController _characterController;
        
        #region Monobehavior
        void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            _stateToControllerMap = new Dictionary<State, IUnitState>()
            {
                [State.FreeMovement] = GetComponent<MovementController>() 
            };

            foreach (IUnitState unitState in _stateToControllerMap.Values)
            {
                unitState.Initialize();
            }
            
            _activeController = _stateToControllerMap[_currentState];
        }

        void Update()
        {
            _inputData.ResetInputs();
            
            ProcessHorizontalInput();
            ProcessVerticalInput();
            ProcessObjectInteraction();

            UnitAnimations frameUnitAnimation = UnitAnimations.Idle;
            
            switch (_currentState)
            {
                case State.FreeMovement:
                    frameUnitAnimation = _activeController.ProcessInput(_inputData, _characterController);
                    if ((frameUnitAnimation & UnitAnimations.Jump) == UnitAnimations.Jump)
                    {
                        frameUnitAnimation &= ~UnitAnimations.Falling;
                    }
                    break;
            }
            
            if (frameUnitAnimation != _lastUnitAnimation)
            {
                // TODO: Trigger animation here.  We can make animation triggers
                // names the same as the values in UnitAnimations
            }
            
            _lastUnitAnimation = frameUnitAnimation;
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
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            {
                _inputData.SetHorizontal(0);
            } else if (Input.GetKey(KeyCode.D))
            {
                _inputData.SetHorizontal(1);
            } else if (Input.GetKey(KeyCode.A))
            {
                _inputData.SetHorizontal(-1);
            } 
        }

        private void ProcessVerticalInput()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                _inputData.SetVertical(1);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                _inputData.SetVertical(-1);
            }
        }

        private void ProcessObjectInteraction()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _inputData.EnableInteractionWithEntities();
            }
        }
    }
}