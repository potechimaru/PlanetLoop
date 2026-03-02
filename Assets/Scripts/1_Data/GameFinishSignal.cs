using UnityEngine;

public readonly struct GameFinishSignal
{
    public readonly GameFinishRuleType Type;
    public readonly object Context;      // îCà”Åinullâ¬Åj

    public GameFinishSignal(GameFinishRuleType type, object context = null)
    {
        Type = type;
        Context = context;
    }
}