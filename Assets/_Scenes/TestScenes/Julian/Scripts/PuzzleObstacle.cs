using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PuzzleObstacle : MonoBehaviour
{
    public enum MovementMode
    {
        None,   
        Yoyo,
        Cyclic,
        Once,
    }
    [Header("Collider - Set here for waypoint preview")]
    public Collider2D collisionCollider;
    [Header("Movement Parameters")]
    public MovementMode movementMode;
    public float waitTime;
    [Range(0,2)]
    public float easeAmount = 1;
    public float speed;
    public ObstacleWaypoint[] localWaypoints;
    
    
    private Vector2    _startPosition;
    private float      _startAngle;
    private int        _fromWaypointIndex;
    private float      _percentBetweenWaypoints;
    private float      _nextMoveTime;
    private ObstacleWaypoint[] _globalWayPoints;

#region Unity API

    

#endregion
    private void Awake()
    {
        _startPosition           = transform.position;
        _startAngle              = transform.rotation.eulerAngles.z;
        
        _nextMoveTime            = 0;
        _fromWaypointIndex       = 0;
        _percentBetweenWaypoints = 0;
        
        int localWayPointCount = localWaypoints != null && localWaypoints.Length > 0 ? localWaypoints.Length : 0;
        _globalWayPoints = new ObstacleWaypoint[localWayPointCount];
        
        for (int i = 0; i < localWayPointCount; ++i)
        {
            ObstacleWaypoint localWaypoint = localWaypoints[i];
            _globalWayPoints[i] = new ObstacleWaypoint
            {
                position = _startPosition + localWaypoint.position,
                angle    = _startAngle + localWaypoint.angle,
            };
        }
    }

    private void Update()
    {
        if (movementMode == MovementMode.None)
            return;
        
        Vector2 oldPosition = transform.position;
        Vector3 oldAngle    = transform.localRotation.eulerAngles;

        if (!HandleObstacleMovement(out Vector2 newPos, out float newAngle))
        {
            return;
        }
        
        transform.position               = newPos;
        transform.localRotation = Quaternion.Euler(oldAngle.x, oldAngle.y, newAngle);
    }
    
    private void OnDrawGizmos()
    {
        const float alphaColor = 0.5f;

        Vector2 currentPosition2D =  transform.position;
        Vector3 currentRotation   = transform.rotation.eulerAngles;
        
        if (localWaypoints != null && collisionCollider != null)
        {
            float size = 0.3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Gizmos.color = Color.red;

                ObstacleWaypoint localWaypoint = localWaypoints[i];
                
                Vector2 globalPos = Application.isPlaying && _globalWayPoints != null ? _globalWayPoints[i].position : localWaypoint.position + currentPosition2D;
                Gizmos.DrawLine(globalPos - Vector2.up * size, globalPos + Vector2.up * size);
                Gizmos.DrawLine(globalPos - Vector2.left * size, globalPos + Vector2.left * size);

                
                Gizmos.color = i == 0 ? new Color(0.42f, 0.75f, 0.4f, alphaColor) : new Color(0.2f, 0.2f, 1.0f, alphaColor);
                
                Vector3    platformSize = collisionCollider.bounds.size;
                Quaternion rotation     = Quaternion.Euler(currentRotation.x, currentRotation.y, localWaypoint.angle);

                // Gizmos.matrix = (transform.localToWorldMatrix * Matrix4x4.Rotate(rotation));
                Gizmos.matrix = Matrix4x4.TRS(globalPos, rotation,Vector3.one); // transform.localToWorldMatrix;
                Gizmos.DrawCube(Vector3.zero, platformSize);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }

    private bool HandleObstacleMovement(out Vector2 newPosition, out float newAngle)
    {
        newPosition = Vector3.zero;
        newAngle    = 0;
        
        float time      = Time.time;
        float deltaTime = Time.deltaTime;
        
        if (time < _nextMoveTime || _globalWayPoints.Length == 0)
        {
            return false;
        }

        _fromWaypointIndex %= _globalWayPoints.Length;
        
        int              toWaypointIndex              = (_fromWaypointIndex + 1) % _globalWayPoints.Length;
        ObstacleWaypoint fromWaypoint                 = _globalWayPoints[_fromWaypointIndex];
        ObstacleWaypoint toWaypoint                   = _globalWayPoints[toWaypointIndex];
        float            distanceBetweenWaypointPos   = Vector2.Distance(fromWaypoint.position, toWaypoint.position);
        float            distanceBetweenWaypointAngle = Mathf.Abs(fromWaypoint.angle - toWaypoint.angle);

        float validDistance = 0.001f;
        validDistance            =  distanceBetweenWaypointPos > 0.001f ? distanceBetweenWaypointPos : validDistance;
        validDistance            =  distanceBetweenWaypointAngle > 0.001f ? distanceBetweenWaypointAngle : validDistance;
        
        _percentBetweenWaypoints += deltaTime * speed / validDistance;
        _percentBetweenWaypoints =  Mathf.Clamp01(_percentBetweenWaypoints);
        
        float lerpValue = Ease(_percentBetweenWaypoints, easeAmount);

        newPosition = Vector2.Lerp(fromWaypoint.position, toWaypoint.position, lerpValue);
        newAngle = Mathf.Lerp(fromWaypoint.angle, toWaypoint.angle, lerpValue);
        
        if (_percentBetweenWaypoints >= 1)
        {
            _percentBetweenWaypoints = 0;
            _fromWaypointIndex++;

            _nextMoveTime = time + waitTime;
            if(movementMode == MovementMode.Yoyo && _fromWaypointIndex >= _globalWayPoints.Length - 1)
            {
                _fromWaypointIndex = 0;
                Array.Reverse(_globalWayPoints);
            }
        }

        return true;
    }
    
    private float Ease(float x, float ease)
    {
        float a = ease + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    [Serializable]
    public class ObstacleWaypoint
    {
        public Vector2 position;
        public float   angle;
    }
}
