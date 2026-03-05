using System;
using UniRx;

public sealed class PointObjectScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.PointObject;
    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Evaluate(in ScoreEventContext ctx)
    {
        if (ctx.Type != RuleType) return;
        if (ctx.PointValue <= 0) return;

        _subject.OnNext(new ScoreRuleSignal(
            type: RuleType,
            amount: ctx.PointValue,
            worldPos: ctx.WorldPos
        ));
    }

    public void Dispose() => _subject.Dispose();
}