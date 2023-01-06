using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWindow : MonoBehaviour
{
#region Editor Properties
    public ScriptableEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary    _PuzzleDictionary;

    public GameObject _BaseView;

    public int _MaxSolveAttempts;
#endregion

#region Private vars
    private PuzzleBase _activePuzzle;
    private PuzzleType _activePuzzleType;
    private string     _activeTriggerKey;
    private Stack<int> _levelIndexStack;
    private int        _solveAttemptCount;
    private Canvas     _canvas;
#endregion

#region Unity API
    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        
        _GameEventDispatcher.AddListener(GameEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.AddListener(GameEventType.HidePuzzleWindow, OnHidePuzzleWindow);

        OnHidePuzzleWindow(null);
    }

    private void OnDestroy()
    {
        _GameEventDispatcher.RemoveListener(GameEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.RemoveListener(GameEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }
#endregion

#region Private Methods
    private void OnHidePuzzleWindow(GeneralEvent obj)
    {
        _BaseView.SetActive(false);

        DestroyPuzzle(_activePuzzle);
        
        _activePuzzle = null;

        _levelIndexStack?.Clear();
    }
    
    private void OnShowPuzzleWindow(GeneralEvent obj)
    {
        if (_activePuzzle != null)
        {
            Debug.LogWarning("Puzzle already active");
            return;
        }
        
        _BaseView.SetActive(true);
        
        _solveAttemptCount = 0;

        ShowPuzzleArgs args = (ShowPuzzleArgs)obj.data;
        _activeTriggerKey = args.TriggerKey;
        _activePuzzleType = args.PuzzleType;
        
        _levelIndexStack  = CreateLevelIndexStack(args);

        GotoNextLevel();
    }

    private Stack<int> CreateLevelIndexStack(ShowPuzzleArgs args)
    {
        args.SpecificLevelList?.Reverse();
        var levelList = args.SpecificLevelList ?? _PuzzleDictionary.CreateRandomIndexListOfType(args.PuzzleType, args.RandomLevelCount);
        
        return new Stack<int>(levelList);
    }

    private PuzzleBase CreatePuzzle(PuzzleType type, int levelIndex)
    {
        var newPuzzle = _PuzzleDictionary.CreatePuzzleOfType(type, levelIndex, null);
        
        // Error handle a null puzzle
        if (newPuzzle == null)
        {
            Debug.LogError($"Puzzle could not be created for puzzle type:{type}");
            _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
            return null;
        }

        newPuzzle.WindowCanvas     =  _canvas;
        newPuzzle.onPuzzleComplete += OnPuzzleCompleted;
        newPuzzle.Init();
        
        return newPuzzle;
    }

    private void DestroyPuzzle(PuzzleBase puzzle)
    {
        if (puzzle != null)
        {
            Destroy(puzzle.gameObject);
        }
    }
    
    private void OnPuzzleCompleted(bool wasSuccess)
    {
        if (wasSuccess)
        {
            HandleSuccess();
            return;
        }
        
        _solveAttemptCount++;
        
        if(_solveAttemptCount >= _MaxSolveAttempts)
        {
            _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, false, CreateTriggerArgs(TriggerType.TerminalLockout));
            _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
        }


        void HandleSuccess()
        {
             bool canGotoNextLevel = GotoNextLevel();
            
            // We have completed all required puzzle levels
            if (!canGotoNextLevel)
            {
                _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, false, CreateTriggerArgs(TriggerType.PuzzleSolved));
                _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
            }
        }

        GameTriggerArgs CreateTriggerArgs(TriggerType type)
        {
            return new GameTriggerArgs
            {
                TriggerType = type,
                TriggerKey  = _activeTriggerKey,
            };
        }
    }
    
    private bool GotoNextLevel()
    {
        if (!_levelIndexStack.TryPop(out int nextLevelIndex))
        {
            return false;
        }
        
        DestroyPuzzle(_activePuzzle);
        _activePuzzle = CreatePuzzle(_activePuzzleType, nextLevelIndex);
        
        return true;
    }
#endregion
}
