
namespace Puzzles
{
    public struct FroggerInputFrame
    {
        public float horizontal;
        public float vertical;

        public bool HasMovementInput()
        {
            return !(horizontal == 0 && vertical == 0);
        }
    }
}
