using System;
using UnityEngine;

namespace Puzzles
{
    public class MovablePlatform : MonoBehaviour, IPuzzleInteractable
    {
#region Editor Properties
        
        [Header("Collider - Set here for waypoint preview")]
        public Collider2D collisionCollider;
        [Header("Movement Parameters")]
        public MovementMode movementMode;
        public float waitTime;
        [Range(0, 2)]
        public float easeAmount = 1;
        public float              speed;
        public ObstacleWaypoint[] localWaypoints;

#endregion

#region Private Vars

        private Vector3            _startPosition;
        private float              _startAngle;
        private int                _fromWaypointIndex;
        private float              _percentBetweenWaypoints;
        private float              _nextMoveTime;
        private ObstacleWaypoint[] _globalWayPoints;
        private ObstacleWaypoint[] _activeGlobalWayPoints;
        
#endregion

#region Public API

        public bool IsPaused { get; set; }

        public void Reset(bool shouldBePaused)
        {
            transform.position = _startPosition;
            Vector3 currentRotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, _startAngle);

            _nextMoveTime            = 0;
            _fromWaypointIndex       = 0;
            _percentBetweenWaypoints = 0;

            for (int i = 0; i < _globalWayPoints.Length; ++i)
            {
                _activeGlobalWayPoints[i] = _globalWayPoints[i];
            }

            IsPaused = shouldBePaused;
        }
#endregion

#region Unity API

        private void Awake()
        {
            _startPosition = transform.position;
            _startAngle    = transform.localRotation.eulerAngles.z;


            int localWayPointCount = localWaypoints?.Length > 0 ? localWaypoints.Length : 0;
            _globalWayPoints       = new ObstacleWaypoint[localWayPointCount];
            _activeGlobalWayPoints = new ObstacleWaypoint[localWayPointCount];

            for (int i = 0; i < localWayPointCount; ++i)
            {
                ObstacleWaypoint localWaypoint = localWaypoints[i];
                _globalWayPoints[i] = new ObstacleWaypoint
                {
                    position = (Vector2)_startPosition + localWaypoint.position,
                    angle    = _startAngle + localWaypoint.angle,
                };
            }

            Reset(true);
        }

        private void Update()
        {
            if (movementMode == MovementMode.None || IsPaused)
                return;

            Vector3 oldPosition = transform.position;
            Vector3 oldAngle    = transform.localRotation.eulerAngles;

            if (!HandlePlatformMovement(out Vector2 newPos, out float newAngle))
                return;

            transform.position      = new Vector3(newPos.x, newPos.y, oldPosition.z);
            transform.localRotation = Quaternion.Euler(oldAngle.x, oldAngle.y, newAngle);
        }

        private void OnDrawGizmos()
        {
            if (localWaypoints == null || collisionCollider == null)
            {
                return;
            }

            const float alphaColor = 0.5f;
            const float lineSize   = 0.3f;

            Vector2 currentPosition2D = transform.position;
            Vector3 currentRotation   = transform.localRotation.eulerAngles;

            if (Application.isPlaying && (_globalWayPoints == null || _globalWayPoints.Length != localWaypoints.Length))
            {
                return;
            }
            
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Gizmos.color = Color.red;

                ObstacleWaypoint localWaypoint = localWaypoints[i];

                Vector2 globalPos = Application.isPlaying && _globalWayPoints != null ? _globalWayPoints[i].position : localWaypoint.position + currentPosition2D;
                Gizmos.DrawLine(globalPos - Vector2.up * lineSize, globalPos + Vector2.up * lineSize);
                Gizmos.DrawLine(globalPos - Vector2.left * lineSize, globalPos + Vector2.left * lineSize);


                Gizmos.color = i == 0 ? new Color(0.42f, 0.75f, 0.4f, alphaColor) : new Color(0.2f, 0.2f, 1.0f, alphaColor);

                Vector3    platformSize = collisionCollider.bounds.size;
                Quaternion rotation     = Quaternion.Euler(currentRotation.x, currentRotation.y, localWaypoint.angle);

                // Gizmos.matrix = (transform.localToWorldMatrix * Matrix4x4.Rotate(rotation));
                Gizmos.matrix = Matrix4x4.TRS(globalPos, rotation, Vector3.one); // transform.localToWorldMatrix;
                Gizmos.DrawCube(Vector3.zero, platformSize);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
        
#endregion

#region Private Methods

        private bool HandlePlatformMovement(out Vector2 newPosition, out float newAngle)
        {
            newPosition = Vector3.zero;
            newAngle    = 0;

            float time      = Time.time;
            float deltaTime = Time.deltaTime;

            if (time < _nextMoveTime || _activeGlobalWayPoints.Length == 0)
            {
                return false;
            }

            _fromWaypointIndex %= _activeGlobalWayPoints.Length;

            int              toWaypointIndex              = (_fromWaypointIndex + 1) % _activeGlobalWayPoints.Length;
            ObstacleWaypoint fromWaypoint                 = _activeGlobalWayPoints[_fromWaypointIndex];
            ObstacleWaypoint toWaypoint                   = _activeGlobalWayPoints[toWaypointIndex];
            float            distanceBetweenWaypointPos   = Vector2.Distance(fromWaypoint.position, toWaypoint.position);
            float            distanceBetweenWaypointAngle = Mathf.Abs(fromWaypoint.angle - toWaypoint.angle);

            float validDistance = 0.001f;
            validDistance = distanceBetweenWaypointPos > 0.001f ? distanceBetweenWaypointPos : validDistance;
            validDistance = distanceBetweenWaypointAngle > 0.001f ? distanceBetweenWaypointAngle : validDistance;

            _percentBetweenWaypoints += deltaTime * speed / validDistance;
            _percentBetweenWaypoints =  Mathf.Clamp01(_percentBetweenWaypoints);

            float lerpValue = PuzzleUtils.Ease(_percentBetweenWaypoints, easeAmount);

            newPosition = Vector2.Lerp(fromWaypoint.position, toWaypoint.position, lerpValue);
            newAngle    = Mathf.Lerp(fromWaypoint.angle, toWaypoint.angle, lerpValue);

            if (_percentBetweenWaypoints >= 1)
            {
                _percentBetweenWaypoints = 0;
                _fromWaypointIndex++;

                _nextMoveTime = time + waitTime;

                // We reached the last waypoint
                if (_fromWaypointIndex >= _activeGlobalWayPoints.Length - 1)
                {
                    if (movementMode == MovementMode.Yoyo)
                    {
                        _fromWaypointIndex = 0;
                        Array.Reverse(_activeGlobalWayPoints);
                    }
                    else if (movementMode == MovementMode.Once)
                    {
                        IsPaused = true;
                        return false;
                    }
                }
            }

            return true;
        }
        
        [Serializable]
        public class ObstacleWaypoint
        {
            public Vector2 position;
            public float   angle;
        }
        
        public enum MovementMode
        {
            None,
            Yoyo,
            Cyclic,
            Once,
        }
  #endregion
    }
}
