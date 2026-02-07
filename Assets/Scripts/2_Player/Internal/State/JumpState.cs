using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using System;

internal class JumpState : IPlayerState
{
    public ReactiveCommand<PlayerStateKey> NextState { get; } = new();
    private readonly PlayerController _playerController;

    internal JumpState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public async UniTask Enter()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask Tick()
    {
        await UniTask.CompletedTask;
    }
}
