using System;
using UniRx;

public sealed class NewOrbitScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.NewOrbit;
    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Trigger(in ScoreRuleSignal signal)
    {
        // 型ガード（誤通知対策）
        if (signal.Type != RuleType) return;
        _subject.OnNext(signal);
    }

    public void Dispose() => _subject.Dispose();
}