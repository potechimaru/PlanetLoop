using System;
using UniRx;

public sealed class EnemyDefeatedScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.DefeatEnemy;

    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Trigger(in ScoreRuleSignal signal)
    {
        // Œ^‚ªˆê’v‚·‚éê‡‚Ì‚İ Publish
        if (signal.Type != RuleType)
            return;

        _subject.OnNext(signal);
    }

    public void Dispose()
    {
        _subject.Dispose();
    }
}