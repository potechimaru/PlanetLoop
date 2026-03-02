using System;
using System.Collections.Generic;
using UniRx;

public sealed class GameFinishRuleEngine : IDisposable
{
    private readonly Dictionary<GameFinishRuleType, IGameFinishRule> _rules;

    public GameFinishRuleEngine(
        PlayerDeadFinishRule playerDead,
        TimeUpFinishRule timeUp)
    {
        _rules = new Dictionary<GameFinishRuleType, IGameFinishRule>
        {
            { GameFinishRuleType.PlayerDead, playerDead },
            { GameFinishRuleType.TimeLimit, timeUp },
        };
    }

    public IObservable<GameFinishSignal> GetStream(GameFinishRuleType type)
    {
        if (_rules.TryGetValue(type, out var rule))
            return rule.OnTriggered;

        return Observable.Empty<GameFinishSignal>();
    }

    public void Publish(in GameFinishSignal signal)
    {
        if (_rules.TryGetValue(signal.Type, out var rule))
            rule.Trigger(signal);
    }

    public void Dispose()
    {
        foreach (var kv in _rules) kv.Value.Dispose();
        _rules.Clear();
    }
}