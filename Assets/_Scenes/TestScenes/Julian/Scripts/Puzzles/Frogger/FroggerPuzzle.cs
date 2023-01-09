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
    
#endregion
        
#region Public API
    public override void Init()
    {
        _startingGridPosition = _Grid.WorldToCell(_Player.transform.position);
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
        
        var inputFrame = new FroggerInputFrame
        {
            horizontal = _playerControls.UI.FroggerMoveHorizontal.ReadValue<float>(),
            vertical = _playerControls.UI.FroggerMoveVertical.ReadValue<float>(),
        };
        
        _Player.Tick(inputFrame);
    }
#endregion

#region Private Methods
    private void StartPuzzle()
    {
        _Player.transform.position = _Grid.CellToWorld(_startingGridPosition);
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
#endregion
}
