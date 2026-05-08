using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class LoadingSceneController : MonoBehaviour
{
    [Inject] private SceneLoadRequest _sceneLoadRequest;
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

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false;

        // ここで最低表示時間を入れると画面が一瞬で消えない
        float minimumLoadingTime = 2.0f;
        float elapsed = 0f;

        while (operation.progress < 0.9f || elapsed < minimumLoadingTime)
        {
            elapsed += Time.deltaTime;

            // progressは0〜0.9まで進む
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // ここでProgressBarに反映できる
            _loadingView.SetProgress(progress);

            await UniTask.Yield();
        }

        _blackBack.gameObject.SetActive(true);
        await _blackBack.FadeInAsync();

        operation.allowSceneActivation = true;
    }
}