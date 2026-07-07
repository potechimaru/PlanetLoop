using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class TitleState : IAppState
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    private readonly Subject<Unit> _onEntered = new();
    private readonly Subject<Unit> _onExited = new();

    public IObservable<Unit> OnEntered => _onEntered;
    public IObservable<Unit> OnExited => _onExited;

    public TitleState()
    {
    }

    public async UniTask Enter()
    {
        _onEntered.OnNext(Unit.Default);
        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        _onExited.OnNext(Unit.Default);
        await UniTask.CompletedTask;
    }

}
