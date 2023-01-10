using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FroggerPuzzle : PuzzleBase
{
#region Editor Properties
    public ScriptableEventDispatcher     _GameEventDispatcher;
    public FroggerPlayer _Player;
    
    public Camera     _Camera;
    public Grid       _Grid;
    public GameObject _ButtonGroup;
    public Button     _StartButton;
    public Button     _ExitButton;
#endregion

#region Private vars
    private Controls   _playerControls;
    private List<BytestreamObstacle> _bytestreamList;
#endregion
        
#region Public API
    public override void Init()
    {
        _Player.Init(_Grid, OnPlayerCollision);
        
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

        if (!_Player.IsAlive)
        {
            return;
        }
        
        var inputFrame = new FroggerInputFrame
        {
            horizontal = _playerControls.UI.FroggerMoveHorizontal.ReadValue<float>(),
            vertical = _playerControls.UI.FroggerMoveVertical.ReadValue<float>(),
        };
        
        _Player.Tick(inputFrame);

        foreach (var bytestream in _bytestreamList)
        {
            bytestream.Tick();
        }
    }
#endregion

#region Private Methods
    private void StartPuzzle()
    {
        _Player.Reset();
        _bytestreamList.ForEach(x => x.Reset());
    }
    
    private float GetScaleFactor()
    {
        return WindowCanvas != null ? WindowCanvas.scaleFactor : 1.0f;
    }
    
    private void OnStartButton()
    {
        _ButtonGroup.SetActive(false);
        
        StartPuzzle();
    }
    
    private void OnExitButton()
    {
        _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
    }

    private void OnPlayerCollision(IPuzzleObstacle obstacle)
    {
        _Player.IsAlive = false;
    
        _ButtonGroup.SetActive(true);

        bool wasSuccessful = obstacle is FroggerGoal;
        TriggerPuzzleComplete(wasSuccessful);
    }
#endregion
}
