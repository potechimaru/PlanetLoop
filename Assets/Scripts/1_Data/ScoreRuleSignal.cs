using UnityEngine;

public readonly struct ScoreRuleSignal
{
    public readonly ScoreRuleType Type;
    public readonly int Amount;
    public readonly Vector3 WorldPos;
    public readonly object Context;

    public ScoreRuleSignal(ScoreRuleType type, int amount, Vector3 worldPos = default, object context = null)
    {
        Type = type;
        Amount = amount;
        WorldPos = worldPos;
        Context = context;
    }
}