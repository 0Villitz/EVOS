
using UnityEngine;

namespace Game2D
{
    public class ChaseState : IBrainState
    {
        public CharacterActionState State => CharacterActionState.Chase;

        public CharacterActionState[] NextState => new[]
        {
            CharacterActionState.Attack,
            CharacterActionState.FreeMovement
        };

        private UnitMovement _unitMovement = UnitMovement.Idle;
        private int _nextNodeIndex = -1;

        public bool TryEnterState(NPCController controller)
        {
            return controller.IsDetectingPlayer();
        }

        public void ProcessInput(NPCController controller, ref InputData inputData)
        {
            _unitMovement = MoveToPlayer(controller);
            int horizontalInput = (_unitMovement & UnitMovement.MoveRight) == UnitMovement.MoveRight
                ? 1
                : (_unitMovement & UnitMovement.MoveLeft) == UnitMovement.MoveLeft
                    ? -1
                    : 0;
            inputData.SetHorizontal(horizontalInput);
        }

        public bool TryExitState(NPCController controller)
        {
            return !controller.IsDetectingPlayer() || controller.PlayerWithInAttackRange();
        }

        private UnitMovement MoveToPlayer(NPCController controller)
        {
            Vector3 playerPosition = controller.Player.GetTransform().position;
            Vector3 nodeDirection = (playerPosition - controller.transform.position).normalized;
            float dotProd = Vector3.Dot(nodeDirection, controller.transform.forward);

            return dotProd > 0
                ? UnitMovement.MoveRight
                : dotProd < 0
                    ? UnitMovement.MoveLeft
                    : UnitMovement.Idle;
        }
    }
}