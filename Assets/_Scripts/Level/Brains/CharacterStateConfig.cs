
using System;

namespace Game2D
{
    [Serializable]
    public class CharacterStateConfig
    {
        public CharacterActionState state;
        public CharacterActionState[] nextState;
        public UnitMovement[] actions;
    }
    
    public interface ICharacterState
    {
        CharacterActionState NextState { get; }
        UnitMovement [] Actions { get; }
        bool TryEnterState();
        bool TryExitState();
    }
}