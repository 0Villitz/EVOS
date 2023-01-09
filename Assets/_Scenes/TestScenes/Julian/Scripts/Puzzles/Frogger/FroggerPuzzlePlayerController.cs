using UnityEngine;

public class FroggerPuzzlePlayerController : MonoBehaviour
{
    public LayerMask _ObstacleLayer;
    public CircleCollider2D  _Collider;

    public float _MoveDurationInSec;
    public float _MoveAmount;
    
    private bool  _isMoving;
    
    private Vector2 _targetPosition;
    private Vector2 _oldPosition;
    private float   _moveLerpValue;
    private float   _startMoveTime;
    
    
    public void Tick(FroggerInputFrame inputFrame)
    {
        HandlePlayerMovement(inputFrame);
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
            transform.position = Vector2.Lerp(_oldPosition, _targetPosition, lerpValue);
        } 
    }
    
    private void HandlePlayerMovement(FroggerInputFrame inputFrame)
    {
        bool hasMovementInput = !(inputFrame.horizontal == 0 && inputFrame.vertical == 0);

        if (_isMoving)
        {
            LerpPlayerPosition();
        }
        else if (hasMovementInput)
        {
            StartPlayerLerp(inputFrame);
        }
    }

    private void StartPlayerLerp(FroggerInputFrame inputFrame)
    {
        _isMoving      = true;
        _oldPosition   = transform.position;
        _startMoveTime = Time.time;
        
        if (inputFrame.horizontal < 0)
        {
            // move left
            _targetPosition = _oldPosition - new Vector2(_MoveAmount, 0);
        }
        else if (inputFrame.horizontal > 0)
        {
            // move right
             _targetPosition = _oldPosition + new Vector2(_MoveAmount, 0);
        }
        else if (inputFrame.vertical < 0)
        {
            // move down
             _targetPosition = _oldPosition - new Vector2(0, _MoveAmount);
        }
        else if (inputFrame.vertical > 0)
        {
            // move up
             _targetPosition = _oldPosition + new Vector2(0, _MoveAmount);
        }
    }
}

