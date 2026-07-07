using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

public class GameState : IAppState
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    [Inject] private SceneLoader _sceneLoader;

    public GameState()
    {
    }

    public async UniTask Enter()
    {
        _sceneLoader.LoadGameSceneAsync().Forget();

        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        Debug.Log("Exiting Endless State");
        await UniTask.CompletedTask;
    }

}
