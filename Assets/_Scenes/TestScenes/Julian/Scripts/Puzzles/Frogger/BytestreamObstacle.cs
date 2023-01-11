using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BytestreamObstacle : MonoBehaviour, IPuzzleObstacle
{
    [Header("Asset References")]
    public CapsuleCollider2D _Collider;
    public SpriteRenderer _SpriteRenderer;
    
    [Header("Tuning Parameters")]
    public LayerMask _ObstacleLayer;
    public float         _MoveSpeed;
    public MoveDirection _MoveDirection;
    public Vector2       _Size;
    


    private Action<IPuzzleObstacle> _playerCollisionCallback;
    private Vector2 _startingPosition;
    private Dictionary<Collider2D, IPuzzleObstacle> _obstacleCache = new Dictionary<Collider2D, IPuzzleObstacle>();
    private float _delayStartTime;

#if UNITY_EDITOR
    private void Update()
    {
        _SpriteRenderer.size = _Collider.size = Vector2.Max(Vector2.one * 0.01f, _Size);
    }
#endif

    public void Init(Action<IPuzzleObstacle> onPlayerCollision)
    {
        _startingPosition = transform.position;
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
                _Collider.size, 
                _Collider.direction, 
                0, 
                directionVector, 
                movementDelta, 
                _ObstacleLayer);


            if (hit.collider == null)
                return;
            
            
            if (!_obstacleCache.TryGetValue(hit.collider, out IPuzzleObstacle obstacle))
            {
                obstacle = hit.collider.GetComponent<IPuzzleObstacle>();
                _obstacleCache[hit.collider] = obstacle;
            }

            if (obstacle is FroggerPlayer)
            {
                _playerCollisionCallback?.Invoke(this);
            }
            else if (obstacle is TeleporterObstacle)
            {
                var teleporter = obstacle as TeleporterObstacle;
                teleporter.Warp(transform);
            }
            
        }
    }

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
    
}
