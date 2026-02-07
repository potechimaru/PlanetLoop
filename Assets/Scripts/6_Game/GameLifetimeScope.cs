using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private GameStateInstaller _gameStateInstaller;
    [SerializeField] private PlayerInstaller _playerInstaller;
    [SerializeField] private OrbitInstaller _orbitInstaller;
    [SerializeField] private UIInstaller _uiInstaller;
    [SerializeField] private InputInstaller _inputInstaller;

    protected override void Configure(IContainerBuilder builder)
    {
        _gameStateInstaller.Install(builder);
        _playerInstaller.Install(builder);
        _orbitInstaller.Install(builder);
        _uiInstaller.Install(builder);
        _inputInstaller.Install(builder);





    }
}
