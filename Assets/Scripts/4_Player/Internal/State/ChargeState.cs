using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// PlayerStateのCharge状態を表すクラス。プレイヤーがチャージ中の状態を管理する。
/// </summary>
public class ChargeState : IPlayerState
{
    public ReactiveCommand<PlayerStateKey> NextState { get; } = new();

    private readonly PlayerController _playerController;

    public ChargeState(PlayerController playerController)
    {
        _playerController = playerController;
    }

    public async UniTask Enter()
    {
        _playerController.StartCharge();
        await UniTask.CompletedTask;
    }
    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        _playerController.TickMove();
        _playerController.TickCharge();
        await UniTask.CompletedTask;
    }
}
