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
    
    public float               valueX;
    public bool                jumpInput;
    public GameEventDispatcher _GameEventDispatcher;

    private void Awake()
    {
        myControls = new Controls();
        myControls.Player.Move.performed += StartMove;
        myControls.Player.Move.canceled  += StopMove;

        myControls.Player.Jump.performed += JumpStart;
        myControls.Player.Jump.canceled  += JumpStop;

        CurrentControlType = ControlType.Player;
        
        _GameEventDispatcher.AddListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }

    private void OnDestroy()
    {
        myControls.Player.Move.performed -= StartMove;
        myControls.Player.Move.canceled  -= StopMove;

        myControls.Player.Jump.performed -= JumpStart;
        myControls.Player.Jump.canceled  -= JumpStop;
        
        _GameEventDispatcher.RemoveListener(PuzzleEventType.HidePuzzleWindow, OnHidePuzzleWindow);
    }

    private void Update()
    {
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            CurrentControlType = ControlType.UI;
        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            CurrentControlType = ControlType.Player;
        }

        // Test
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            CurrentControlType = ControlType.UI;

            var showPuzzleArgs = new ShowPuzzleArgs
            {
                PuzzleType  = PuzzleType.Path,
                TerminalKey = "test",
            };

            _GameEventDispatcher.DispatchEvent(PuzzleEventType.ShowPuzzleWindow, false, showPuzzleArgs);

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
}
