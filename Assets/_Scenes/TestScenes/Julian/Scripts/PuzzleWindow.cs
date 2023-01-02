using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleWindow : MonoBehaviour
{
    public GameEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary    _PuzzleDictionary;
    
    public GameObject    _BaseView;

    private GameObject _activePuzzle;

    private void Awake()
    {
        
        _GameEventDispatcher.AddListener(PuzzleEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.AddListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);
        _GameEventDispatcher.AddListener(PuzzleEventType.ObstacleHit, OnObstacleHit);

        OnHidePuzzleWindow(null);
    }

    // private void OnStartButton()
    // {
    //     _GameEventDispatcher.DispatchEvent(PuzzleEventType.Start);
    // }
    //
    // private void OnExitButton()
    // {
    //     _GameEventDispatcher.DispatchEvent(PuzzleEventType.End);
    //     _GameEventDispatcher.DispatchEvent(PuzzleEventType.HidePuzzleWindow);
    // }

    private void OnObstacleHit(GeneralEvent obj)
    {
    }
    
    private void OnHidePuzzleWindow(GeneralEvent obj)
    {
        _BaseView.SetActive(false);
        
        if (_activePuzzle != null)
        {
            GameObject.Destroy(_activePuzzle);
        }
    }
    
    private void OnShowPuzzleWindow(GeneralEvent obj)
    {
        PuzzleType type = (PuzzleType)obj.data;
        _activePuzzle = _PuzzleDictionary.CreateRandomPuzzleOfType(type, null);
        
        _BaseView.SetActive(true);
    }
}
