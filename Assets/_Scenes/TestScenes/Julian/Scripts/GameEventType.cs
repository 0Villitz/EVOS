
using System.Collections.Generic;
using Puzzles;
public static class GameEventType
{
    // Puzzles
    public const string ShowPuzzleWindow = "puzzle_show_window";
    public const string HidePuzzleWindow = "puzzle_hide_window";
    
    // Triggers
    public const string GameTrigger = "game_trigger";
}

public struct ShowPuzzleArgs
{
    public Puzzles.PuzzleType PuzzleType       { get; set; }
    public string     TriggerKey       { get; set; }
    public int        RandomLevelCount { get; set; }
    public List<PuzzleBase>  SpecificLevelList { get; set; }   
}

public struct GameTriggerArgs
{
    public string      TriggerKey { get; set; }
    public TriggerType TriggerType       { get; set; }
}
