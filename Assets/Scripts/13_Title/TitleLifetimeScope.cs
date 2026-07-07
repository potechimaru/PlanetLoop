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
        Debug.Log("[TitleLifetimeScope] Configure called", this);

        builder.RegisterComponent(_uiManager)
               .As<ITitleUIManager>();

        builder.Register<GameModeManager>(Lifetime.Singleton).As<IGameModeManager>()
               .WithParameter<IEnumerable<GameModeDummyEntry>>(_gameModeDummyEntries);

        builder.RegisterComponent(_gameModeCarouselController);

        builder.RegisterEntryPoint<GameModeSelectionPresenter>(Lifetime.Singleton);
    }
}