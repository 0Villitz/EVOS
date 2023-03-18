using System;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    [ExecuteInEditMode]
    public class Bytestream : MonoBehaviour, IPuzzleInteractable
    {
#region Editor Properties
    
        [Header("Asset References")]
        public CapsuleCollider2D _Collider;
        public SpriteRenderer _SpriteRenderer;

        [Header("Tuning Parameters")]
        public LayerMask _ObstacleLayer;
        public float         _MoveSpeed;
        public MoveDirection _MoveDirection;
        public Vector2       _Size;

#endregion

#region Private Vars


        private Action<IPuzzleInteractable> _playerCollisionCallback;
        private Vector2                     _startingPosition;
        private float                       _delayStartTime;
        
        private Dictionary<Collider2D, IPuzzleInteractable> _obstacleCache = new Dictionary<Collider2D, IPuzzleInteractable>();

#endregion

#region Unity API

#if UNITY_EDITOR
        private void Update()
        {
            _SpriteRenderer.size = _Collider.size = Vector2.Max(Vector2.one * 0.01f, _Size);
        }
#endif

#endregion


#region Public API

        public void Init(Action<IPuzzleInteractable> onPlayerCollision)
        {
            _startingPosition        = transform.position;
            _playerCollisionCallback = onPlayerCollision;

            _SpriteRenderer.size = _Collider.size = _Size;
        }

        public void Reset()
        {
            transform.position = _startingPosition;
            _delayStartTime    = Time.time + 0.1f;
        }

        public void Tick()
        {
            if (Time.time < _delayStartTime)
                return;

            Vector2 currentPosition = transform.position;

            Vector2 directionVector = GetDirectionVector();
            float   movementDelta   = _MoveSpeed * Time.deltaTime;
            Vector2 targetPosition  = currentPosition + (directionVector * movementDelta);

            transform.position = targetPosition;

            HandleCollisionDetection();


            void HandleCollisionDetection()
            {
                Debug.DrawRay(currentPosition, targetPosition - currentPosition, Color.green);

                RaycastHit2D hit = Physics2D.CapsuleCast(
                    currentPosition,
                    _Collider.size * transform.localScale,
                    _Collider.direction,
                    0,
                    directionVector,
                    movementDelta,
                    _ObstacleLayer);


                if (hit.collider == null)
                    return;


                if (!_obstacleCache.TryGetValue(hit.collider, out IPuzzleInteractable obstacle))
                {
                    obstacle                     = hit.collider.GetComponent<IPuzzleInteractable>();
                    _obstacleCache[hit.collider] = obstacle;
                }

                if (obstacle is FroggerPlayer)
                {
                    _playerCollisionCallback?.Invoke(this);
                }
                else if (obstacle is Teleporter)
                {
                    var teleporter = obstacle as Teleporter;
                    teleporter.Warp(transform);
                }

            }
        }
#endregion

#region Private Methods

        private Vector2 GetDirectionVector()
        {
            switch (_MoveDirection)
            {
                case MoveDirection.Down: return new Vector2(0, -1);
                case MoveDirection.Up:   return new Vector2(0, 1);
            }

            return Vector2.zero;
        }


        [Serializable]
        public enum MoveDirection
        {
            None,
            Up,
            Down,
        }
        
#endregion
    }
}
