
namespace Game2D
{
    public class SpawnState : BaseBrainState
    {
        public override CharacterActionState State => CharacterActionState.Spawn;

        public override bool TryEnterState(NPCController controller)
        {
            return true;
        }

        public override void ProcessInput(NPCController controller, ref InputData inputData)
        {
        }

        public override bool TryExitState(NPCController controller)
        {
            return controller.CharacterController.isGrounded;
        }
    }
}
