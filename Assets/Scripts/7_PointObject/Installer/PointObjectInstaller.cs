using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

/// <summary>
/// PointObject?R???|?[?l???g?Q??DI?R???e?i??o?^????
/// </summary>
public class PointObjectInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Transform _linesRoot;
    public void Install(IContainerBuilder builder)
    {

        var pointObject = _linesRoot.GetComponentsInChildren<PointObject>();

        builder.Register<PointObjectManager>(Lifetime.Singleton)
               .AsSelf()
               .As<IDisposable>()
               .WithParameter<IEnumerable<PointObject>>(pointObject);

        builder.Register<PointObjectFacade>(Lifetime.Singleton).As<IPointObjectFacade>();
        builder.Register<PointObjectExternalFacade>(Lifetime.Singleton).As<IPointObjectExternalFacade>();
    }
}
