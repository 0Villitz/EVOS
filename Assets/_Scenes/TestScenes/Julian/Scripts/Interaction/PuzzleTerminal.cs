using System;
using System.Collections.Generic;
using System.Linq;
using Puzzles;
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
    
    public ScriptableEventDispatcher _GameEventDispatcher;
    public Collider2D _Collider;
    
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

    private Animator _animator;


    public void Interact()
    {
        if (string.IsNullOrEmpty(_TriggerKey))
        {
            Debug.LogError($"Puzzle Terminal:{name} cannot have an empty Trigger key!");
            return;
        }

        if (CurrentLockState == LockState.Lock)
        {
            Debug.LogWarning($"Puzzle Terminal:{name} is locked!");
            return;
        }

        if (Time.time < _unlockTime)
        {
            PlayLockoutVfx();
        }
        else
        {
            ShowPuzzle();
        }


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
        _animator.SetInteger("UnlockState", 2);
    } 

    public void Unlock()
    {
        CurrentLockState = LockState.Unlock;
        
        _unlockTime      = 0;
        _animator.SetInteger("UnlockState", 1);
        
        LoadNextScene();
        if (_PuzzleType == PuzzleType.Frogger && this.gameObject.name == "DoorTerminal")
        {
            _nextScene = "EVOS_EndCutscene";
        }
    }

    public void Lock()
    {
        CurrentLockState = LockState.Lock;
        _animator.SetInteger("UnlockState", 2);
    }

    public void Setup()
    {
        if (_startUnlock)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        Setup();
    }
    
    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(_nextScene))
        {
            int y = SceneManager.GetActiveScene().buildIndex;
            SceneManager.UnloadSceneAsync(y);
            SceneManager.LoadSceneAsync(_nextScene, LoadSceneMode.Additive);
        }
    }
}
