
public class TerminalUnlockTrigger : TriggerBase
{
    public PuzzleTerminal _Terminal;


    protected override void OnGameTrigger()
    {
        _Terminal.Unlock();
    }
}
