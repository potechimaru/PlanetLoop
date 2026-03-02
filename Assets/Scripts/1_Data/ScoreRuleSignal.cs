using UnityEngine;

public readonly struct ScoreRuleSignal
{
    public readonly ScoreRuleType Type;
    public readonly int Amount;          // 必要なら。未使用なら 0 でOK
    public readonly Vector3 WorldPos;    // 必要なら。未使用なら Vector3.zero
    public readonly object Context;      // 任意（null可）

    public ScoreRuleSignal(ScoreRuleType type, int amount = 0, Vector3 worldPos = default, object context = null)
    {
        Type = type;
        Amount = amount;
        WorldPos = worldPos;
        Context = context;
    }
}