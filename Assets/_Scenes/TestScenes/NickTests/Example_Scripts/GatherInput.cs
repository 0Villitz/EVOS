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
    
    public  float       valueX;
    public  bool        jumpInput;

    private void Awake()
    {
        myControls = new Controls();
        myControls.Player.Move.performed += StartMove;
        myControls.Player.Move.canceled  += StopMove;

        myControls.Player.Jump.performed += JumpStart;
        myControls.Player.Jump.canceled  += JumpStop;

        CurrentControlType = ControlType.Player;
    }

    private void OnDestroy()
    {
        myControls.Player.Move.performed -= StartMove;
        myControls.Player.Move.canceled  -= StopMove;

        myControls.Player.Jump.performed -= JumpStart;
        myControls.Player.Jump.canceled  -= JumpStop;
    }
    
    private void OnEnable()
    {
        CurrentControlType = ControlType.Player;
    }

    private void OnDisable()
    {
        CurrentControlType = ControlType.None;
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
