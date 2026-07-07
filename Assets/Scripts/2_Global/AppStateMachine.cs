using System;
using System.Collections.Generic;
using UniRx;
using VContainer.Unity;

public enum AppStateKey
{
    Title,
    ModeSelectState,
    Game,
}

public interface IAppStateChangeNotifier
{
    IObservable<AppStateKey> OnRequestChangeState { get; }
}

public class AppStateMachine : IStartable, IDisposable
{
    private readonly Dictionary<AppStateKey, IAppState> _states = new();
    private IAppState _currentState;

    private readonly CompositeDisposable _disposables = new();

    public AppStateMachine(
        TitleState titleState,
        ModeSelectState modeSelectState,
        GameState gameState,
        IAppStateChangeRequestSource requestSource)
    {
        RegisterState(AppStateKey.Title, titleState);
        RegisterState(AppStateKey.ModeSelectState, modeSelectState);
        RegisterState(AppStateKey.Game, gameState);

        requestSource.OnRequestChangeState
            .Subscribe(ChangeState)
            .AddTo(_disposables);
    }

    public void Start()
    {
        ChangeState(AppStateKey.Title);
    }

    private void RegisterState(AppStateKey key, IAppState state)
    {
        _states[key] = state;

        state.NextState
            .Subscribe(ChangeState)
            .AddTo(_disposables);
    }

    public void ChangeState(AppStateKey key)
    {
        if (_states.TryGetValue(key, out var nextState) == false)
        {
            return;
        }

        if (_currentState == nextState) return;

        _currentState?.Exit();
        _currentState = nextState;
        _currentState.Enter();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}