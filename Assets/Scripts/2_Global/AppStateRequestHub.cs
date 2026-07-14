using System;
using UniRx;

/// <summary>
/// 外部からアプリケーションの状態変更をリクエストするためのインターフェース
/// </summary>
public interface IAppStateChangeRequester
{
    void Request(AppStateKey key);
}

/// <summary>
/// ステートマシンが状態変更リクエストを受け取るためのインターフェース
/// </summary>
public interface IAppStateChangeRequestSource
{
    IObservable<AppStateKey> OnRequestChangeState { get; }
}

/// <summary>
/// 外部コンポーネントがアプリケーションの状態変更をリクエストするためのハブ
/// </summary>
public class AppStateRequestHub : IAppStateChangeRequester, IAppStateChangeRequestSource, IDisposable
{
    private readonly Subject<AppStateKey> _onRequestChangeState = new();

    public IObservable<AppStateKey> OnRequestChangeState => _onRequestChangeState;

    public void Request(AppStateKey key)
    {
        _onRequestChangeState.OnNext(key);
    }

    public void Dispose()
    {
        _onRequestChangeState.OnCompleted();
        _onRequestChangeState.Dispose();
    }
}