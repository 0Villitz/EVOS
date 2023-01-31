using UnityEngine;

namespace Game2D
{   
    public interface IUnitState
    {
        void Initialize();
        UnitAnimations ProcessInput(UnitAnimations [] actionTypes, InputData inputData, Component component);
    }
}