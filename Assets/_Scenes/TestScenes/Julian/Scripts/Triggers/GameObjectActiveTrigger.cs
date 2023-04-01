using System;
using System.Collections.Generic;
using UnityEngine;
    
public class GameObjectActiveTrigger : TriggerBase
{
    public Action           _ActiveState;   
    public List<GameObject> _TargetList;
    
    protected override void OnGameTrigger()
    {
        foreach (var target in _TargetList)
        {
            if (_ActiveState == Action.Toggle)
            {
                target.SetActive(!target.activeSelf);
            }
            else if(_ActiveState == Action.TurnOff)
            {
                target.SetActive(false);
            }
            else
            {
                target.SetActive(true);
            }
        }
    }

    [Serializable]
    public enum Action
    {
        Toggle,
        TurnOn,
        TurnOff,
    }
}
