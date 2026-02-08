using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;

internal class MoveState : IPlayerState
{
    public ReactiveCommand<PlayerStateKey> NextState { get; } = new();
    private readonly PlayerController _playerController;
    internal MoveState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public async UniTask Enter()
    {
        _playerController.StartMove();
        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }

    public async UniTask Tick()
    {
        _playerController.TickMove();
        await UniTask.CompletedTask;
    }
}
