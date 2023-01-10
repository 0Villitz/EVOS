using System;
using UnityEngine;

public class FroggerPlayer : MonoBehaviour, IPuzzleObstacle
{
    public LayerMask _ObstacleLayer;
    public CircleCollider2D  _Collider;

    public float _MoveDurationInSec;
    
    private bool _isMoving;
    
    private Vector2 _targetPosition;
    private Vector2 _oldPosition;
    private Vector2 _startingPosition;
    private float   _startMoveTime;
    private Grid    _grid;
    
    private Action<IPuzzleObstacle> _onPlayerCollision;

    public  bool IsAlive { get; set; }
    
    public void Init(Grid grid, Action<IPuzzleObstacle> onPlayerCollision)
    {
        _grid              = grid;
        _onPlayerCollision = onPlayerCollision;
        
        Vector3Int gridPosition = grid.WorldToCell(transform.position);
        _startingPosition = grid.GetCellCenterWorld(gridPosition);

        IsAlive = false;
    }
    
    public void Reset()
    {
        transform.position = _startingPosition;
        transform.rotation = Quaternion.identity;
        
        _isMoving          = false;
        _oldPosition       = _startingPosition;
        _targetPosition    = _startingPosition;

        IsAlive = true;
    }
    
    public void Tick(FroggerInputFrame inputFrame)
    {
        if (inputFrame.HasMovementInput())
        {
            Vector3 targetPosition = GetTargetPlayerPosition(inputFrame);
            MoveTo(targetPosition);
        }
    
        if (_isMoving)
        {
            LerpPlayerPosition();
        }
    }
    
        
    private Vector3 GetTargetPlayerPosition(FroggerInputFrame inputFrame)
    {
        Vector3Int currentGridPos = _grid.WorldToCell(transform.position);
        Vector3Int targetCellPos   = currentGridPos;
        
        if (inputFrame.horizontal < 0)
        {
            // move left
            targetCellPos -= new Vector3Int(1, 0, 0);
        }
        else if (inputFrame.horizontal > 0)
        {
            // move right
             targetCellPos += new Vector3Int(1, 0, 0);
        }
        else if (inputFrame.vertical < 0)
        {
            // move down
             targetCellPos -= new Vector3Int(0, 1, 0);
        }
        else if (inputFrame.vertical > 0)
        {
            // move up
             targetCellPos += new Vector3Int(0, 1, 0);
        }

        return _grid.GetCellCenterWorld(targetCellPos);
    }
    
    private void MoveTo(Vector3 targetPosition)
    {
        if (_isMoving || CheckForImpassableWall(targetPosition))
        {
            return;
        }
    
        _isMoving      = true;
        _startMoveTime = Time.time;
        
        _oldPosition   = transform.position;
        _targetPosition = targetPosition;


        bool CheckForImpassableWall(Vector2 targetPos)
        {
            bool hasCollision = CheckCollision(targetPos, out var obstacle);
            return  hasCollision && obstacle is ImpassableObstacle;
        }
    }

    private bool CheckCollision(Vector2 targetPosition, out IPuzzleObstacle obstacle)
    {
        obstacle = null;
        
        Vector2 currentPosition = transform.position;
        Vector2 delta = targetPosition - currentPosition;

        RaycastHit2D hit = Physics2D.CircleCast(
        currentPosition, 
        _Collider.radius, 
        delta.normalized, 
        delta.magnitude, 
        _ObstacleLayer);

        if (hit.collider != null)
        {
            obstacle = hit.collider.GetComponent<IPuzzleObstacle>();
        }

        return obstacle != null;
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
        
        HandlePlayerGoalCollision(transform.position);
    }

    void HandlePlayerRotation(Vector2 delta)
    {
        const float kRotationSpeed = 20.0f;
        
        float faceAngle = Mathf.Atan2(delta.y, delta.x);
        
        if (delta.magnitude > 0.01f)
        {
            float      speedDeltaTime   = Time.deltaTime * kRotationSpeed;
            Quaternion originalRotation = transform.rotation;
            Quaternion targetRotation   = Quaternion.Euler(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, faceAngle * Mathf.Rad2Deg);
            transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, speedDeltaTime);
        }
    }

    private void HandlePlayerGoalCollision(Vector2 testPosition)
    {
        bool hasCollision = CheckCollision(testPosition, out var obstacle);
        if (hasCollision)
        {
            _onPlayerCollision?.Invoke(obstacle);
        }
    }

    private float Ease(float x, float ease)
    {
        float a = ease + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }
}

