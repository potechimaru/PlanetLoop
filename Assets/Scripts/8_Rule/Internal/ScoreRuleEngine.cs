using System;
using System.Collections.Generic;
using UniRx;

public sealed class ScoreRuleEngine : IDisposable
{
    private readonly Dictionary<ScoreRuleType, IScoreRule> _rules;

    public ScoreRuleEngine(
        NewOrbitScoreRule newOrbit,
        EnemyDefeatedScoreRule enemyDefeated,
        PointObjectScoreRule pointObject)
    {
        _rules = new Dictionary<ScoreRuleType, IScoreRule>
        {
            { ScoreRuleType.NewOrbit, newOrbit },
            { ScoreRuleType.DefeatEnemy, enemyDefeated },
            { ScoreRuleType.PointObject, pointObject },
        };
    }

    public IObservable<ScoreRuleSignal> GetStream(ScoreRuleType type)
    {
        if (_rules.TryGetValue(type, out var rule))
            return rule.OnTriggered;

        // 未登録なら空ストリーム（Null回避）
        return Observable.Empty<ScoreRuleSignal>();
    }

    public void Publish(in ScoreRuleSignal signal)
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