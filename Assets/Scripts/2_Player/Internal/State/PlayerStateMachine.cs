using System;
using System.Collections.Generic;
using System.Data;

using UniRx;
using UnityEngine;
using VContainer.Unity;

public enum PlayerStateKey
{
    Idle,
    Move,
    Jump
}

public class PlayerStateMachine : IDisposable
{
    private readonly Dictionary<PlayerStateKey, IPlayerState> _states = new();
    private IPlayerState _currentState;

    private readonly CompositeDisposable _disposables = new();

    private PlayerController _playerController;

    public PlayerStateMachine(PlayerController playerController)
    {
        _playerController = playerController;

        RegisterState(PlayerStateKey.Idle, new IdleState(_playerController));
        RegisterState(PlayerStateKey.Move, new MoveState(_playerController));
        RegisterState(PlayerStateKey.Jump, new JumpState(_playerController));

        ChangeState(PlayerStateKey.Idle);
        _playerController.SetPlayerStateMachine(this);
    }

    public void StartMove()
    {
        ChangeState(PlayerStateKey.Move);
    }

    private void RegisterState(PlayerStateKey key, IPlayerState state)
    {
        _states[key] = state;

        state.NextState
            .Subscribe(ChangeState)
            .AddTo(_disposables);
    }

    public void ChangeState(PlayerStateKey key)
    {
        _currentState?.Exit();
        _currentState = _states[key];
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
