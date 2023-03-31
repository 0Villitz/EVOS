
namespace Game2D
{
    public interface IBrainState
    {
        void Initialize(CharacterActionState[] nextStates);
        CharacterActionState State { get; }
        CharacterActionState[] NextState { get; }
        bool TryEnterState(NPCController controller);
        void ProcessInput(NPCController controller, ref InputData inputData);
        bool TryExitState(NPCController controller);
    }
}