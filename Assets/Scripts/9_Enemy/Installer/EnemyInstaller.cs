using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EnemyInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SingleBulletPool _singleBulletPool;
    [SerializeField] private Transform _enemyRoot;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private EnemySpawnDirector _enemySpawnDirector;

    [SerializeField] private EnemySimulationRangeController _enemySimulationRangeController;

    [SerializeField] private Transform _playerTransform;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<EnemyBulletFactory>(Lifetime.Singleton);

        if (_singleBulletPool != null)
        {
            builder.RegisterComponent(_singleBulletPool).WithParameter(_playerTransform);
        }

        var enemies = _enemyRoot.GetComponentsInChildren<Enemy>();

        builder.Register<EnemyManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<Enemy>>(enemies);

        builder.Register<EnemyBulletManager>(Lifetime.Singleton);

        builder.Register<EnemyFacade>(Lifetime.Singleton).As<IEnemyFacade>();
        builder.Register<EnemyExternalFacade>(Lifetime.Singleton).As<IEnemyExternalFacade>();

        builder.Register<EnemyFactory>(Lifetime.Singleton);
        builder.RegisterComponent(_enemyPool).WithParameter(_playerTransform);

        builder.RegisterComponent(_enemySpawnDirector);

        builder.RegisterComponent(_enemySimulationRangeController);

        builder.RegisterBuildCallback(container =>
        {
            // EnemyManagerでEnemyをサーチするため、ここで一度Resolveしておく
            container.Resolve<EnemyManager>();
        });
    }
}