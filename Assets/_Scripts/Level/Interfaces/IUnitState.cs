using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{   
    public interface IUnitState
    {
        void Initialize(Dictionary<CharacterActionState, UnitMovement[]> unitMovementMap);
        UnitMovement ProcessInput(UnitMovement [] actionTypes, InputData inputData, Component component);
    }
}