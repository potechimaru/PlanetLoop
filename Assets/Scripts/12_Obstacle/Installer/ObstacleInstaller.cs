using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class ObstacleInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Transform _linesRoot;
    public void Install(IContainerBuilder builder)
    {

        var obstacle = _linesRoot.GetComponentsInChildren<Obstacle>();

        builder.Register<ObstacleManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<Obstacle>>(obstacle);

        builder.Register<ObstacleFacade>(Lifetime.Singleton).As<IObstacleFacade>();
        builder.Register<ObstacleExternalFacade>(Lifetime.Singleton).As<IObstacleExternalFacade>();
    }
}
