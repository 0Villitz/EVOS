using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathPuzzlePlayerController : MonoBehaviour
{
    public LayerMask  _obstacleLayer;
    public CircleCollider2D _collider;

    public float _RotationSpeed;
    public float _ActiveDelay;
        
    public event Action<Collider2D> onObstacleHit;

    private Vector3 _startPos;
    private bool    _isAlive;
    private float   _activeTimestamp;

    void Awake()
    {
        _startPos = transform.position;
    }

    public void Init()
    {
        transform.position = _startPos;
        _isAlive           = true;
        _activeTimestamp   = Time.time + _ActiveDelay;
    }
    
    public void Move(Vector2 newPosition)
    {
        if (!_isAlive)
        {
            return;
        }
        
        Vector2 adjustedNewPosition = newPosition;
        Vector2 oldPosition         = transform.position;
        Vector2 delta               = adjustedNewPosition - oldPosition;

        HandlePlayerRotation(delta);
        
        float scaledRadius = _collider.radius * transform.localScale.x;
        
        RaycastHit2D hit = Physics2D.CircleCast(
            oldPosition, 
            scaledRadius, 
            delta.normalized, 
            delta.magnitude, 
            _obstacleLayer);

        
        if (hit.collider != null && Time.time > _activeTimestamp)
        {
            _isAlive = false;
            
            // Move the player to the position where it made contact with an obstical/wall 
            // and offset it by it's radius 
            adjustedNewPosition = hit.point + (hit.normal * scaledRadius);
            onObstacleHit?.Invoke(hit.collider);
        }

        transform.position = adjustedNewPosition;
    }

    void HandlePlayerRotation(Vector2 delta)
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
}
