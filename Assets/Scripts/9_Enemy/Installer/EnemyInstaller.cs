using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EnemyInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SingleBulletPool _singleBulletPool;
    [SerializeField] private Transform _enemyRoot;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<EnemyBulletFactory>(Lifetime.Singleton);

        if (_singleBulletPool != null)
        {
            builder.RegisterComponent(_singleBulletPool);
        }

        var enemies = _enemyRoot.GetComponentsInChildren<Enemy>();

        builder.Register<EnemyManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<Enemy>>(enemies);

        builder.Register<EnemyBulletManager>(Lifetime.Singleton);

        builder.Register<EnemyFacade>(Lifetime.Singleton).As<IEnemyFacade>();
        builder.Register<EnemyExternalFacade>(Lifetime.Singleton).As<IEnemyExternalFacade>();

        builder.RegisterBuildCallback(container =>
        {
            // EnemyManagerでEnemyをサーチするため、ここで一度Resolveしておく
            container.Resolve<EnemyManager>();
        });
    }
}