using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// UIコンポーネント群をDIコンテナに登録する
/// </summary>
public class UIInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private DefeatEnemyCountView _defeatEnemyCountView;
    [SerializeField] private LongJumpedCountView _longJumpedCountView;
    [SerializeField] private ResetConditionView _resetConditionView;

    [SerializeField] private TimerView _timerView;

    [SerializeField] private NewOrbitPointPool _newOrbitPointPool;
    [SerializeField] private DefeatEnemyPointPool _enemyDefeatedPointPool;
    [SerializeField] private LongJumpPointPool _longJumpPointPool;

    [SerializeField] private VeryHighPointPool _veryHighPointPool;
    [SerializeField] private HighPointPool _highPointPool;
    [SerializeField] private MediumPointPool _mediumPointPool;
    [SerializeField] private LowPointPool _lowPointPool;

    [SerializeField] private VisitedSplineCountView _visitedSplineCountView;

    [SerializeField] private AudioVolumeSlider _bgmVolumeSlider;
    [SerializeField] private AudioVolumeSlider _seVolumeSlider;

    [SerializeField] private Transform _playerTransform;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<HUDPresenter>(Lifetime.Singleton).AsSelf().As<IDisposable>().WithParameter(_playerTransform);
        builder.RegisterComponent(_scoreView);
        builder.RegisterComponent(_defeatEnemyCountView)
            .As<IDefeatEnemyCountView>();

        builder.RegisterComponent(_visitedSplineCountView)
            .As<IVisitedSplineCountView>();

        builder.RegisterComponent(_longJumpedCountView)
            .As<ILongJumpedCountView>();

        builder.RegisterComponent(_resetConditionView)
            .As<IResetConditionView>();

        builder.RegisterComponent(_timerView);

        builder.Register<PlayUIFactory>(Lifetime.Singleton);

        // PlayUIFactory以外は直接注入しない
        builder.RegisterComponent(_newOrbitPointPool);
        builder.RegisterComponent(_enemyDefeatedPointPool);
        builder.RegisterComponent(_longJumpPointPool);

        builder.RegisterComponent(_veryHighPointPool);
        builder.RegisterComponent(_highPointPool);
        builder.RegisterComponent(_mediumPointPool);
        builder.RegisterComponent(_lowPointPool);


        builder.Register<UIFacade>(Lifetime.Singleton).As<IUIFacade>();
        builder.Register<UIExternalFacade>(Lifetime.Singleton).As<IUIExternalFacade>();

        builder.RegisterBuildCallback(container =>
        {
            // Subscribeするためにインスタンスを解決しておく
            container.Resolve<PlayUIFactory>();
            container.Resolve<HUDPresenter>();
        });

        builder.RegisterBuildCallback(container =>
        {
            container.Inject(_bgmVolumeSlider);
            container.Inject(_seVolumeSlider);
        });


    }
}

