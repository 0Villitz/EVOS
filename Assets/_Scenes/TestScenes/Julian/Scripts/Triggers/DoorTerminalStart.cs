using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorTerminalStart : TriggerBase
{
    public List<GameObject> _TargetList;
    public GameObject _TargetSound; 
    public Action           _ActiveState;
    
    protected override void OnGameTrigger()
    {
       
         if(_ActiveState == Action.TurnOff)
            {
                _TargetSound.SetActive(false);
            }
            else
            {
                _TargetSound.SetActive(true);
            }
    }

    [Serializable]
    public enum Action
    {
        TurnOn,
        TurnOff,
    }
}
