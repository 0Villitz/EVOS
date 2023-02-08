using UnityEngine;

namespace Game2D
{   
    public interface IUnitState
    {
        void Initialize();
        UnitMovement ProcessInput(UnitMovement [] actionTypes, InputData inputData, Component component);
    }
}