
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game2D
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private SpawnObject [] _spawnObjects;
        [SerializeField] private Transform _player;

        public void SpawnNPC()
        {
            foreach (SpawnObject spawn in _spawnObjects)
            {
                spawn.SpawnNPC(_player);
            }
        }

        #region MonoBehaviour

        private void Start()
        {
            SpawnNPC();
        }

        #endregion
    }
}