using System;
using UniRx;

public interface IGameFinishRule : IDisposable
{
    GameFinishRuleType RuleType { get; }
    IObservable<GameFinishSignal> OnTriggered { get; }
    void Trigger(in GameFinishSignal signal);
}