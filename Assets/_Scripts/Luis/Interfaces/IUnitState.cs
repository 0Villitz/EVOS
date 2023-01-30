using UnityEngine;

namespace Game2D
{   
    public interface IUnitState
    {
        void Initialize();
        UnitAnimations ProcessInput(InputData inputData, Component component);
    }
}