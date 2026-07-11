using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// Obstacle?R???|?[?l???g?Q??DI?R???e?i??o?^????
/// </summary>
public class ObstacleInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Transform _linesRoot;
    public void Install(IContainerBuilder builder)
    {

        var obstacle = _linesRoot.GetComponentsInChildren<Obstacle>();

        builder.Register<ObstacleManager>(Lifetime.Singleton)
               .AsSelf()
               .As<IDisposable>()
               .WithParameter<IEnumerable<Obstacle>>(obstacle);

        builder.Register<ObstacleFacade>(Lifetime.Singleton).As<IObstacleFacade>();
        builder.Register<ObstacleExternalFacade>(Lifetime.Singleton).As<IObstacleExternalFacade>();
    }
}
