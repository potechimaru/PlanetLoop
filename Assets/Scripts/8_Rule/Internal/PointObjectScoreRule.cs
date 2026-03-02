using System;
using UniRx;

public sealed class PointObjectScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.PointObject;

    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Trigger(in ScoreRuleSignal signal)
    {
        if (signal.Type != RuleType)
            return;

        _subject.OnNext(signal);
    }

    public void Dispose()
    {
        _subject.Dispose();
    }
}