
namespace Game2D
{
    public class SpawnState : IBrainState
    {
        public CharacterActionState State => CharacterActionState.Spawn;
        public CharacterActionState[] NextState => new[] { CharacterActionState.FreeMovement };

        public bool TryEnterState(NPCController controller)
        {
            return true;
        }

        public void ProcessInput(NPCController controller, ref InputData inputData)
        {
        }

        public bool TryExitState(NPCController controller)
        {
            return controller.CharacterController.isGrounded;
        }
    }
}