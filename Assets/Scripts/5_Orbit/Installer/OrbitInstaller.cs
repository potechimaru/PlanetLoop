using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Orbitコンポーネント群をDIコンテナに登録する
/// </summary>
public class OrbitInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Transform _linesRoot;
    [SerializeField] private OrbitPointRotationRangeController _orbitPointRotationRangeController;
    public void Install(IContainerBuilder builder)
    {
        //ClosedSplineLineを全取得
        var splines = _linesRoot.GetComponentsInChildren<ClosedSplineLine>();

        builder.Register<OrbitManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<ClosedSplineLine>>(splines); // 全てのSplineを挿入

        builder.Register<OrbitFacade>(Lifetime.Singleton).As<IOrbitFacade>();

        builder.RegisterComponent(_orbitPointRotationRangeController);


    }
}
