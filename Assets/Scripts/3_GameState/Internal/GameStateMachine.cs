using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class GameStateMachine : IStartable, ITickable, IDisposable
{
    private readonly Dictionary<GameStateKey, IGameState> _states = new();
    private readonly CompositeDisposable _disposables = new();

    private IGameState _currentState;

    public GameStateMachine(
        OpeningState openingState,
        PlayState playState,
        ResultState resultState,
        IGameStateChangeRequestSource requestSource,
        IGameStateExternalFacade gameStateExternalFacade)
    {
        //Debug.Log("GameStateMachine Constructor");

        RegisterState(GameStateKey.Opening, openingState);
        RegisterState(GameStateKey.Play, playState);
        RegisterState(GameStateKey.Result, resultState);

        requestSource.OnRequestChangeState
            .Subscribe(ChangeState)
            .AddTo(_disposables);

        gameStateExternalFacade.OnPlayerDead
            .Subscribe(_ => ChangeState(GameStateKey.Result))
            .AddTo(_disposables);

    }

    public void Start()
    {
        //Debug.Log("GameStateMachine Start");
        ChangeState(GameStateKey.Opening);
    }

    private void RegisterState(GameStateKey key, IGameState state)
    {
        _states[key] = state;

        state.NextState
            .Subscribe(ChangeState)
            .AddTo(_disposables);
    }

    public void ChangeState(GameStateKey key)
    {
        if (!_states.TryGetValue(key, out var nextState))
        {
            Debug.LogWarning($"[GameStateMachine] Unknown state: {key}");
            return;
        }

        if (_currentState == nextState) return;

        _currentState?.Exit();

        _currentState = nextState;
        _currentState.Enter();
    }

    public void Tick()
    {
        _currentState?.Tick();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}