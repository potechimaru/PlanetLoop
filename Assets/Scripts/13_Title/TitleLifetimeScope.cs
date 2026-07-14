using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// タイトルシーンのLifetimeScopeを表すクラス。タイトルシーンの依存関係を管理する。
/// </summary>
public class TitleLifetimeScope : LifetimeScope
{
    [SerializeField] private TitleUIManager _uiManager;
    [SerializeField] private List<GameModeDummyEntry> _gameModeDummyEntries = new();
    [SerializeField] private GameModeCarouselController _gameModeCarouselController;

    [SerializeField] private OKButton _okButton;

    [SerializeField] private AudioVolumeSlider _bgmVolumeSlider;
    [SerializeField] private AudioVolumeSlider _seVolumeSlider;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_uiManager)
               .As<ITitleUIManager>();

        builder.Register<GameModeManager>(Lifetime.Singleton)
               .AsSelf()
               .As<IGameModeManager>()
               .As<IDisposable>()
               .WithParameter<IEnumerable<GameModeDummyEntry>>(_gameModeDummyEntries);

        builder.RegisterComponent(_gameModeCarouselController);

        builder.RegisterEntryPoint<GameModeSelectionPresenter>(Lifetime.Singleton);

        builder.RegisterBuildCallback(container =>
        {
            container.Inject(_bgmVolumeSlider);
            container.Inject(_seVolumeSlider);
        });
    }
}