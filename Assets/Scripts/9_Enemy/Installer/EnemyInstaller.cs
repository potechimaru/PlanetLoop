using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Enemyコンポーネント群をDIコンテナに登録する
/// </summary>
public class EnemyInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private SingleBulletPool _singleBulletPool;
    [SerializeField] private LargeSingleBulletPool _largeSingleBulletPool;
    [SerializeField] private LaserBeamPool _laserBeamPool;
    [SerializeField] private Transform _enemyRoot;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private EnemySpawnDirector _enemySpawnDirector;

    [SerializeField] private EnemySimulationRangeController _enemySimulationRangeController;

    [SerializeField] private Transform _playerTransform;

    public void Install(IContainerBuilder builder)
    {
        builder.Register<EnemyBulletFactory>(Lifetime.Singleton);
        builder.Register<EnemyLaserFactory>(Lifetime.Singleton);

        if (_singleBulletPool != null)
        {
            builder.RegisterComponent(_singleBulletPool).WithParameter(_playerTransform);
            builder.RegisterComponent(_largeSingleBulletPool).WithParameter(_playerTransform);
        }

        builder.RegisterComponent(_laserBeamPool).WithParameter(_playerTransform);

        var enemies = _enemyRoot.GetComponentsInChildren<Enemy>();

        builder.Register<EnemyManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<Enemy>>(enemies);

        builder.Register<EnemyBulletManager>(Lifetime.Singleton);
        builder.Register<LaserBeamManager>(Lifetime.Singleton);

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