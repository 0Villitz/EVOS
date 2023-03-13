
using System.Collections.Generic;
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
            int spawnIdx = Random.Range(0, _spawnObjects.Length - 1);
            _spawnObjects[spawnIdx].SpawnNPC(_player);
        }

        #region MonoBehaviour

        private void Start()
        {
            SpawnNPC();
        }

        #endregion
    }
}