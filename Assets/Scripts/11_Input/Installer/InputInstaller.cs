using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Inputコンポーネント群をDIコンテナに登録する
/// </summary>
public class InputInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<InputService>(Lifetime.Singleton);
        builder.Register<InputFacade>(Lifetime.Singleton).As<IInputFacade>();
    }
}
