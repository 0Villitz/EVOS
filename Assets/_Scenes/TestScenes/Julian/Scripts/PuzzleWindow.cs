using System;
using UnityEngine;

public class PuzzleWindow : MonoBehaviour
{
    public GameEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary    _PuzzleDictionary;

    public GameObject _BaseView;

    public  int        _MaxSolveAttempts;
    
    private PuzzleBase _activePuzzle;
    private string     _activeTerminalKey;
    private int        _solveAttemptCount;
    private Canvas     _canvas;
    
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        
        _GameEventDispatcher.AddListener(PuzzleEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.AddListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);

        OnHidePuzzleWindow(null);
    }

    private void OnDestroy()
    {
        _GameEventDispatcher.RemoveListener(PuzzleEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.RemoveListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }

    private void OnHidePuzzleWindow(GeneralEvent obj)
    {
        _BaseView.SetActive(false);
        
        if (_activePuzzle != null)
        {
            Destroy(_activePuzzle.gameObject);
        }
    }
    
    private void OnShowPuzzleWindow(GeneralEvent obj)
    {
        _solveAttemptCount = 0;
        _BaseView.SetActive(true);
        
        ShowPuzzleArgs args = obj.data as ShowPuzzleArgs;
        
        _activeTerminalKey = args.TerminalKey;
        _activePuzzle      = CreatePuzzle(args.PuzzleType);
    }

    private PuzzleBase CreatePuzzle(PuzzleType type)
    {
        _activePuzzle      = _PuzzleDictionary.CreateRandomPuzzleOfType(type, null);
        
        // Error handle a null puzzle
        if (_activePuzzle == null)
        {
            Debug.LogError($"Puzzle could not be created for puzzle type:{type}");
            _GameEventDispatcher.DispatchEvent(PuzzleEventType.HidePuzzleWindow);
            return null;
        }

        _activePuzzle.WindowCanvas     =  _canvas;
        _activePuzzle.onPuzzleComplete += OnPuzzleComplete;
        
        return _activePuzzle;
    }
    private void OnPuzzleComplete(bool wasSuccess)
    {
        _solveAttemptCount++;
        
        if (wasSuccess || _solveAttemptCount >= _MaxSolveAttempts)
        {
            // Fire puzzle success event
            _GameEventDispatcher.DispatchEvent(PuzzleEventType.HidePuzzleWindow);
        }
    }
}
