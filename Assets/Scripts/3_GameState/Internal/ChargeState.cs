using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ChargeState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    private readonly IPlayerFacade _playerFacade;

    public ChargeState(IPlayerFacade playerFacade)
    {
        _playerFacade = playerFacade;
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
