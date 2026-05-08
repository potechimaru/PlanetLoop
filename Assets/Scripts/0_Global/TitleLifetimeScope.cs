using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TitleLifetimeScope : LifetimeScope
{
    [SerializeField] private TitleUIManager _uiManager;
    [SerializeField] private List<GameModeDummyEntry> _gameModeDummyEntries = new();
    [SerializeField] private GameModeCarouselController _gameModeCarouselController;

    protected override void Configure(IContainerBuilder builder)
    {
        //builder.RegisterEntryPoint<AppStateMachine>(Lifetime.Singleton);
        //builder.Register<TitleState>(Lifetime.Singleton);
        //builder.Register<ModeSelectState>(Lifetime.Singleton);
        //builder.Register<EndlessModeState>(Lifetime.Singleton);

        builder.Register<SceneLoader>(Lifetime.Singleton);

        builder.RegisterComponent(_uiManager)
               .As<ITitleUIManager>();

        builder.Register<IGameModeManager>(resolver =>
        {
            return new GameModeManager(_gameModeDummyEntries);
        }, Lifetime.Singleton);

        builder.RegisterComponent(_gameModeCarouselController);
    }
}