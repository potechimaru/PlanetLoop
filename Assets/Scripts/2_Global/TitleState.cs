using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class TitleState : IAppState, IDisposable
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    private readonly Subject<Unit> _onEntered = new();
    private readonly Subject<Unit> _onExited = new();

    public IObservable<Unit> OnEntered => _onEntered;
    public IObservable<Unit> OnExited => _onExited;

    private readonly AudioManager _audioManager;

    public TitleState(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async UniTask Enter()
    {
        _onEntered.OnNext(Unit.Default);
        _audioManager.PlayBGM(BGMType.Title);
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
