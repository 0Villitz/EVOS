using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleBase : MonoBehaviour
{
    public abstract void StartPuzzle();
    
    public Canvas WindowCanvas { get; set; }
    
    public event Action<bool> onPuzzleComplete;

    protected void TriggerPuzzleComplete(bool wasSuccess)
    {
        onPuzzleComplete?.Invoke(wasSuccess);
    }
}
