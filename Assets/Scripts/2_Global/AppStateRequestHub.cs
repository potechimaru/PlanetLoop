using System;
using UniRx;

public interface IAppStateChangeRequester
{
    void Request(AppStateKey key);
}

public interface IAppStateChangeRequestSource
{
    IObservable<AppStateKey> OnRequestChangeState { get; }
}

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