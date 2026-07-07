using UnityEngine;

/// <summary>
/// Žg‚Á‚Ä‚È‚¢
/// </summary>
public readonly struct GameFinishSignal
{
    public readonly GameFinishRuleType Type;
    public readonly object Context;

    public GameFinishSignal(GameFinishRuleType type, object context = null)
    {
        Type = type;
        Context = context;
    }
}