using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

/// <summary>
/// AppStateの中でゲームプレイ中の状態を表すクラス。
/// </summary>
public class GameState : IAppState
{
    public ReactiveCommand<AppStateKey> NextState { get; } = new();

    [Inject] private SceneLoader _sceneLoader;

    public GameState()
    {
    }

    public async UniTask Enter()
    {
        // ゲームシーンを非同期でロードする
        _sceneLoader.LoadGameSceneAsync().Forget();

        await UniTask.CompletedTask;
    }

    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }

}
