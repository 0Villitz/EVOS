
public class TerminalSetupTrigger : TriggerBase
{
    public PuzzleTerminal _Terminal;


    protected override void OnGameTrigger()
    {
        _Terminal.Setup();
    }
}
