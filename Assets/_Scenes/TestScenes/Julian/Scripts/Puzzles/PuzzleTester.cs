using System;
using System.Collections;
using System.Collections.Generic;
using Puzzles;
using UnityEngine;

public class PuzzleTester : MonoBehaviour
{
    public ScriptableEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary          _PuzzleDictionary;

    public string     _TriggerKey;
    public PuzzleBase _PuzzleLevel;

    private void OnGUI()
    {
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
