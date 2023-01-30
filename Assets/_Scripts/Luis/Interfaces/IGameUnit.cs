
using System.Collections.Generic;

namespace Game2D
{
    public interface IGameUnit
    {
        bool IsGrounded { get; }
    }

    public interface IFallMovementUnit
    {
        bool CanFall();
        bool IsMovingDownSlop();
    }

    public interface IJumpMovementUnit
    {
        bool CanJump();
    }

    public interface IUnitState
    {
        void Initialize();
        UnitAnimations ProcessInput(InputData inputData);
    }
}