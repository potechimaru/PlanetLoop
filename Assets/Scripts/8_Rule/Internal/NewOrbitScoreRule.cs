using System;
using UniRx;

public sealed class NewOrbitScoreRule : IScoreRule
{
    private readonly Subject<ScoreRuleSignal> _subject = new();

    public ScoreRuleType RuleType => ScoreRuleType.NewOrbit;
    public IObservable<ScoreRuleSignal> OnTriggered => _subject;

    public void Evaluate(in ScoreEventContext ctx)
    {
        if (ctx.Type != RuleType) return;
        if (!ctx.IsFirstLanding) return;

        // 演出をルール側でやるならここ（不要なら消してOK）
        //ctx.ToSpline?.FlashLandingMaterial();

        //_subject.OnNext(new ScoreRuleSignal(
        //    type: RuleType,
        //    amount: 1,
        //    worldPos: ctx.WorldPos,
        //    context: ctx.ToSpline
        //));
    }

    public void Dispose() => _subject.Dispose();
}