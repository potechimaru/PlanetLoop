using System;
using UniRx;

public sealed class EnemyDefeatedScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.DefeatEnemy;
    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Evaluate(in ScoreEventContext ctx)
    {
        if (ctx.Type != RuleType) return;
        if (ctx.EnemyId == 0) return; // 0‚Í–³Œøˆµ‚¢i‰^—p‚Å•Ï‚¦‚ÄOKj

        int amount = ctx.WasCharged ? 3 : 1;

        _subject.OnNext(new ScoreRuleSignal(
            type: RuleType,
            amount: amount,
            worldPos: ctx.WorldPos,
            context: ctx.EnemyId
        ));
    }

    public void Dispose() => _subject.Dispose();
}