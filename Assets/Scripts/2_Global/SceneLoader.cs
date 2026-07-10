using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VContainer.Unity;

public class SceneLoader
{
    private readonly RootLifetimeScope _globalScope;
    private readonly SceneLoadRequest _sceneLoadRequest;
    private readonly IGameModeSelectionReader _gameModeSelectionReader;
    private readonly IAppStateChangeRequester _appStateChangeRequester;

    private const string LoadingSceneName = "LoadingScene";

    public SceneLoader(
        RootLifetimeScope globalScope,
        SceneLoadRequest sceneLoadRequest,
        IGameModeSelectionReader gameModeSelectionReader,
        IAppStateChangeRequester appStateChangeRequester)
    {
        _globalScope = globalScope;
        _sceneLoadRequest = sceneLoadRequest;
        _gameModeSelectionReader = gameModeSelectionReader;
        _appStateChangeRequester = appStateChangeRequester;
    }

    public async UniTask LoadGameSceneAsync()
    {
        string nextSceneName = GetSceneName(_gameModeSelectionReader.CurrentSelectedMode);

        _sceneLoadRequest.SetNextScene(nextSceneName);

        using (LifetimeScope.EnqueueParent(_globalScope))
        {
            await SceneManager.LoadSceneAsync(LoadingSceneName).ToUniTask();
        }
    }

    public async UniTask LoadTitleSceneAsync()
    {
        _sceneLoadRequest.SetNextScene("Title•SelectScene");

        using (LifetimeScope.EnqueueParent(_globalScope))
        {
            await SceneManager.LoadSceneAsync(LoadingSceneName).ToUniTask();
        }

    }

    public async UniTask RetryGameAsync()
    {
        string currentSceneName = GetSceneName(_gameModeSelectionReader.CurrentSelectedMode);
        _sceneLoadRequest.SetNextScene(currentSceneName, true);
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