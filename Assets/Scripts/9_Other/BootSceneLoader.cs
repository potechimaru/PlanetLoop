using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

/// <summary>
/// ゲーム起動時に最初に読み込まれるシーンで、初期シーンを非同期で読み込むクラス
/// </summary>
public class BootSceneLoader : MonoBehaviour
{
    [SerializeField] private RootLifetimeScope _rootLifetimeScope;

    private async void Start()
    {
        using (LifetimeScope.EnqueueParent(_rootLifetimeScope))
        {
            await SceneManager.LoadSceneAsync("Title＆SelectScene").ToUniTask();
        }
    }
}