
namespace Game2D
{
    public static class BrainStateBuilder
    {
        public static IBrainState Build(CharacterStateConfig dataConfig)
        {
            IBrainState brainState = null;
            switch (dataConfig.state)
            {
                case CharacterActionState.Attack:
                    brainState = new AttackState();
                    break;

                case CharacterActionState.Chase:
                    brainState = new ChaseState();
                    break;

                case CharacterActionState.Spawn:
                    brainState = new SpawnState();
                    break;

                case CharacterActionState.FreeMovement:
                    brainState = new HorizontalMovementState();
                    break;
            }

            brainState?.Initialize(dataConfig.nextState);

            return brainState;
        }
    }
}