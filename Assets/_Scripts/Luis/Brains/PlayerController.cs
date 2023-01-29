
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : MonoBehaviour
    {
        private enum State
        {
            FreeMovement,
            Climbing,
            PuzzleSolving
        }

        private State _currentState = State.FreeMovement;
        private UnitAnimations _lastUnitAnimation = UnitAnimations.Idle;
        
        private Dictionary<State, IUnitState> _stateToControllerMap;
        private IUnitState _activeController;
        
        private Dictionary<int, IInteractableObject> _interactableObjects = new Dictionary<int, IInteractableObject>();
        
        private InputData _inputData = new InputData();
        
        #region Monobehavior
        void Awake()
        {
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
            
            ProcessHorizontalInput(ref _inputData);
            ProcessVerticalInput(ref _inputData);
            ProcessObjectInteraction(ref _inputData);
            
            UnitAnimations frameUnitAnimation = _activeController.ProcessInput(_inputData);
            
            if ((frameUnitAnimation & UnitAnimations.Jump) == UnitAnimations.Jump)
            {
                frameUnitAnimation &= ~UnitAnimations.Falling;
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
        
        private void ProcessHorizontalInput(ref InputData inputData)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                inputData.horizontal += 1;
            }

            if (Input.GetKeyUp(KeyCode.D))
            {
                inputData.horizontal -= 1;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                inputData.horizontal += -1;
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                inputData.horizontal -= -1;
            }
        }

        private void ProcessVerticalInput(ref InputData inputData)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                inputData.vertical += 1;
            }
            
            if (Input.GetKeyUp(KeyCode.W))
            {
                inputData.vertical -= 1;
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                inputData.vertical += -1;
            }
            
            if (Input.GetKeyUp(KeyCode.S))
            {
                inputData.vertical -= -1;
            }
        }

        private void ProcessObjectInteraction(ref InputData inputData)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                inputData.EnableInteractionWithEntities();
            }
        }
    }
}