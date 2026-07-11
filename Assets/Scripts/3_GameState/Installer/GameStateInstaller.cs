using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// GameState?R???|?[?l???g?Q??DI?R???e?i??o?^????
/// </summary>
public class GameStateInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private GameUIManager _gameUIManager;
    public void Install(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<GameStateMachine>(Lifetime.Singleton);
        builder.Register<OpeningState>(Lifetime.Singleton);
        builder.Register<PlayState>(Lifetime.Singleton);
        builder.Register<ResultState>(Lifetime.Singleton);

        builder.Register<GameStateExternalFacade>(Lifetime.Singleton).As<IGameStateExternalFacade>();
        builder.Register<GameStateFacade>(Lifetime.Singleton).As<IGameStateFacade>();

        builder.Register<GameStateRequestHub>(Lifetime.Singleton)
            .AsSelf()
            .As<IGameStateChangeRequester>()
            .As<IGameStateChangeRequestSource>()
            .As<IDisposable>();

        builder.RegisterComponent(_gameUIManager);

    }
}
