using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FroggerPuzzle : PuzzleBase
{
#region Editor Properties
    public ScriptableEventDispatcher     _GameEventDispatcher;
    public FroggerPuzzlePlayerController _Player;
    
    public Camera     _Camera;
    public Grid       _Grid;
    public GameObject _ButtonGroup;
    public Button     _StartButton;
    public Button     _ExitButton;
#endregion

#region Private vars
    private Controls   _playerControls;
    private Vector3Int _playerGridPosition;
    private Vector3Int _startingGridPosition;
    private bool       _isPaused;
    
    private List<BytestreamObstacle> _bytestreamList;
#endregion
        
#region Public API
    public override void Init()
    {
        _startingGridPosition = _Grid.WorldToCell(_Player.transform.position);
        
        _bytestreamList = GetComponentsInChildren<BytestreamObstacle>().ToList();
        _bytestreamList.ForEach(x => x.Init(OnPlayerCollision));
    }
#endregion

#region Unity API
    private void Awake()
    {
        _playerControls = new Controls();
        _playerControls.UI.Enable();
        
        _StartButton.onClick.AddListener(OnStartButton);
        _ExitButton.onClick.AddListener(OnExitButton);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnExitButton();
        }

        if (_isPaused)
        {
            return;
        }
        
        var inputFrame = new FroggerInputFrame
        {
            horizontal = _playerControls.UI.FroggerMoveHorizontal.ReadValue<float>(),
            vertical = _playerControls.UI.FroggerMoveVertical.ReadValue<float>(),
        };

        if (inputFrame.HasMovementInput())
        {
            Vector3 targetPosition = GetTargetPlayerPosition(inputFrame);
            _Player.MoveTo(targetPosition);
        }
        
        _Player.Tick();

        foreach (var bytestream in _bytestreamList)
        {
            bytestream.Tick();
        }
    }
#endregion

#region Private Methods
    private void StartPuzzle()
    {
        _Player.transform.position = _Grid.GetCellCenterWorld(_startingGridPosition);
        
        _bytestreamList.ForEach(x => x.Reset());
        
        _isPaused = false;
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
    
    private Vector3 GetTargetPlayerPosition(FroggerInputFrame inputFrame)
    {
        Vector3Int currentGridPos = _Grid.WorldToCell(_Player.transform.position);
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

        return _Grid.GetCellCenterWorld(targetCellPos);
    }

    private void OnPlayerCollision(BytestreamObstacle bytestream)
    {
        _isPaused = true;
    }
#endregion
}
