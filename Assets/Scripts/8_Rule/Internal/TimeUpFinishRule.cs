using System;
using UniRx;

public sealed class TimeUpFinishRule : IGameFinishRule
{
    private readonly Subject<GameFinishSignal> _subject = new();

    public GameFinishRuleType RuleType => GameFinishRuleType.TimeLimit;
    public IObservable<GameFinishSignal> OnTriggered => _subject;

    public void Trigger(in GameFinishSignal signal)
    {
        if (signal.Type != RuleType) return;
        _subject.OnNext(signal);
    }

    public void Dispose() => _subject.Dispose();
}