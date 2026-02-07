using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class ResultState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    public ResultState()
    {

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
