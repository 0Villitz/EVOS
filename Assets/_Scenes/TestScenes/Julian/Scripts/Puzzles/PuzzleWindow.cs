using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Puzzles
{
    public class PuzzleWindow : MonoBehaviour
    {
#region Editor Properties

        [Header("Scriptable Objects")]
        public ScriptableEventDispatcher _GameEventDispatcher;
        public PuzzleDictionary _PuzzleDictionary;

        [Header("Asset References")]
        public GameObject _BaseView;
        public TextMeshProUGUI _AttemptsLeftText;
        public TextMeshProUGUI _LevelsText;

        [Header("Tuning Parameters")]
        public int _MaxSolveAttempts;

#endregion

#region Private vars

        private PuzzleBase _activePuzzle;
        private PuzzleType _activePuzzleType;
        private string     _activeTriggerKey;
        private Stack<int> _levelIndexStack;
        private int        _solveAttemptCount;
        private int        _startingLevelCount;
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
            
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
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

            _levelIndexStack    = CreateLevelIndexStack(args);
            _startingLevelCount = _levelIndexStack.Count;

            if (_startingLevelCount == 0)
            {
                OnPuzzleCompleted(true);
            }
            else
            {
                GotoNextLevel();
                UpdateText();
            }
        }

        private Stack<int> CreateLevelIndexStack(ShowPuzzleArgs args)
        {
            List<int> levelList;
            if (args.SpecificLevelList != null && args.SpecificLevelList.Count > 0)
            {
                levelList = _PuzzleDictionary.CreateIndexListFromPuzzleList(args.PuzzleType, args.SpecificLevelList);
                levelList.Reverse();
            }
            else
            {
                levelList = _PuzzleDictionary.CreateRandomIndexListOfType(args.PuzzleType, args.RandomLevelCount);
            }
            
            return levelList != null ? new Stack<int>(levelList) : null;
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

            UpdateText();

            if (_solveAttemptCount >= _MaxSolveAttempts)
            {
                _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, CreateTriggerArgs(TriggerType.TerminalLockout));
                _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
            }


            void HandleSuccess()
            {
                _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, CreateTriggerArgs(TriggerType.PuzzleLevelCompleted));
            
                bool canGotoNextLevel = GotoNextLevel();

                UpdateText();

                // We have completed all required puzzle levels
                if (!canGotoNextLevel)
                {
                    _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, CreateTriggerArgs(TriggerType.PuzzleSolved));
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

        private void UpdateText()
        {
            _AttemptsLeftText.text = $"Attempts Left: {_MaxSolveAttempts - _solveAttemptCount}";
            _LevelsText.text       = $"Levels: {_startingLevelCount - _levelIndexStack.Count}/{_startingLevelCount}";
        }

#endregion
    }
}
