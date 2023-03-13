
using System;
using System.Collections.Generic;
using Game2D.Inventory;
using UnityEngine;

namespace Game2D
{
    public class NPCController : MonoBehaviour, ICharacterController
    {
        private enum State
        {
            None,
            Spawn,
            FreeMovement,
            Chase,
            Attack
        }

        [SerializeField] private State _currentState = State.FreeMovement;
        [SerializeField] private UnitMovement _lastUnitMovement = UnitMovement.Idle;
        [SerializeField] private UnitMovement _currentUnitMovement = UnitMovement.Idle;

        private Dictionary<State, UnitMovement[]> _actionsToStateMap;
        private IUnitState _activeState;

        private Dictionary<int, IInteractableObject> _interactableObjects = new Dictionary<int, IInteractableObject>();

        private InputData _inputData = new InputData();
        private CharacterController _characterController;
        
        private NavigationNode _fromNode;
        private NavigationNode _toNode;
        private List<NavigationNode> _path;
        private Transform _player;
        private int _targetNodeIndex = -1;
        
        [SerializeField] private Vector3 _detectionRange = new Vector3(10f, 5f, 0f);
        [SerializeField] private bool _chasingPlayer = false;

        public void Initialize(List<NavigationNode> path, Transform player)
        {
            _player = player;
            _path = path;
            _characterController = GetComponent<CharacterController>();

            _actionsToStateMap = new Dictionary<State, UnitMovement[]>()
            {
                [State.Spawn] = new[]
                {
                    UnitMovement.Falling
                },
                [State.FreeMovement] = new[]
                {
                    UnitMovement.MoveHorizontal,
                },
                [State.Chase] = new[]
                {
                    UnitMovement.MoveHorizontal,
                },
            };

            _activeState = GetComponent<ActionController>();
            _activeState.Initialize();

            _lastUnitMovement = UnitMovement.Idle;
            _currentUnitMovement = UnitMovement.Idle;
        }
        
        #region Monobehavior
        
        void Update()
        {
            _inputData.ResetInputs();

            ProcessHorizontalInput();
            ProcessVerticalInput();
            ProcessObjectInteraction();

            switch (_currentState)
            {
                case State.Spawn:
                    if (_characterController.isGrounded)
                    {
                        _currentUnitMovement = MoveToNextNodeInPath();
                        _currentState = State.FreeMovement;
                    }
                    break;
                    
                case State.FreeMovement:
                    if (_chasingPlayer)
                    {
                        _targetNodeIndex = -1;
                        _currentState = State.Chase;
                    }
                    else
                    {
                        int movementDirection =
                            (UnitMovement.MoveRight & _currentUnitMovement) == UnitMovement.MoveRight
                                ? 1
                                : (UnitMovement.MoveLeft & _currentUnitMovement) == UnitMovement.MoveLeft
                                    ? -1
                                    : 0;

                        NavigationNode targetNode = _path[_targetNodeIndex];
                        Vector3 targetDirection = targetNode.transform.position - this.transform.position;
                        float dotProd = Vector3.Dot(targetDirection.normalized, this.transform.forward);
                        if ((movementDirection == 1 && dotProd < 0)
                            || (movementDirection == -1 && dotProd > 0)
                           )
                        {
                            this.transform.position = new Vector3(
                                targetNode.transform.position.x,
                                this.transform.position.y,
                                targetNode.transform.position.z
                            );
                            _currentUnitMovement = MoveToNextNodeInPath();
                        }
                    }
                    break;
                
                case State.Chase:
                    if (!_chasingPlayer)
                    {
                        _currentUnitMovement = MoveToNextNodeInPath();
                        _currentState = State.FreeMovement;
                    }
                    else
                    {
                        _currentUnitMovement = GetMovementToPlayer();
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
                    case State.Spawn:
                    case State.Chase:
                        frameUnitMovement =
                            _activeState.ProcessInput(actionTypes, _inputData, _characterController);

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
                case State.Spawn:
                    _inputData.SetHorizontal(0);
                    break;
                
                case State.FreeMovement:
                case State.Chase:
                    int horizontalInput = (_currentUnitMovement & UnitMovement.MoveRight) == UnitMovement.MoveRight
                        ? 1
                        : (_currentUnitMovement & UnitMovement.MoveLeft) == UnitMovement.MoveLeft
                            ? -1
                            : 0;
                    _inputData.SetHorizontal(horizontalInput);
                    break;
            }
        }

        private void ProcessVerticalInput()
        {
            switch (_currentState)
            {
                case State.Spawn:
                    _inputData.SetVertical(0);
                    break;
                
                case State.FreeMovement:
                    break;
            }
        }
        
        private void ProcessObjectInteraction()
        {
            switch (_currentState)
            {
                case State.Spawn:
                    _chasingPlayer = false;
                    break;

                case State.FreeMovement:
                case State.Chase:
                    playerPosition = _player.position;
                    
                    Vector3 npcPosition = this.transform.position;
                    topCorner = npcPosition + _detectionRange;
                    bottomCorner = npcPosition - _detectionRange;
                    _chasingPlayer = (
                        playerPosition.x <= topCorner.x
                        && playerPosition.y <= topCorner.y
                        && playerPosition.x >= bottomCorner.x
                        && playerPosition.y >= bottomCorner.y
                    );
                    break;
            }
        }

        public Vector3 playerPosition;
        public Vector3 topCorner;
        public Vector3 bottomCorner;
        public void AddToInventory(IInventoryItem inventoryItem)
        {
            
        }

        public void GrabClimbObject(IClimbObject climbObject)
        {
            
        }

        
        private UnitMovement MoveToNextNodeInPath()
        {   
            if (_targetNodeIndex >= 0)
            {
                int nextNodeIndex = (_targetNodeIndex == _path.Count - 1)
                    ? 0
                    : _targetNodeIndex + 1;
                
                NavigationNode targetNode = _path[_targetNodeIndex];
                NavigationNode nextTargetNode = _path[nextNodeIndex];
                foreach (NavigationConnectionData connection in targetNode.connections)
                {
                    if (connection.node.id == nextTargetNode.id)
                    {
                        _targetNodeIndex = nextNodeIndex;
                        return connection.action;
                    }
                }
            }
            
            float smallestSqrDistance = float.MaxValue;
            for (int idx = 0; idx < _path.Count; idx++)
            {
                NavigationNode node = _path[idx];
                float sqrDistance = (node.transform.position - this.transform.position).sqrMagnitude;
                if (sqrDistance < smallestSqrDistance)
                {
                    smallestSqrDistance = sqrDistance;
                    _targetNodeIndex = idx;
                    if (smallestSqrDistance <= float.Epsilon)
                    {
                        return MoveToNextNodeInPath();
                    }
                }
            }

            Vector3 nextNodeDirection = _path[_targetNodeIndex].transform.position - this.transform.position;
            float dotProduct = Vector3.Dot(nextNodeDirection.normalized, transform.forward);
            return dotProduct >= 0
                ? UnitMovement.MoveRight
                : UnitMovement.MoveLeft;
        }

        private UnitMovement GetMovementToPlayer()
        {
            Vector3 directionToPlayer = _player.transform.position - this.transform.position;
            float dotProduct = Vector3.Dot(directionToPlayer.normalized, this.transform.forward);
            return dotProduct > 0
                ? UnitMovement.MoveRight
                : UnitMovement.MoveLeft;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.25f);
            Gizmos.DrawCube(transform.position, _detectionRange * 2);
        }
    }
}