using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PathPuzzle : MonoBehaviour, IPuzzle
{
    public GameEventDispatcher        _GameEventDispatcher;
    public Camera                     _Camera; 
    public PathPuzzlePlayerController _Player;
    
    public Button        _StartButton;
    public Button        _ExitButton;
    public Transform     _ButtonGroup;


    private Controls _playerControls;

#region Public API
    public void StartPuzzle()
    {
        _Player.Init();
        
        Vector3 playerPos = _Player.transform.position;
        playerPos.z = _Camera.transform.position.z;
        
        Vector2 screenPoint = _Camera.WorldToScreenPoint(playerPos);
        Mouse.current.WarpCursorPosition(screenPoint);
        Cursor.visible = false;
    }
#endregion

#region Unity Methods
    private void Awake()
    {
        _playerControls = new Controls();
        _Player.onObstacleHit += OnObstacleHit;

        _StartButton.onClick.AddListener(OnStartButton);
        _ExitButton.onClick.AddListener(OnExitButton);
        
        _GameEventDispatcher.AddListener(PuzzleEventType.Start, OnPuzzleStart);
    }

    private void OnDestroy()
    {
        _GameEventDispatcher.RemoveListener(PuzzleEventType.Start, OnPuzzleStart);
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
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

    private void OnStartButton()
    {
        _ButtonGroup.gameObject.SetActive(false);
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.Start);
    }
    
    private void OnExitButton()
    {
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.End);
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.HidePuzzleWindow);
    }
    
    private void OnObstacleHit(Collider2D collider)
    {
        Debug.Log($"Obstacle Hit:{collider.name}");
        _ButtonGroup.gameObject.SetActive(true);
        
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.ObstacleHit);
        Cursor.visible = true;
    }
    
    private void OnPuzzleStart(GeneralEvent obj)
    {
        StartPuzzle();
    }
}
