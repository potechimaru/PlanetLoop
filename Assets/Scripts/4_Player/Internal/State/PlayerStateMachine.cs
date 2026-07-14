using System;
using System.Collections.Generic;
using System.Data;

using UniRx;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// PlayerStateの状態を表す列挙型。プレイヤーの状態遷移を管理するためのキーを定義する。
/// </summary>
public enum PlayerStateKey
{
    Idle,
    Move,
    Charge,
    Jump,
    GameOver
}

/// <summary>
///プレイヤーの状態遷移を管理するステートマシンを表す。各状態はIPlayerStateインターフェースを実装し、状態遷移のロジックを提供する。
/// </summary>
public class PlayerStateMachine : IDisposable
{
    private readonly Dictionary<PlayerStateKey, IPlayerState> _states = new();
    internal IPlayerState CurrentState { get; private set; }

    private readonly CompositeDisposable _disposables = new();

    private PlayerController _playerController;


    public PlayerStateMachine(PlayerController playerController)
    {
        _playerController = playerController;

        RegisterState(PlayerStateKey.Idle, new IdleState(_playerController));
        RegisterState(PlayerStateKey.Move, new MoveState(_playerController));
        RegisterState(PlayerStateKey.Jump, new JumpState(_playerController));
        RegisterState(PlayerStateKey.Charge, new ChargeState(_playerController));
        RegisterState(PlayerStateKey.GameOver, new GameOverState(_playerController));


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
        CurrentState?.Exit();
        CurrentState = _states[key];
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState?.Tick();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

}
