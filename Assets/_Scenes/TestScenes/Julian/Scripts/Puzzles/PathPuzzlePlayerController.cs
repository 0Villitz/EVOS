using System;
using UnityEngine;

public class PathPuzzlePlayerController : MonoBehaviour
{
#region Editor Properties
    public LayerMask  _ObstacleLayer;
    public CircleCollider2D _Collider;

    public float _RotationSpeed;
        
    public event Action<Collider2D> onObstacleHit;
#endregion

#region Private vars
    private Vector3 _startPos;
    private bool    _isAlive;
#endregion

#region Unity API
    void Awake()
    {
        _startPos = transform.position;
    }
#endregion

#region Public API
    public void Init()
    {
        transform.position = _startPos;
        _isAlive           = true;
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
        
        float scaledRadius = _Collider.radius * transform.localScale.x;
        
        RaycastHit2D hit = Physics2D.CircleCast(
            oldPosition, 
            scaledRadius, 
            delta.normalized, 
            delta.magnitude, 
            _ObstacleLayer);

        
        if (hit.collider != null)
        {
            _isAlive = false;
            
            // Move the player to the position where it made contact with an obstacle/wall 
            // and offset it by it's radius 
            adjustedNewPosition = hit.point + (hit.normal * scaledRadius);
            onObstacleHit?.Invoke(hit.collider);
        }

        transform.position = adjustedNewPosition;
    }
#endregion

#region Private Methods
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
#endregion
}
