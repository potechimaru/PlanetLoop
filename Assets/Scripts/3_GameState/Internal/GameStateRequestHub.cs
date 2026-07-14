using System;
using UniRx;

/// <summary>
/// 外部コンポーネント群からゲームの状態変更をリクエストするためのインターフェース
/// </summary>
public interface IGameStateChangeRequester
{
    void Request(GameStateKey key);
}

/// <summary>
/// コンポネント群内部でゲームの状態変更リクエストを受け取るためのインターフェース
/// </summary>
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