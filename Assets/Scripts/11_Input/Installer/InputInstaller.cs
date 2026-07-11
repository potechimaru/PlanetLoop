using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// Input?R???|?[?l???g?Q??DI?R???e?i??o?^????
/// </summary>
public class InputInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<InputService>(Lifetime.Singleton).AsSelf().As<IDisposable>();
        builder.Register<InputFacade>(Lifetime.Singleton).AsSelf().As<IInputFacade>().As<IDisposable>();
    }
}
