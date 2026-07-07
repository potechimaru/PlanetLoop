using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// 全てのコンポーネント群を束ねるライフタイムスコープ
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameStateInstaller _gameStateInstaller;
    [SerializeField] private PlayerInstaller _playerInstaller;
    [SerializeField] private OrbitInstaller _orbitInstaller;
    [SerializeField] private UIInstaller _uiInstaller;
    [SerializeField] private InputInstaller _inputInstaller;
    [SerializeField] private BlackHoleInstaller _blachHoleInstaller;
    [SerializeField] private PointObjectInstaller _pointObjectInstaller;
    [SerializeField] private EnemyInstaller _enemyInstaller;
    [SerializeField] private ObstacleInstaller _obstacleInstaller;

    protected override void Configure(IContainerBuilder builder)
    {
        _gameStateInstaller.Install(builder);
        _playerInstaller.Install(builder);
        _orbitInstaller.Install(builder);
        _uiInstaller.Install(builder);
        _inputInstaller.Install(builder);
        _blachHoleInstaller.Install(builder);
        _pointObjectInstaller.Install(builder);
        _enemyInstaller.Install(builder);
        _obstacleInstaller.Install(builder);


    }
}
