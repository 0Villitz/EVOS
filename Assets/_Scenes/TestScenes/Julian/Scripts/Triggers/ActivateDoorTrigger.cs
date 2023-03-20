using System;
using System.Collections.Generic;
    
public class ActivateDoorTrigger : TriggerBase
{
    public List<DoorController> _TargetList;
    public Action     _ActiveState;   
    
    protected override void OnGameTrigger()
    {
        foreach (var targetDoor in _TargetList)
        {
            if (_ActiveState == Action.Open)
            {
                targetDoor.UnlockDoors();
            }
            else if(_ActiveState == Action.Close)
            {
                targetDoor.LockDoors();
            }
        }
    }

    [Serializable]
    public enum Action
    {
        Open,
        Close,
    }
}
