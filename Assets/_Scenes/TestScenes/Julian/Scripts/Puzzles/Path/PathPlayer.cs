using System;
using UnityEngine;

namespace Puzzles
{
    public class PathPlayer : MonoBehaviour
    {
#region Editor Properties

        public LayerMask        _ObstacleLayer;
        public CircleCollider2D _Collider;

        public float _RotationSpeed;

#endregion

#region Private vars

        private Vector3                     _startPos;
        private Action<IPuzzleInteractable> _onPlayerCollision;

#endregion

#region Public API
        public bool IsAlive;
        
        public void Init(Action<IPuzzleInteractable> onPlayerCollision)
        {
            _startPos          = transform.position;
            _onPlayerCollision = onPlayerCollision;
        }

        public void Reset()
        {
            transform.position = _startPos;
            IsAlive           = true;
        }

        public void Move(Vector2 newPosition)
        {
            if (!IsAlive)
            {
                return;
            }

            Vector2 finalPosition = newPosition;
            Vector2 oldPosition   = transform.position;
            Vector2 delta         = newPosition - oldPosition;

            HandlePlayerRotation(delta);

            if (HandleCollisionDetection(oldPosition, delta, out Vector2 adjustedPosition))
            {
                finalPosition = adjustedPosition;
            }

            transform.position = finalPosition;
        }

#endregion

#region Private Methods

        private void HandlePlayerRotation(Vector2 delta)
        {
            float faceAngle = Mathf.Atan2(delta.y, delta.x);
            if (delta.magnitude > 0.01f)
            {
                float      speedDeltaTime   = Time.deltaTime * _RotationSpeed;
                Quaternion originalRotation = transform.rotation;
                Quaternion targetRotation   = Quaternion.Euler(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, faceAngle * Mathf.Rad2Deg);
                transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, speedDeltaTime);
            }
        }

        private bool HandleCollisionDetection(Vector2 oldPosition, Vector2 deltaMove, out Vector2 adjustedPosition)
        {
            adjustedPosition = Vector2.zero;

            float scaledRadius = _Collider.radius * transform.localScale.x;

            RaycastHit2D hit = Physics2D.CircleCast(
                oldPosition,
                scaledRadius,
                deltaMove.normalized,
                deltaMove.magnitude,
                _ObstacleLayer);

            bool hasCollision = hit.collider != null;
            if (hasCollision)
            {
                // Move the player to the position where it made contact with an obstacle/wall 
                // and offset it by it's radius 
                var interactable = hit.collider.GetComponent<IPuzzleInteractable>();
                if (interactable is PuzzleAudioPlayer)
                {
                    hasCollision = false;
                }
                else
                {
                     adjustedPosition = hit.point + (hit.normal * scaledRadius);
                }
                
                _onPlayerCollision?.Invoke(interactable);
            }

            return hasCollision;
        }

#endregion
    }
}
