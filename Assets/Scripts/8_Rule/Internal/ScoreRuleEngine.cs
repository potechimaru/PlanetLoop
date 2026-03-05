using System;
using System.Collections.Generic;
using UniRx;

public sealed class ScoreRuleEngine : IDisposable
{
    private readonly Dictionary<ScoreRuleType, IScoreRule> _rules;

    public ScoreRuleEngine(IEnumerable<IScoreRule> rules)
    {
        _rules = new Dictionary<ScoreRuleType, IScoreRule>();
        foreach (var r in rules) _rules[r.RuleType] = r;
    }

    public IObservable<ScoreRuleSignal> GetStream(ScoreRuleType type)
        => _rules.TryGetValue(type, out var r) ? r.OnTriggered : Observable.Empty<ScoreRuleSignal>();

    public void Evaluate(in ScoreEventContext ctx)
    {
        if (_rules.TryGetValue(ctx.Type, out var r))
        {
            r.Evaluate(ctx); // Åö1å¬ÇæÇØ
        }
    }

    public void Dispose()
    {
        foreach (var r in _rules.Values) r.Dispose();
        _rules.Clear();
    }
}