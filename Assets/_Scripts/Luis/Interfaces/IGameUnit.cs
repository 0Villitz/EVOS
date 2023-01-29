
using System.Collections.Generic;

namespace Game2D
{
    public interface IGameUnit
    {
        bool IsGrounded { get; }
        // List<IInteractableObject> GetInteractableObjects();
    }

    public interface IUnitState
    {
        void Initialize();
        UnitAnimations ProcessInput(InputData inputData);
    }
}