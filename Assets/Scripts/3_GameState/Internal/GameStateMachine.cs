using System.Collections.Generic;
using UniRx;
using UnityEngine;
using System;
using VContainer.Unity;

public enum GameStateKey
{
    Opening,
    Play,
    Result
}

public class GameStateMachine : IStartable, ITickable
{
    private readonly Dictionary<GameStateKey, IGameState> _states = new();
    private IGameState _currentState;

    private readonly CompositeDisposable _disposables = new();

    public GameStateMachine(OpeningState openingState, PlayState playState, ResultState resultState)
    {
        Debug.Log("GameStateMachine Constructor");
        RegisterState(GameStateKey.Opening, openingState);
        RegisterState(GameStateKey.Play, playState);
        RegisterState(GameStateKey.Result, resultState);

    }

    public void Start()
    {
        Debug.Log("GameStateMachine Start");
        ChangeState(GameStateKey.Play);
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
        _currentState?.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }

    public void Tick()
    {
        _currentState?.Tick();
    }
}
