using System;
using System.Collections.Generic;
using System.Linq;
using Puzzles;
using UnityEngine;

public class PuzzleTerminal : MonoBehaviour, IPlayerInteractable
{
    public ScriptableEventDispatcher _GameEventDispatcher;
    public Collider2D _Collider;
    
    public float      _InteractableCutoffDistance;
    public string     _TriggerKey;
    public PuzzleType _PuzzleType;
    
    public int       _RandomLevelCount;
    public List<PuzzleBase> _SpecificLevelList;

    
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
            showPuzzleArgs.SpecificLevelList = _SpecificLevelList.ToList();
        }

        _GameEventDispatcher.DispatchEvent(GameEventType.ShowPuzzleWindow, showPuzzleArgs);
    }
}
