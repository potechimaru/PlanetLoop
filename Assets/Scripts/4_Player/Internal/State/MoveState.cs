using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// PlayerStateのMove状態を表すクラス。プレイヤーが移動中の状態を管理する。
/// </summary>
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
