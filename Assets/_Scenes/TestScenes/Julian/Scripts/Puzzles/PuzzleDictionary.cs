using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Puzzles
{
    [CreateAssetMenu(fileName = "PuzzleDictionary", menuName = "EVOS/PuzzleDictionary")]
    public class PuzzleDictionary : ScriptableObject
    {
        public List<PuzzleKeyValuePair> _PuzzleList;

        public PuzzleBase CreatePuzzleOfType(PuzzleType type, int levelIndex, Transform parent)
        {
            PuzzleKeyValuePair pair = _PuzzleList.Find((x) => x.Type == type);
            // Get random puzzle in list
            if (pair?.PuzzleList?.Count == 0)
                return null;

            PuzzleBase puzzlePrefab = pair.PuzzleList[levelIndex];
            return GameObject.Instantiate<PuzzleBase>(puzzlePrefab, parent);
        }

        public List<int> CreateIndexListFromPuzzleList(PuzzleType type, List<PuzzleBase> puzzleList)
        {
            PuzzleKeyValuePair pair = _PuzzleList.Find((x) => x.Type == type);
            if (pair?.PuzzleList?.Count == 0)
                return null;

            return  puzzleList.Select(x => pair.PuzzleList.IndexOf(x)).ToList();
        }

        public List<int> CreateRandomIndexListOfType(PuzzleType type, int neededLevelCount)
        {
            PuzzleKeyValuePair pair = _PuzzleList.Find((x) => x.Type == type);
            // Get random puzzle in list
            if (pair?.PuzzleList?.Count == 0)
                return null;

            var indexList = new List<int>(neededLevelCount);

            pair.PuzzleList.ForEach((x) =>
            {
                indexList.Add(indexList.Count);
            });

            indexList = indexList.OrderBy(x => UnityEngine.Random.value).ToList();

            if (neededLevelCount < indexList.Count)
            {
                indexList.RemoveRange(neededLevelCount, indexList.Count - neededLevelCount);
            }

            return indexList;
        }

        [Serializable]
        public class PuzzleKeyValuePair
        {
            public PuzzleType       Type;
            public List<PuzzleBase> PuzzleList;
        }
    }

    public enum PuzzleType
    {
        None,
        Path,
        Frogger,
    }
}
