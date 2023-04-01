
public class SetTerminalLockStateTrigger : TriggerBase
{
    public PuzzleTerminal           _Terminal;
    public PuzzleTerminal.LockState _LockState;

    protected override void OnGameTrigger()
    {
        if (_LockState == PuzzleTerminal.LockState.Lock)
        {
            _Terminal.Lock();
        }
        else if (_LockState == PuzzleTerminal.LockState.Unlock)
        {
            _Terminal.Unlock();
        }
    }
}

