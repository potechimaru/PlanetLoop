using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class EndlessModeState : IAppState
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    public EndlessModeState()
    {
    }

    public async UniTask Enter()
    {
        Debug.Log("Entering Endless State");
        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        Debug.Log("Exiting Endless State");
        await UniTask.CompletedTask;
    }

}
