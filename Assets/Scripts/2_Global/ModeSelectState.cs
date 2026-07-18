using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

/// <summary>
/// ・ｽQ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽh・ｽI・ｽ・ｽ・ｽ・ｽﾊの擾ｿｽﾔゑｿｽ\・ｽ・ｽ・ｽBAppState・ｽ・ｽ1・ｽﾂ。
/// </summary>
public class ModeSelectState : IAppState, IDisposable
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    private readonly Subject<Unit> _onEntered = new();
    private readonly Subject<Unit> _onExited = new();

    public IObservable<Unit> OnEntered => _onEntered;
    public IObservable<Unit> OnExited => _onExited;

    public ModeSelectState()
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

    public void Dispose()
    {
        _onEntered.Dispose();
        _onExited.Dispose();
    }
}
