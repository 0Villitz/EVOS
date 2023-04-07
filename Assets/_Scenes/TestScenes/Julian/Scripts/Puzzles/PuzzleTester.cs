using System.Collections.Generic;
using Puzzles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PuzzleTester : MonoBehaviour
{
    public ScriptableEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary          _PuzzleDictionary;

    public string     _TriggerKey;
    public PuzzleBase _PuzzleLevel;

    public  KeyCode _ToggleKey = KeyCode.F;
    public  KeyCode _Modifier = KeyCode.LeftControl;
    
    private bool       _isShowing = true;
    private KeyControl _toggleKeyControl;
    
    private void Awake()
    {
        _toggleKeyControl      = Keyboard.current.FindKeyOnCurrentKeyboardLayout(_ToggleKey.ToString());
        
        _isShowing = Application.isEditor;
    }

    private void Update()
    {
        if (Keyboard.current.ctrlKey.isPressed && _toggleKeyControl.wasPressedThisFrame)
        {
            _isShowing = !_isShowing;
        }    
    }
    
    private void OnGUI()
    {
        if (!_isShowing)
            return;
            
            
        if (GUILayout.Button("Test Specific Puzzle"))
        {
            var showPuzzleArgs = new ShowPuzzleArgs
            {
                TriggerKey        = _TriggerKey,
                PuzzleType = _PuzzleDictionary.GetTypeForPuzzle(_PuzzleLevel),
                SpecificLevelList = new List<PuzzleBase> { _PuzzleLevel },
            };
        
            _GameEventDispatcher.DispatchEvent(GameEventType.ShowPuzzleWindow, showPuzzleArgs);
        }
        
        if(GUILayout.Button("Test Puzzle Solve Success"))
        {
            _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger,
               new GameTriggerArgs
               {
                   TriggerType = TriggerType.PuzzleSolved,
                   TriggerKey  = _TriggerKey,
               });
        }
        
        if(GUILayout.Button("Test Puzzle Level Completed"))
        {
            _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger,
               new GameTriggerArgs
               {
                   TriggerType = TriggerType.PuzzleLevelCompleted,
                   TriggerKey  = _TriggerKey,
               });
        }
        
        if(GUILayout.Button("Test Puzzle Lockout"))
        {
            _GameEventDispatcher.DispatchEvent(GameEventType.GameTrigger, 
        new GameTriggerArgs
               {
                   TriggerType = TriggerType.TerminalLockout,
                   TriggerKey  = _TriggerKey,
               });
        }
    }
}
