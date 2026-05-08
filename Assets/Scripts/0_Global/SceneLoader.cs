using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class SceneLoader
{
    private readonly TitleLifetimeScope _globalScope;
    private readonly SceneLoadRequest _sceneLoadRequest;

    private const string LoadingSceneName = "LoadingScene";

    public SceneLoader(
        TitleLifetimeScope globalScope,
        SceneLoadRequest sceneLoadRequest)
    {
        _globalScope = globalScope;
        _sceneLoadRequest = sceneLoadRequest;
    }

    public async UniTask LoadGameSceneAsync(GameModeType gameModeType)
    {
        string nextSceneName = GetSceneName(gameModeType);

        _sceneLoadRequest.SetNextScene(nextSceneName);

        using (LifetimeScope.EnqueueParent(_globalScope))
        {
            await SceneManager.LoadSceneAsync(LoadingSceneName).ToUniTask();
        }
    }

    private string GetSceneName(GameModeType gameModeType)
    {
        return gameModeType switch
        {
            GameModeType.Endless => "EndlessModeScene",
            GameModeType.TimeAttack => "TimeAttackModeScene",
            GameModeType.Stage => "StageModeScene",
            _ => throw new System.ArgumentOutOfRangeException(nameof(gameModeType), gameModeType, null)
        };
    }
}