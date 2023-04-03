using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Puzzles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleTerminal : MonoBehaviour, IPlayerInteractable
{
    public enum LockState
    {
        None,
        Lock,
        Unlock,
    }
    [Header("Asset References")]
    public ScriptableEventDispatcher _GameEventDispatcher;
    public Collider2D  _Collider;
    public Animator _Animator;
    public TextMeshPro _CountdownText;

    [Header("Terminal Triggers")]
    public TerminalLockoutTrigger _LockoutTrigger;
    public PuzzleCheckpointTrigger _PuzzleCheckpointTrigger;
    
    [Header("Terminal Controls")]
    public float      _InteractableCutoffDistance;
    public string     _TriggerKey;
    public PuzzleType _PuzzleType;
    
    public int       _RandomLevelCount;
    public List<PuzzleBase> _SpecificLevelList;

   [SerializeField] private float     _unlockTime = float.MaxValue;

   [SerializeField] private bool _startUnlock = false;
   

    public LockState CurrentLockState { get; private set; }
   
    public float InteractCutoffDistance => _InteractableCutoffDistance;

    public string _nextScene = null;


    public void Interact()
    {
        if (string.IsNullOrEmpty(_TriggerKey))
        {
            Debug.LogError($"Puzzle Terminal:{name} cannot have an empty Trigger key!");
            return;
        }

        if (Time.time < _unlockTime)
        {
            PlayLockoutVfx();
            return;
        }
        
        if (CurrentLockState == LockState.Lock)
        {
            Debug.LogWarning($"Puzzle Terminal:{name} is locked!");
            return;
        }

        ShowPuzzle();
        
        
        void PlayLockoutVfx()
        {
            Debug.Log($"Locked out of Terminal! Unlocks in: {_unlockTime - Time.time} seconds!");
        }

        void ShowPuzzle()
        {
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

    public void Lockout(float lockoutTimeInSeconds)
    {
        _unlockTime = Time.time + lockoutTimeInSeconds;
        StartCoroutine(LockoutDelay());
    } 

    public void Unlock()
    {
        CurrentLockState = LockState.Unlock;
        
        _unlockTime      = 0;
        SetAnimationState(CurrentLockState);
        //
        // LoadNextScene();
        // if (_PuzzleType == PuzzleType.Frogger && this.gameObject.name == "DoorTerminal")
        // {
        //     _nextScene = "EVOS_EndCutscene";
        // }
    }

    public void Lock()
    {
        CurrentLockState = LockState.Lock;
        SetAnimationState(CurrentLockState);
    }

    private void SetAnimationState(LockState lockState)
    {
        int animIntValue = lockState == LockState.Unlock ? 1 : 2;
        _Animator.SetInteger("UnlockState", animIntValue);
    }

    public void Setup()
    {
        _CountdownText.gameObject.SetActive(false);

        // Makes setting this up for every terminal automatic & easier
        _LockoutTrigger._Key          = _TriggerKey;
        _PuzzleCheckpointTrigger._Key = _TriggerKey;
        
        if (_startUnlock)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    public void PuzzleLevelCompleted()
    {
        if (_SpecificLevelList.Count > 0)
        {
            _SpecificLevelList.RemoveAt(0);
        }    
        else if (_RandomLevelCount > 0)
        {
            _RandomLevelCount--;
        }
    }
    
    private void Start()
    {
        Setup();
    }
    
    // private void LoadNextScene()
    // {
    //     if (!string.IsNullOrEmpty(_nextScene))
    //     {
    //         int y = SceneManager.GetActiveScene().buildIndex;
    //         SceneManager.UnloadSceneAsync(y);
    //         SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Additive);
    //     }
    // }

    private IEnumerator LockoutDelay()
    {
        Lock();
        
        _CountdownText.gameObject.SetActive(true);
        
        yield return new WaitUntil(() =>
        {
            float lockoutTimeLeft = _unlockTime - Time.time;
            
            _CountdownText.text = $"{Mathf.CeilToInt(lockoutTimeLeft)}";
            return lockoutTimeLeft <= 0.0f;
        });
        
        _CountdownText.gameObject.SetActive(false);
        
        Unlock();
    }
}
