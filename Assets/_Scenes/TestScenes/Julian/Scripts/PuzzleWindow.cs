using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleWindow : MonoBehaviour
{
    public GameEventDispatcher _GameEventDispatcher;
    public PuzzleDictionary    _PuzzleDictionary;
    
    public Button        _StartButton;
    public Button        _ExitButton;
    public GameObject    _BaseView;
    public Transform     _ButtonGroup;

    private GameObject _activePuzzle;

    private void Awake()
    {
        _StartButton.onClick.AddListener(OnStartButton);
        _ExitButton.onClick.AddListener(OnExitButton);
        
        _GameEventDispatcher.AddListener(PuzzleEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.AddListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);

        OnHidePuzzleWindow(null);
    }

    private void OnStartButton()
    {
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.Start);
        _ButtonGroup.gameObject.SetActive(false);
    }
    
    private void OnExitButton()
    {
        _GameEventDispatcher.DispatchEvent(PuzzleEventType.End);
        OnHidePuzzleWindow(null);
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
