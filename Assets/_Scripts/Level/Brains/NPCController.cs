
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{    
    public class NPCController : BaseController, IAttackerObject
    {
        [SerializeField] private int _attackDamage = 100;
        public int AttackDamage => _attackDamage;
        [SerializeField] private UnitMovement _currentUnitMovement = UnitMovement.Idle;
        
        private NavigationNode _fromNode;
        private NavigationNode _toNode;
        private List<NavigationNode> _path;
        public List<NavigationNode> Path => _path;

        [SerializeField] private Transform _playerTransform = null;
        private Game2D.IPlayerCharacter _player;
        public Game2D.IPlayerCharacter Player => _player;
        
        private int _targetNodeIndex = -1;
        
        [SerializeField] private Vector3 _detectionRange = new Vector3(10f, 5f, 0f);
        [SerializeField] private bool _chasingPlayer = false;

        [SerializeField] private AnimationEventHelper _animationEventHelper;

        private Dictionary<CharacterActionState, IBrainState> _brainStateMap =
            new Dictionary<CharacterActionState, IBrainState>();
        public void Initialize(List<NavigationNode> path, Game2D.IPlayerCharacter player)
        {
            Initialize();
            
            _player = player;
            _playerTransform = _player.GetTransform();
            _path = path;
            
            _animationEventHelper.AddEvent(OnNPCAttackAnimationEvent);
            
            foreach (CharacterStateConfig stateConfig in _stateConfigs)
            {
                _brainStateMap.Add(stateConfig.state, BrainStateBuilder.Build(stateConfig));
            }
        }

        private void OnNPCAttackAnimationEvent(AnimationEvent animationEvent)
        {
            if (_brainStateMap.TryGetValue(CharacterActionState.Attack, out IBrainState brainState)
                && brainState is AttackState attackState
               )
            {
                attackState.ProcessAttackAnimation(this, animationEvent);
            }
        }
        
        #region Monobehavior

        private void Start()
        {
            _brainStateMap.TryGetValue(_currentState, out IBrainState brainState);
            if (brainState?.TryEnterState(this) ?? false)
            {
                Debug.LogError("Failed to enter state " + _currentState);
            }
        }

        private void OnDestroy()
        {
            _animationEventHelper.RemoveEvent(OnNPCAttackAnimationEvent);
        }

        private void Update()
        {
            _inputData.ResetInputs();

            if (_brainStateMap.TryGetValue(_currentState, out IBrainState brainState)
                && brainState != null
               )
            {
                if (brainState.TryExitState(this))
                {
                    foreach (CharacterActionState nextState in brainState.NextState)
                    {
                        if (_brainStateMap.TryGetValue(nextState, out IBrainState nextBrainState)
                            && nextBrainState != null
                            && nextBrainState.TryEnterState(this)
                           )
                        {
                            _currentState = nextState;
                            _brainStateMap.TryGetValue(_currentState, out brainState);
                            break;
                        }
                    }
                }

                brainState?.ProcessInput(this, ref _inputData);
            }

            ProcessState();
        }

        public UnitMovement NavigateToNextNodePath(UnitMovement currentUnitMovement)
        {
            int movementDirection =
                (UnitMovement.MoveRight & currentUnitMovement) == UnitMovement.MoveRight
                    ? 1
                    : (UnitMovement.MoveLeft & currentUnitMovement) == UnitMovement.MoveLeft
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
                return MoveToNextNodeInPath(currentUnitMovement);
            }

            return currentUnitMovement;
        }

        #endregion

        [SerializeField] private float _sqrAttackRange;
        public bool PlayerWithInAttackRange()
        {
            Vector3 playerPosition = _player.GetTransform().position;
            Vector3 npcPosition = this.transform.position;
            float sqrDistance = (playerPosition - npcPosition).sqrMagnitude;
            return sqrDistance <= _sqrAttackRange;
        }

        public bool IsDetectingPlayer()
        {
            if (_player == null || !_player.CanBeDetected())
            {
                return false;
            }
            
            Vector3 playerPosition = _player.GetTransform().position;
                    
            Vector3 npcPosition = this.transform.position;
            Vector3 topCorner = npcPosition + _detectionRange;
            Vector3 bottomCorner = npcPosition - _detectionRange;

            return (
                playerPosition.x <= topCorner.x
                && playerPosition.y <= topCorner.y
                && playerPosition.x >= bottomCorner.x
                && playerPosition.y >= bottomCorner.y
            );
        }
        
        public UnitMovement MoveToNextNodeInPath(UnitMovement currentUnitMovement)
        {   
            if (_targetNodeIndex >= 0 && currentUnitMovement != UnitMovement.Idle)
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
                        return MoveToNextNodeInPath(currentUnitMovement);
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
            Vector3 directionToPlayer = _player.GetTransform().position - this.transform.position;
            float dotProduct = Vector3.Dot(directionToPlayer.normalized, this.transform.forward);
            return dotProduct > 0
                ? UnitMovement.MoveRight
                : UnitMovement.MoveLeft;
        }

        #region IAttackerObject

        public int Damage => _attackDamage;

        #endregion
        
        #region Gizmos
        
        private void OnDrawGizmos()
        {
            Gizmos.color = (_currentState != CharacterActionState.Chase && _currentState != CharacterActionState.Attack)
                ? new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.25f)
                : new Color(Color.red.r, Color.red.g, Color.red.b, 0.25f);
            Gizmos.DrawCube(transform.position, _detectionRange * 2);

            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.5f);
            float radius = Mathf.Sqrt(_sqrAttackRange);
            Gizmos.DrawSphere(transform.position, radius);
        }
        
        #endregion
    }
}