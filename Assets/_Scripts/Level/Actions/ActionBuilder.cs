
namespace Game2D
{
    public static class ActionBuilder
    {
        public static IUnitAction Build(UnitMovement unitMovement, ActionController controller)
        {
            switch (unitMovement)
            {
                case UnitMovement.MoveHorizontal: return new HorizontalAction(controller);
                case UnitMovement.Jump: return new JumpAction(controller);
                case UnitMovement.Falling: return new FallAction(controller);
                case UnitMovement.Climb: return new ClimbAction(controller);
                case UnitMovement.AttackHorizontal: return new AttackHorizontalAction(controller);
                default: return null;
            }
        }
    }
}