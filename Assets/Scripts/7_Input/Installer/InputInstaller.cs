using UnityEngine;
using VContainer;
using VContainer.Unity;

public class InputInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<InputService>(Lifetime.Singleton);
        builder.Register<InputFacade>(Lifetime.Singleton).As<IInputFacade>();
    }
}
