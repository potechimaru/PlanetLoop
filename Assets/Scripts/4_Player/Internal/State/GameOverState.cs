using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using System;

/// <summary>
/// PlayerStateのGameOver状態を表すクラス。プレイヤーが死亡した状態を管理する。
/// </summary>
internal class GameOverState : IPlayerState
{
    public ReactiveCommand<PlayerStateKey> NextState { get; } = new();
    private readonly PlayerController _playerController;

    internal GameOverState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public async UniTask Enter()
    {
        _playerController.Dead();
        
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
