using System;
using UnityEngine;

public class FroggerPuzzlePlayerController : MonoBehaviour
{
    public LayerMask _ObstacleLayer;
    public CircleCollider2D  _Collider;

    public float _MoveDurationInSec;
    
    private bool  _isMoving;
    
    private Vector2 _targetPosition;
    private Vector2 _oldPosition;
    private float   _moveLerpValue;
    private float   _startMoveTime;


    private void Update()
    {
        if (_isMoving)
        {
            LerpPlayerPosition();
        }
    }
    
    public void MoveTo(Vector3 targetPosition)
    {
        if (_isMoving)
        {
            return;
        }
    
        _isMoving      = true;
        
        _oldPosition   = transform.position;
        _startMoveTime = Time.time;

        _targetPosition = targetPosition;
    }

    private void LerpPlayerPosition()
    {
        if (!_isMoving)
            return;

        float secondsSinceMovementStarted = Time.time - _startMoveTime;
        float moveDuration = _MoveDurationInSec < 0.0001f ? 0.0001f : _MoveDurationInSec; // Protect against dividing by 0
        float lerpValue = secondsSinceMovementStarted / moveDuration;

        if (secondsSinceMovementStarted >= _MoveDurationInSec)
        {
            transform.position = _targetPosition;
            _isMoving = false;
        }
        else
        {
            float easeLerpValue = Ease(lerpValue, 1.5f);
            transform.position = Vector2.Lerp(_oldPosition, _targetPosition, easeLerpValue);
        }

        Vector2 moveDelta = _targetPosition - _oldPosition;
        HandlePlayerRotation(moveDelta);
    }

    void HandlePlayerRotation(Vector2 delta)
    {
        const float kRotationSpeed = 18.0f;
        
        float       faceAngle      = Mathf.Atan2(delta.y, delta.x);
        if (delta.magnitude > 0.01f)
        {
            float      speedDeltaTime   = Time.deltaTime * kRotationSpeed;
            Quaternion originalRotation = transform.rotation;
            Quaternion targetRotation   = Quaternion.Euler(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, faceAngle * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, speedDeltaTime);
        }
    }

    private float Ease(float x, float ease)
    {
        float a = ease + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}

