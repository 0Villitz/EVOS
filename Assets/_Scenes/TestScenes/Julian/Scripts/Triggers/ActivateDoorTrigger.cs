using System;
using System.Collections.Generic;
    
public class ActivateDoorTrigger : TriggerBase
{
    
    //public List<Door> _TargetList;
    public Action     _ActiveState;   
    
    protected override void OnGameTrigger()
    {
       /* foreach (var targetDoor in _TargetList)
        {
            if (_ActiveState == Action.Open)
            {
                //targetDoor.Open();
            }
            else if(_ActiveState == Action.Close)
            {
                //targetDoor.Close();
            }
        }*/
    }

    [Serializable]
    public enum Action
    {
        Open,
        Close,
    }
}
