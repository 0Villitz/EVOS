
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
            
        // SUUUUUPER Hack,
        // enemy monster NPC's can fall through the floor for some reason sometimes
        // just force them up, and return false, for can fall for now
        // FIX THIS BETTER PLEASE SENIOR SAENZ, I HATE MYSELF FOR THIS
            var position = controller.transform.position;
            if (position.y < -70)
            {
                return true;
            }
            
            return controller.CharacterController.isGrounded;
        }
    }
}
