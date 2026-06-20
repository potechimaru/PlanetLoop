using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class ResultState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();
    private readonly GameUIManager _gameUIManager;
    private readonly IGameStateExternalFacade _gameStateExternalFacade;

    public ResultState(GameUIManager gameUIManager, IGameStateExternalFacade gameStateExternalFacade)
    {
        _gameUIManager = gameUIManager;
        _gameStateExternalFacade = gameStateExternalFacade;

    }

    public async UniTask Enter()
    {
        _gameStateExternalFacade.EndGame();
        _gameStateExternalFacade.StopTimer();
        await _gameUIManager.ShowGameOver();
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
