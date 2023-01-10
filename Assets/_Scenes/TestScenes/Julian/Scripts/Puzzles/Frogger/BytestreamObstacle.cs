using System;
using UnityEngine;

public class BytestreamObstacle : MonoBehaviour, IPuzzleObstacle
{
    [Header("Asset References")]
    public CapsuleCollider2D _Collider;

    [Header("Tuning Parameters")]
    public LayerMask _PlayerLayer;
    public float _MoveSpeed;
    public MoveDirection _MoveDirection;


    private Action<BytestreamObstacle> _playerCollisionCallback;
    private Vector2 _startingPosition;

    public void Init(Action<BytestreamObstacle> onPlayerCollision)
    {
        _startingPosition = transform.position;
        _playerCollisionCallback = onPlayerCollision;
    }

    public void Reset()
    {
        transform.position = _startingPosition;
    }

    public void Tick()
    {
        Vector2 currentPosition = transform.position;

        Vector2 directionVector = GetDirectionVector();
        float   movementDelta   = _MoveSpeed * Time.deltaTime;
        Vector2 targetPosition  = currentPosition + (directionVector * movementDelta);

        transform.position = targetPosition;

        HandleCollisionDetection();


        void HandleCollisionDetection()
        {
            RaycastHit2D hit = Physics2D.CapsuleCast(
                currentPosition, 
                _Collider.size, 
                _Collider.direction, 
                0, 
                directionVector, 
                movementDelta, 
                _PlayerLayer);

            if (hit.collider != null)
            {
                _playerCollisionCallback?.Invoke(this);
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
