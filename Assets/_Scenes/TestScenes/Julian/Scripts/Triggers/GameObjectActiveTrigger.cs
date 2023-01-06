using System;
using System.Collections.Generic;
using UnityEngine;
    
public class GameObjectActiveTrigger : TriggerBase
{
    public List<GameObject> _TargetList;
    public Action           _TriggerActiveState;   
    
    protected override void OnGameTrigger()
    {
        foreach (var target in _TargetList)
        {
            if (_TriggerActiveState == Action.Toggle)
            {
                target.SetActive(!target.activeSelf);
            }
            else if(_TriggerActiveState == Action.TurnOff)
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
