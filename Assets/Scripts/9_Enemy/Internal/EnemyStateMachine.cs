using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

public interface IEnemyState
{
    ReactiveCommand<EnemyStateKey> NextState { get; }

    UniTask Enter();
    UniTask Exit();

    void Tick();
}
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