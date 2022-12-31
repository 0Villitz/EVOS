using UnityEngine;
using UnityEngine.InputSystem;

public class PathPuzzle : MonoBehaviour
{
    public GameEventDispatcher        _GameEventDispatcher;
    public Camera                     _Camera; 
    public PathPuzzlePlayerController _Player;


    private Controls _playerControls;

#region Public API
    public void StartPuzzle()
    {
        _Player.Init();
        
        Vector3 playerPos = _Player.transform.position;
        playerPos.z = _Camera.transform.position.z;
        
        Vector2 screenPoint = _Camera.WorldToScreenPoint(playerPos);
        Mouse.current.WarpCursorPosition(screenPoint);
    }
#endregion

#region Unity Methods
    private void Awake()
    {
        _playerControls = new Controls();
        _Player.onObstacleHit += OnObstacleHit;
    }
    
    private void Update()
    {
        if (Keyboard.current.rKey.isPressed)
        {
            StartPuzzle();
        }
    
        Vector2 rawPointDelta = _playerControls.UI.PuzzlePoint.ReadValue<Vector2>();
        Vector2 rawMousePointPosition = Mouse.current.position.ReadValue();

        Vector3 screenMousePos = rawMousePointPosition;
        Vector3 cameraPosition = _Camera.transform.position;
        
        screenMousePos.z = Mathf.Abs(cameraPosition.z);
        Vector2 worldPos = _Camera.ScreenToWorldPoint(screenMousePos);
        
        Debug.DrawLine(cameraPosition, worldPos, Color.blue);
        
        _Player.Move(worldPos);
    }
#endregion

    private void OnObstacleHit(Collider2D collider)
    {
        Debug.Log($"Obstacle Hit:{collider.name}");
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.ObstacleHit);
    }
}
