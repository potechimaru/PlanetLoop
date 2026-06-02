using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UIInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private DefeatEnemyCountView _defeatEnemyCountView;
    [SerializeField] private LongJumpedCountView _longJumpedCountView;

    [SerializeField] private NewOrbitPointPool _newOrbitPointPool;
    [SerializeField] private DefeatEnemyPointPool _enemyDefeatedPointPool;
    [SerializeField] private LongJumpPointPool _longJumpPointPool;

    [SerializeField] private VeryHighPointPool _veryHighPointPool;
    [SerializeField] private HighPointPool _highPointPool;
    [SerializeField] private MediumPointPool _mediumPointPool;
    [SerializeField] private LowPointPool _lowPointPool;

    [SerializeField] private VisitedSplineCountView _visitedSplineCountView;

    [SerializeField] private Transform _playerTransform;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<HUDPresenter>(Lifetime.Singleton).WithParameter(_playerTransform);
        builder.RegisterComponent(_scoreView);
        builder.RegisterComponent(_defeatEnemyCountView);
        builder.RegisterComponent(_longJumpedCountView);
        builder.RegisterComponent(_visitedSplineCountView);

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


    }
}

