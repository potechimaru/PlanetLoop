using UnityEngine;
using VContainer;
using VContainer.Unity;

public class UIInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ScoreView _scoreView;

    [SerializeField] private NewOrbitPointPool _newOrbitPointPool;
    [SerializeField] private EnemyDefeatedPointPool _enemyDefeatedPointPool;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<HUDPresenter>(Lifetime.Singleton);
        builder.RegisterComponent(_scoreView);

        // PlayUIFactoryà»äOÇÕíºê⁄íçì¸ÇµÇ»Ç¢
        builder.RegisterComponent(_newOrbitPointPool);
        builder.RegisterComponent(_enemyDefeatedPointPool);

        builder.Register<UIFacade>(Lifetime.Singleton).As<IUIFacade>();
        builder.Register<UIExternalFacade>(Lifetime.Singleton).As<IUIExternalFacade>();


    }
}

