using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorChangeLight : TriggerBase
{
    public Light2D l1;
    public Action           _ActiveState;
    
    protected override void OnGameTrigger()
    {
       
         if(_ActiveState == Action.TurnOff)
            {
                 l1.color = Color.white;
            }
            else
            {
                 l1.color = Color.red;
            }
    }

    [Serializable]
    public enum Action
    {
        TurnOn,
        TurnOff,
    }
}
