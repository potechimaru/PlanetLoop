using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

public class BootSceneLoader : MonoBehaviour
{
    [SerializeField] private RootLifetimeScope _rootLifetimeScope;

    private async void Start()
    {
        using (LifetimeScope.EnqueueParent(_rootLifetimeScope))
        {
            await SceneManager.LoadSceneAsync("TitleÅïSelectScene").ToUniTask();
        }
    }
}