using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class PlayState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    private readonly IGameStateExternalFacade _gameStateExternalFacade;

    public PlayState(IGameStateExternalFacade gameStateExternalFacade)
    {
        _gameStateExternalFacade = gameStateExternalFacade;
    }

    public async UniTask Enter()
    {
        _gameStateExternalFacade.RegisterInputSubscriptions();
        _gameStateExternalFacade.StartMove();
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
