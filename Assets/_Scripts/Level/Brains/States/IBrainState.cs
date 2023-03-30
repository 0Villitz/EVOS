
namespace Game2D
{
    public interface IBrainState
    {
        CharacterActionState State { get; }
        CharacterActionState[] NextState { get; }
        bool TryEnterState(NPCController controller);
        void ProcessInput(NPCController controller, ref InputData inputData);
        bool TryExitState(NPCController controller);
    }
}