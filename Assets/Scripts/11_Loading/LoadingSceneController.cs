using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class LoadingSceneController : MonoBehaviour
{
    [Inject] private SceneLoadRequest _sceneLoadRequest;
    [Inject] private RootLifetimeScope _rootLifetimeScope;
    [Inject] private IAppStateChangeRequester _appStateChangeRequester;

    [SerializeField] private LoadingProgressView _loadingView;
    [SerializeField] private CanvasGroupFader _blackBack;

    private async void Start()
    {
        await LoadAsync();
    }

    private async UniTask LoadAsync()
    {
        string nextSceneName = _sceneLoadRequest.NextSceneName;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("NextSceneName が設定されていません。");
            return;
        }

        AsyncOperation operation;

        using (LifetimeScope.EnqueueParent(_rootLifetimeScope))
        {
            operation = SceneManager.LoadSceneAsync(nextSceneName);
            operation.allowSceneActivation = false;

            float minimumLoadingTime = 2.0f;
            float elapsed = 0f;

            while (operation.progress < 0.9f || elapsed < minimumLoadingTime)
            {
                elapsed += Time.deltaTime;

                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                _loadingView.SetProgress(progress);

                await UniTask.Yield();
            }

            _blackBack.gameObject.SetActive(true);
            await _blackBack.FadeInAsync();

            operation.allowSceneActivation = true;

            await UniTask.WaitUntil(() => operation.isDone);
        }

        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

        if (nextSceneName == "Title＆SelectScene")
        {
            _appStateChangeRequester.Request(AppStateKey.Title);
        }
        else
        {
            _appStateChangeRequester.Request(AppStateKey.Game);
        }
    }
}