
using System.Collections.Generic;
using Game2D.Inventory;
using UnityEngine;

namespace Game2D
{
    public abstract class BaseController : MonoBehaviour, ICharacterController
    {
        [SerializeField] protected CharacterActionState _currentState = CharacterActionState.FreeMovement;
        [SerializeField] private UnitMovement _frameUnitMovement = UnitMovement.Idle;
        
        [SerializeField] protected CharacterStateConfig[] _stateConfigs;
        
        protected Dictionary<CharacterActionState, UnitMovement[]> _actionsToStateMap;
        protected IUnitState _activeState;

        protected InputData _inputData = new InputData();
        protected CharacterController _characterController;
        public CharacterController CharacterController => _characterController;

        protected virtual void Initialize()
        {
            if (_stateConfigs != null && _stateConfigs.Length > 0)
            {
                _actionsToStateMap = new Dictionary<CharacterActionState, UnitMovement[]>();
                foreach (CharacterStateConfig stateConfig in _stateConfigs)
                {
                    _actionsToStateMap.Add(stateConfig.state, stateConfig.actions);
                }
            }
            
            _activeState = GetComponent<ActionController>();
            _activeState.Initialize(_actionsToStateMap);
            
            _characterController = GetComponent<CharacterController>();
            _characterController.enabled = true;
        }

        protected void ProcessState()
        {
            if (!_actionsToStateMap.TryGetValue(_currentState, out UnitMovement[] actionTypes)
                || actionTypes == null
               )
            {
                Debug.LogError("Missing list of " + nameof(UnitMovement) + " for state " + _currentState);
            }
            else
            {
                switch (_currentState)
                {
                    case CharacterActionState.FreeMovement:
                    case CharacterActionState.Climbing:
                    case CharacterActionState.Spawn:
                    case CharacterActionState.Chase:
                    case CharacterActionState.Attack:
                        _frameUnitMovement =
                            _activeState.ProcessInput(actionTypes, _inputData, _characterController);
                        
                        if ((_frameUnitMovement & UnitMovement.Jump) == UnitMovement.Jump)
                        {
                            _frameUnitMovement &= ~UnitMovement.Falling;
                        }
                        break;
                    
                    default:
                        Debug.LogError("State " + _currentState + " not implemented");
                        break;
                }
            }
        }
        
        #region ICharacterController

        public virtual void GrabClimbObject(IClimbObject climbObject)
        {
        }

        public virtual void AddToInventory(IInventoryItem inventoryObject)
        {
        }

        #endregion

    }
}