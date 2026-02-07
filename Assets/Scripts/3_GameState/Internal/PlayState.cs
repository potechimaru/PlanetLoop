using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class PlayState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    private readonly IPlayerFacade _playerFacade;

    public PlayState(IPlayerFacade playerFacade)
    {
        _playerFacade = playerFacade;
    }

    public async UniTask Enter()
    {
        Debug.Log("Enter Play State");
        _playerFacade.StartMove();
        await UniTask.CompletedTask;
    }
    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        // Logic for the opening state
        await UniTask.CompletedTask;
    }
}
