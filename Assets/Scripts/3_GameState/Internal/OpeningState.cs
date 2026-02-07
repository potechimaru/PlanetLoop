using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class OpeningState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    public OpeningState()
    {

    }

    public async UniTask Enter()
    {
        Debug.Log("Entering Opening State");
        await UniTask.CompletedTask;
    }
    public async UniTask Exit()
    {
        Debug.Log("Exiting Opening State");
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        // Logic for the opening state
        await UniTask.CompletedTask;
    }
}
