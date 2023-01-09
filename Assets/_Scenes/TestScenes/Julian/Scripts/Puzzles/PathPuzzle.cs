using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PathPuzzle : PuzzleBase
{
#region Editor Properties
    public ScriptableEventDispatcher  _GameEventDispatcher;
    public Camera                     _Camera; 
    public PathPuzzlePlayerController _Player;
    
    public Button        _StartButton;
    public Button        _ExitButton;
    public Transform     _ButtonGroup;
#endregion

#region Private vars
    private Vector2  _startOffset;
    private List<MovablePuzzleObstacle> _movableObstacleList;
#endregion
        
#region Public API
    public override void Init()
    {
        
    }
#endregion

#region Unity API
    private void Awake()
    {
        _Player.onObstacleHit += OnObstacleHit;

        _StartButton.onClick.AddListener(OnStartButton);
        _ExitButton.onClick.AddListener(OnExitButton);

        _movableObstacleList = GetComponentsInChildren<MovablePuzzleObstacle>().ToList();
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartPuzzle();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnExitButton();
        }
    
        Vector2 worldMousePos = (Vector2)GetMouseWorldPoint() - _startOffset;
        _Player.Move(worldMousePos);
        
        // For Debugging
        Debug.DrawLine( _Camera.transform.position, worldMousePos, Color.blue);
    }
#endregion

#region Private Methods
    private void StartPuzzle()
    {
        _movableObstacleList.ForEach(x => x.Reset(false));
        
        _Player.Init();
        
        Vector3 playerPos = _Player.transform.position;
        playerPos.z = _Camera.transform.position.z;

        Vector2 worldMousePos = GetMouseWorldPoint();

        _startOffset = worldMousePos - (Vector2)playerPos;
        Cursor.visible = false; 
    }
    
    private Vector3 GetMouseWorldPoint()
    {
        Vector2 rawMousePointPosition = Mouse.current.position.ReadValue();
        Vector3 screenMousePos = rawMousePointPosition / GetScaleFactor();
        Vector3 cameraPosition = _Camera.transform.position;
        
        // Set the z value so when we use 'ScreenToWorldPoint',
        // that position is projected out into 3d space away from the camera
        screenMousePos.z = Mathf.Abs(cameraPosition.z);
        
        Vector2 worldMousePos = (Vector2)_Camera.ScreenToWorldPoint(screenMousePos);
        return worldMousePos;
    }

    private float GetScaleFactor()
    {
        return WindowCanvas != null ? WindowCanvas.scaleFactor : 1.0f;
    }
    
    private void OnStartButton()
    {
        _ButtonGroup.gameObject.SetActive(false);
        
        StartPuzzle();
    }
    
    private void OnExitButton()
    {
        _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
    }
    
    private void OnObstacleHit(Collider2D collider)
    {
        _movableObstacleList.ForEach(x => x.IsPaused = true);
        
        _ButtonGroup.gameObject.SetActive(true);
        Cursor.visible = true;
        
        IPuzzleObstacle obstacleHit = collider.GetComponent<IPuzzleObstacle>();
        bool wasPuzzleSuccess = obstacleHit is PathPuzzleGoal;
        
        TriggerPuzzleComplete(wasPuzzleSuccess);
    }
#endregion
}
