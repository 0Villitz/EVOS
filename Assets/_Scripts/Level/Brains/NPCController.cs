
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    public class NPCController : BaseController, IAttackerObject
    {
        [SerializeField] private int _attackDamage = 100;
        [SerializeField] private UnitMovement _currentUnitMovement = UnitMovement.Idle;
        
        private NavigationNode _fromNode;
        private NavigationNode _toNode;
        private List<NavigationNode> _path;
        private Game2D.IPlayerCharacter _player;
        private int _targetNodeIndex = -1;
        
        [SerializeField] private Vector3 _detectionRange = new Vector3(10f, 5f, 0f);
        [SerializeField] private bool _chasingPlayer = false;

        [SerializeField] private AnimationEventHelper _animationEventHelper;

        public void Initialize(List<NavigationNode> path, Game2D.IPlayerCharacter player)
        {
            _player = player;
            _path = path;
            
            Initialize();
        }

        private void OnAnimationEvent(AnimationEvent animationEvent)
        {
            switch (_currentState)
            {
                case CharacterActionState.Attack:
                    if (animationEvent.stringParameter == AnimationEventKey.Attack)
                    {
                        _player.TakeDamage(_attackDamage, this);
                    }
                    else if (animationEvent.stringParameter == AnimationEventKey.End)
                    {
                        _inputData.RemoveInteractableEntity(_player);
                        _chasingPlayer = false;
                    }
                    break;
            }    
        }
        
        #region Monobehavior

        private void OnDestroy()
        {
            _animationEventHelper.RemoveEvent(OnAnimationEvent);
        }

        private void Update()
        {
            _inputData.ResetInputs();

            ProcessHorizontalInput();
            ProcessVerticalInput();
            ProcessObjectInteraction();

            switch (_currentState)
            {
                case CharacterActionState.Spawn:
                    if (_characterController.isGrounded)
                    {
                        _currentUnitMovement = MoveToNextNodeInPath();
                        _currentState = CharacterActionState.FreeMovement;
                    }

                    break;

                case CharacterActionState.FreeMovement:
                    if (_chasingPlayer)
                    {
                        _targetNodeIndex = -1;
                        _currentState = CharacterActionState.Chase;
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

                case CharacterActionState.Chase:
                    if (_player != null && _player.GetHealth() > 0)
                    {
                        if (_chasingPlayer && _inputData.interactWithEntities)
                        {
                            _currentUnitMovement = GetMovementToPlayer();
                            _currentState = CharacterActionState.Attack;
                        }
                        else if (!_chasingPlayer)
                        {
                            _currentUnitMovement = MoveToNextNodeInPath();
                            _currentState = CharacterActionState.FreeMovement;
                        }
                        else
                        {
                            _currentUnitMovement = GetMovementToPlayer();
                        }
                    }
                    else
                    {
                        _chasingPlayer = false;
                        _currentUnitMovement = MoveToNextNodeInPath();
                        _currentState = CharacterActionState.FreeMovement;
                        _inputData.RemoveInteractableEntity(_player);
                    }

                    break;

                case CharacterActionState.Attack:
                    if (_player != null && _player.GetHealth() > 0)
                    {
                        if (!_chasingPlayer)
                        {
                            _currentUnitMovement = MoveToNextNodeInPath();
                            _currentState = CharacterActionState.FreeMovement;
                        }
                    }
                    else
                    {
                        _chasingPlayer = false;
                        _currentUnitMovement = MoveToNextNodeInPath();
                        _currentState = CharacterActionState.FreeMovement;
                        _inputData.RemoveInteractableEntity(_player);
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

        protected void OnTriggerExit(Collider other)
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
                case CharacterActionState.Spawn:
                    _inputData.SetHorizontal(0);
                    break;
                
                case CharacterActionState.FreeMovement:
                case CharacterActionState.Chase:
                case CharacterActionState.Attack:
                    int horizontalInput = (_currentUnitMovement & UnitMovement.MoveRight) == UnitMovement.MoveRight
                        ? 1
                        : (_currentUnitMovement & UnitMovement.MoveLeft) == UnitMovement.MoveLeft
                            ? -1
                            : 0;
                    _inputData.SetHorizontal(horizontalInput);
                    break;
            }
        }

        protected override void ProcessVerticalInput()
        {
            switch (_currentState)
            {
                case CharacterActionState.Spawn:
                    _inputData.SetVertical(0);
                    break;
                
                case CharacterActionState.FreeMovement:
                    break;
            }
        }
        
        protected override void ProcessObjectInteraction()
        {
            switch (_currentState)
            {
                case CharacterActionState.Spawn:
                    _chasingPlayer = false;
                    break;

                case CharacterActionState.FreeMovement:
                    if (_player != null && _player.GetHealth() > 0)
                    {
                        _chasingPlayer = IsDetectingPlayer();
                    }

                    break;
                    
                case CharacterActionState.Chase:
                    if (PlayerWithInAttackRange())
                    {
                        _inputData.EnableInteractionWithEntities();
                        _inputData.AddInteractableEntity(_player);
                    }
                    else
                    {
                        _chasingPlayer = IsDetectingPlayer();
                    }
                    break;
            }
        }

        [SerializeField] private float _sqrAttackRange;
        private bool PlayerWithInAttackRange()
        {
            Vector3 playerPosition = _player.GetTransform().position;
            Vector3 npcPosition = this.transform.position;
            float sqrDistance = (playerPosition - npcPosition).sqrMagnitude;
            return sqrDistance <= _sqrAttackRange;
        }

        private bool IsDetectingPlayer()
        {
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
            Vector3 directionToPlayer = _player.GetTransform().position - this.transform.position;
            float dotProduct = Vector3.Dot(directionToPlayer.normalized, this.transform.forward);
            return dotProduct > 0
                ? UnitMovement.MoveRight
                : UnitMovement.MoveLeft;
        }

        #region IAttackerObject
        
        public Transform GetTransform()
        {
            return this.transform;
        }

        public void ProcessAttack()
        {
            if (_player.GetHealth() <= 0)
            {
                _inputData.RemoveInteractableEntity(_player);
                _player = null;
            }
        }
        
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