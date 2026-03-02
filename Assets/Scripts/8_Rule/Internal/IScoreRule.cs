using System;
using UniRx;

public interface IScoreRule : IDisposable
{
    ScoreRuleType RuleType { get; }
    IObservable<ScoreRuleSignal> OnTriggered { get; }
    void Trigger(in ScoreRuleSignal signal);
}