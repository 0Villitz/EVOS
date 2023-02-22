using System.Collections.Generic;
using Puzzles;
using UnityEngine;

public class PuzzleTerminal : MonoBehaviour, IPlayerInteractable
{
    public ScriptableEventDispatcher _GameEventDispatcher;

    public float      _InteractableCutoffDistance;
    public string     _TriggerKey;
    public PuzzleType _PuzzleType;
    
    public int       _RandomLevelCount;
    public List<int> _SpecificLevelList;

    
    public float InteractCutoffDistance => _InteractableCutoffDistance;
    
    public void Interact()
    {
        if (string.IsNullOrEmpty(_TriggerKey))
        {
            Debug.LogError($"Puzzle Terminal:{name} cannot have an empty Trigger key!");
            return;
        }
        
        var showPuzzleArgs = new ShowPuzzleArgs
        {
            PuzzleType = _PuzzleType,
            TriggerKey = _TriggerKey,
        };
        
        if (_RandomLevelCount > 0)
        {
            showPuzzleArgs.RandomLevelCount = _RandomLevelCount;
        }
        else
        {
            showPuzzleArgs.SpecificLevelList = _SpecificLevelList;
        }

        _GameEventDispatcher.DispatchEvent(GameEventType.ShowPuzzleWindow, showPuzzleArgs);
    }
}
