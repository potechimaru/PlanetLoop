using System;
using UniRx;

public interface IScoreRule : IDisposable
{
    ScoreRuleType RuleType { get; }
    IObservable<ScoreRuleSignal> OnTriggered { get; }
    void Evaluate(in ScoreEventContext ctx);
}