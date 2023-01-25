using UnityEngine;

namespace Puzzles
{
    public class Teleporter : MonoBehaviour, IPuzzleInteractable
    {
        public Transform  _Exit;
        public Collider2D _Collider;

        public void Warp(Transform target)
        {
            target.position = _Exit.transform.position;
        }


        private void OnDrawGizmos()
        {
            if (_Exit != null && _Collider != null)
            {
                Gizmos.DrawWireSphere(_Exit.position, _Collider.bounds.size.x / 2.0f);
            }
        }
    }
}
