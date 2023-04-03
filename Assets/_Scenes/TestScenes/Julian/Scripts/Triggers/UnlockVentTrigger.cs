using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockVentTrigger : TriggerBase
{
    // Start is called before the first frame update
    public VentController _TargetVent;
    protected override void OnGameTrigger()
    {
        _TargetVent.UnlockDoors();
    }

}
