using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[CreateAssetMenu(fileName = "PuzzleDictionary", menuName = "EVOS/PuzzleDictionary")]
public class PuzzleDictionary : ScriptableObject
{
    public List<PuzzleKeyValuePair> _PuzzleList;


    public PuzzleBase CreateRandomPuzzleOfType(PuzzleType type, Transform parent)
    {
        PuzzleKeyValuePair pair = _PuzzleList.Find((x) => x.Type == type);
        // Get random puzzle in list
        if (pair?.PuzzleList?.Count == 0)
            return null;

        int puzzleCount = pair.PuzzleList.Count;

        PuzzleBase puzzlePrefab = pair.PuzzleList[UnityEngine.Random.Range(0, puzzleCount)];
        return GameObject.Instantiate<PuzzleBase>(puzzlePrefab, parent);
    }

    [Serializable]
    public class PuzzleKeyValuePair
    {
       public PuzzleType Type;
       public List<PuzzleBase>     PuzzleList;
    }
}
    
public enum PuzzleType
{
   None,
   Path,
   Frogger,
}
