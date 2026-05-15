using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class ResultState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();
    private readonly GameUIManager _gameUIManager;

    public ResultState(GameUIManager gameUIManager)
    {
        _gameUIManager = gameUIManager;

    }

    public async UniTask Enter()
    {
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
