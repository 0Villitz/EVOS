
using UnityEngine;

namespace Game2D
{
    public class HorizontalMovementState : BaseBrainState
    {
        public override CharacterActionState State => CharacterActionState.FreeMovement;

        private UnitMovement _unitMovement = UnitMovement.Idle;
        private int _nextNodeIndex = -1;

        public override bool TryEnterState(NPCController controller)
        {
            if (controller.CharacterController.isGrounded)
            {
                _unitMovement = UnitMovement.Idle;
                _nextNodeIndex = -1;
                return true;
            }

            return false;
        }

        public override void ProcessInput(NPCController controller, ref InputData inputData)
        {
            _unitMovement = MoveToNextNode(controller);
            int horizontalInput = (_unitMovement & UnitMovement.MoveRight) == UnitMovement.MoveRight
                ? 1
                : (_unitMovement & UnitMovement.MoveLeft) == UnitMovement.MoveLeft
                    ? -1
                    : 0;
            inputData.SetHorizontal(horizontalInput);
        }

        public override bool TryExitState(NPCController controller)
        {
            return controller.IsDetectingPlayer();
        }

        private UnitMovement MoveToNextNode(NPCController controller)
        {
            if (_nextNodeIndex < 0)
            {
                _nextNodeIndex = GetClosestTargetNode(controller);
            }

            NavigationNode nextNode = controller.Path[_nextNodeIndex];
            if (OnTargetNode(controller, nextNode))
            {
                controller.transform.position = new Vector3(
                    nextNode.transform.position.x,
                    controller.transform.position.y,
                    nextNode.transform.position.z
                );
                _nextNodeIndex = _nextNodeIndex == controller.Path.Count - 1
                    ? 0
                    : _nextNodeIndex + 1;
                nextNode = controller.Path[_nextNodeIndex];
            }

            Vector3 nodeDirection = (nextNode.transform.position - controller.transform.position).normalized;
            float dotProd = Vector3.Dot(nodeDirection, controller.transform.forward);

            return dotProd > 0
                ? UnitMovement.MoveRight
                : dotProd < 0
                    ? UnitMovement.MoveLeft
                    : UnitMovement.Idle;
        }

        private int GetClosestTargetNode(NPCController controller)
        {
            Vector3 characterPosition = controller.transform.position;
            float minDistance = float.MaxValue;
            int nextIndex = -1;

            for (int idx = 0; idx < controller.Path.Count; idx++)
            {
                NavigationNode node = controller.Path[idx];
                float sqrDistance = (node.transform.position - characterPosition).sqrMagnitude;
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    nextIndex = idx;
                }
            }

            return nextIndex;
        }

        private bool OnTargetNode(NPCController controller, NavigationNode node)
        {
            Vector3 nodeDirection = (node.transform.position - controller.transform.position).normalized;
            float dotProd = Vector3.Dot(nodeDirection, controller.transform.forward);

            if ((_unitMovement & UnitMovement.MoveRight) == UnitMovement.MoveRight
                && dotProd < 0
               )
            {
                return true;
            }

            if ((_unitMovement & UnitMovement.MoveLeft) == UnitMovement.MoveLeft
                && dotProd > 0
               )
            {
                return true;
            }

            return false;
        }
    }
}