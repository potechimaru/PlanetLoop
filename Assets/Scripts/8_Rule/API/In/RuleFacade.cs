using System;
using UniRx;

public interface IRuleFacade
{
    IObservable<ScoreRuleSignal> ObserveScore(ScoreRuleType type);
    IObservable<GameFinishSignal> ObserveFinish(GameFinishRuleType type);
}

public sealed class RuleFacade : IRuleFacade
{
    private readonly ScoreRuleEngine _scoreEngine;
    private readonly GameFinishRuleEngine _finishEngine;

    public RuleFacade(ScoreRuleEngine scoreEngine, GameFinishRuleEngine finishEngine)
    {
        _scoreEngine = scoreEngine;
        _finishEngine = finishEngine;
    }

    public IObservable<ScoreRuleSignal> ObserveScore(ScoreRuleType type)
        => _scoreEngine.GetStream(type);

    public IObservable<GameFinishSignal> ObserveFinish(GameFinishRuleType type)
        => _finishEngine.GetStream(type);
}