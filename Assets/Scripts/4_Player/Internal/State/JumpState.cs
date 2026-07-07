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
        _playerController.StartJump();
        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask Tick()
    {
        if (_playerController.TickJump())
        {
            NextState.Execute(PlayerStateKey.Move);
        }
        await UniTask.CompletedTask;
    }
}
