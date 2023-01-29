using UnityEngine;

namespace Puzzles
{
    public static class PuzzleUtils
    {
        public static float Ease(float x, float ease)
        {
            float a = ease + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }
    }
}
