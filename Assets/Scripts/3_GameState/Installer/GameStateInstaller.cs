using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameStateInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameStateMachine>(Lifetime.Singleton);
        builder.Register<OpeningState>(Lifetime.Singleton);
        builder.Register<PlayState>(Lifetime.Singleton);
        builder.Register<ResultState>(Lifetime.Singleton);

    }
}
