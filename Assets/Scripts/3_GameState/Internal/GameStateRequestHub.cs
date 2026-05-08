using System;
using UniRx;

public interface IGameStateChangeRequester
{
    void Request(GameStateKey key);
}

public interface IGameStateChangeRequestSource
{
    IObservable<GameStateKey> OnRequestChangeState { get; }
}

public class GameStateRequestHub :
    IGameStateChangeRequester,
    IGameStateChangeRequestSource,
    IDisposable
{
    private readonly Subject<GameStateKey> _onRequestChangeState = new();

    public IObservable<GameStateKey> OnRequestChangeState
        => _onRequestChangeState;

    public void Request(GameStateKey key)
    {
        _onRequestChangeState.OnNext(key);
    }

    public void Dispose()
    {
        _onRequestChangeState.OnCompleted();
        _onRequestChangeState.Dispose();
    }
}