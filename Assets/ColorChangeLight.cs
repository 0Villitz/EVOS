using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ColorChangeLight : TriggerBase
{
    public Light2D l1;


    protected override void OnGameTrigger()
    {
        l1.color = Color.red;
    }
}
