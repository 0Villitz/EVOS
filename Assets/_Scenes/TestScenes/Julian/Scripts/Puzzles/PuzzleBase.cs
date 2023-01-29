using System;
using UnityEngine;

namespace Puzzles
{
    public abstract class PuzzleBase : MonoBehaviour
    {
        public abstract void Init();

        public Canvas WindowCanvas { get; set; }

        public event Action<bool> onPuzzleComplete;

        protected void TriggerPuzzleComplete(bool wasSuccess)
        {
            onPuzzleComplete?.Invoke(wasSuccess);
        }
    }
}
