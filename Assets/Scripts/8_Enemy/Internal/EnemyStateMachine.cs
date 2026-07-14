using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

/// <summary>
/// EnemyStateを表すインターフェース。敵の状態遷移を管理するためのメソッドを定義する。
/// </summary>
public interface IEnemyState
{
    ReactiveCommand<EnemyStateKey> NextState { get; }

    UniTask Enter();
    UniTask Exit();

    void Tick();
}
/// <summary>
/// EnemyStateの状態を管理するステートマシン。敵の状態遷移を管理するためのクラス。
/// </summary>
public sealed class EnemyStateMachine : IDisposable
{
    private readonly Dictionary<EnemyStateKey, IEnemyState> _states = new();
    public IEnemyState CurrentState { get; private set; }

    private readonly CompositeDisposable _disposables = new();


    public void RegisterState(EnemyStateKey key, IEnemyState state)
    {
        _states[key] = state;

        state.NextState
            .Subscribe(k => ChangeStateAsync(k).Forget())
            .AddTo(_disposables);
    }

    public async UniTask ChangeStateAsync(EnemyStateKey key)
    {
        if (!_states.TryGetValue(key, out var next))
            throw new Exception($"State not registered {key}");

        if (ReferenceEquals(CurrentState, next))
            return;

        if (CurrentState != null)
            await CurrentState.Exit();

        CurrentState = next;

        await CurrentState.Enter();
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