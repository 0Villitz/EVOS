using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    public enum ControlType
    {
        None,
        Player,
        UI,
    }

    public ControlType CurrentControlType
    {
        get => currentControlType;
        set
        {
            currentControlType = value;
            switch (value)
            {
                case ControlType.None: 
                    DisableAllControls();
                    break;
                case ControlType.Player: 
                    EnablePlayerControls();
                    break;
                case ControlType.UI: 
                    EnableUIControls();
                    break;
            }
        }
    }
    
    
    private Controls    myControls;
    private ControlType currentControlType;

    public bool ShouldInteract => myControls.Player.Interact.WasPressedThisFrame();
    
    public float                     valueX;
    public bool                      jumpInput;
    public ScriptableEventDispatcher _GameEventDispatcher;
    public bool pause = false;


    private void Awake()
    {
        myControls = new Controls();
        myControls.Player.Move.performed += StartMove;
        myControls.Player.Move.canceled  += StopMove;

        myControls.Player.Jump.performed += JumpStart;
        myControls.Player.Jump.canceled  += JumpStop;

        myControls.UI.Pause.performed += Pause;
        myControls.Player.Pause.performed += Pause;

        CurrentControlType = ControlType.Player;
        
        _GameEventDispatcher.AddListener(GameEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.AddListener(GameEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }

    private void OnDestroy()
    {
        myControls.Player.Move.performed -= StartMove;
        myControls.Player.Move.canceled  -= StopMove;

        myControls.Player.Jump.performed -= JumpStart;
        myControls.Player.Jump.canceled  -= JumpStop;

        myControls.UI.Pause.performed -= Pause;
        myControls.Player.Pause.performed -= Pause;

        _GameEventDispatcher.RemoveListener(GameEventType.ShowPuzzleWindow, OnShowPuzzleWindow);
        _GameEventDispatcher.RemoveListener(GameEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }

    private void Update()
    {
        // jwilliams - Example of how to start a puzzle sequence
        // We create a `ShowPuzzleArgs` and set parameters for the 
        // type of puzzle we would like to initiate
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            CurrentControlType = ControlType.UI;

            var showPuzzleArgs = new ShowPuzzleArgs
            {
                PuzzleType  = Puzzles.PuzzleType.Path,
                TriggerKey = "terminal-a",
                RandomLevelCount = 3,
            };

            _GameEventDispatcher.DispatchEvent(GameEventType.ShowPuzzleWindow, showPuzzleArgs);
        }
        
        // Test frogger puzzle
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            CurrentControlType = ControlType.UI;

            var showPuzzleArgs = new ShowPuzzleArgs
            {
                PuzzleType  = Puzzles.PuzzleType.Frogger,
                TriggerKey = "terminal-a",
                RandomLevelCount = 1,
            };

            _GameEventDispatcher.DispatchEvent(GameEventType.ShowPuzzleWindow, showPuzzleArgs);
        }
    }
    
    private void OnEnable()
    {
        CurrentControlType = ControlType.Player;
    }

    private void OnDisable()
    {
        CurrentControlType = ControlType.None;
    }

    private void OnShowPuzzleWindow(GeneralEvent obj)
    {
        CurrentControlType = ControlType.UI;
    }
    
    private void OnHidePuzzleWindow(GeneralEvent obj)
    {
        CurrentControlType = ControlType.Player;
    }
    
    public void DisableControls()
    {
       DisableAllControls();
    }

    private void EnablePlayerControls()
    {
        myControls.Player.Enable();
        myControls.UI.Disable();
    }
    
    private void EnableUIControls()
    {
        myControls.UI.Enable();
        myControls.Player.Disable();
        // Prevent player from moonwalking without user input!
        valueX = 0;
    }

    private void DisableAllControls()
    {
        myControls.Player.Disable();
        myControls.UI.Disable();
        valueX = 0;
    }


    private void StartMove(InputAction.CallbackContext ctx)
    {
        valueX = Mathf.RoundToInt(ctx.ReadValue<float>());
        //Debug.Log("valueX: " + valueX );
    }
    private void StopMove(InputAction.CallbackContext ctx)
    {
        valueX = 0;
    }

    private void JumpStart(InputAction.CallbackContext ctx)
    {
        jumpInput = true;
    }

    private void JumpStop(InputAction.CallbackContext ctx)
    {
        jumpInput = false;
    }
    private void Pause(InputAction.CallbackContext ctx)
    {
        pause = !pause;
    }

}
