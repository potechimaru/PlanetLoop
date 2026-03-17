using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UIInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ScoreView _scoreView;
    [SerializeField] private DefeatEnemyCountView _defeatEnemyCountView;
    [SerializeField] private LongJumpedCountView _longJumpedCountView;

    [SerializeField] private NewOrbitPointPool _newOrbitPointPool;
    [SerializeField] private EnemyDefeatedPointPool _enemyDefeatedPointPool;
    [SerializeField] private VisitedSplineCountView _visitedSplineCountView;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<HUDPresenter>(Lifetime.Singleton);
        builder.RegisterComponent(_scoreView);
        builder.RegisterComponent(_defeatEnemyCountView);
        builder.RegisterComponent(_longJumpedCountView);
        builder.RegisterComponent(_visitedSplineCountView);

        builder.Register<PlayUIFactory>(Lifetime.Singleton);

        // PlayUIFactory以外は直接注入しない
        builder.RegisterComponent(_newOrbitPointPool);
        builder.RegisterComponent(_enemyDefeatedPointPool);

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

