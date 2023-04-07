public class PuzzleCheckpointTrigger : TriggerBase
{
    public PuzzleTerminal _Terminal;

    protected override void OnGameTrigger()
    {
        _Terminal.PuzzleLevelCompleted();
    }
}
