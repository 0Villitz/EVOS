
namespace Game2D
{
    public abstract class BaseBrainState : IBrainState
    {
        private CharacterActionState[] _nextState = null;
        public CharacterActionState[] NextState => _nextState;

        public abstract CharacterActionState State { get; }
        public abstract bool TryEnterState(NPCController controller);
        public abstract void ProcessInput(NPCController controller, ref InputData inputData);
        public abstract bool TryExitState(NPCController controller);

        public virtual void Initialize(CharacterActionState[] nextStates)
        {
            _nextState = nextStates;
        }
    }
}