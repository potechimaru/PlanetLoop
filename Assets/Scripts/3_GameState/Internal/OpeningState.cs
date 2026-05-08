using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class OpeningState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();
    private readonly IGameStateExternalFacade _gameStateExternalFacade;
    private readonly GameUIManager _gameUIManager;

    public OpeningState(IGameStateExternalFacade gameStateExternalFacade, GameUIManager gameUIManager)
    {
        _gameStateExternalFacade = gameStateExternalFacade;
        _gameUIManager = gameUIManager;
    }

    public async UniTask Enter()
    {
        _gameStateExternalFacade.RegisterPlayerSubscriptions();
        _gameStateExternalFacade.SetPlayer();
        Time.timeScale = 0f;
        await _gameUIManager.GameOpening();
        await _gameUIManager.ShowPreGame();
    }
    public async UniTask Exit()
    {
        Time.timeScale = 1f;
        Debug.Log("Exiting Opening State");
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        // Logic for the opening state
        await UniTask.CompletedTask;
    }
}
