
public class TerminalLockoutTrigger : TriggerBase
{
    public float          _LockoutTimeInSeconds = 3.0f;
    public PuzzleTerminal _Terminal;


    protected override void OnGameTrigger()
    {
        _Terminal.Lockout(_LockoutTimeInSeconds);
    }
}
