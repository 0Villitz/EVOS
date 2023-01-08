using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{
    // [RequiredMember]
    public ScriptableEventDispatcher _GameEventDispatcher;

    [Header("Trigger Parameters")]
    public TriggerType type;
    public string _Key;

    protected void Awake()
    {
        _GameEventDispatcher.AddListener(GameEventType.GameTrigger, OnGameTriggerEvent);
    }

    protected void OnDestroy()
    {
        _GameEventDispatcher.RemoveListener(GameEventType.GameTrigger, OnGameTriggerEvent);
    }

    protected abstract void OnGameTrigger();


    private void OnGameTriggerEvent(GeneralEvent e)
    {
        GameTriggerArgs args = (GameTriggerArgs)e.data;
        
        if (type != args.TriggerType ||  _Key != args.TriggerKey)
            return;
            
        OnGameTrigger();
    }
}

