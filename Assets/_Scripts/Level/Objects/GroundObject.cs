
using UnityEngine;

public class GroundObject : MonoBehaviour
{
    [SerializeField] private float _maxPlayerDistance = 0f;
    public float MaxPlayerDistance => _maxPlayerDistance;
}
