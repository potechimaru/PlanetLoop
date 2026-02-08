using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class OrbitInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] Transform _linesRoot;
    public void Install(IContainerBuilder builder)
    {
        // ƒqƒGƒ‰ƒ‹ƒL[ã‚Ì ClosedSplineLine ‚ğ‘Sæ“¾
        var splines = _linesRoot.GetComponentsInChildren<ClosedSplineLine>();

        // OrbitManager ‚ğ Singleton ‚Å“o˜^
        builder.Register<OrbitManager>(Lifetime.Singleton)
               .AsSelf()
               .WithParameter<IEnumerable<ClosedSplineLine>>(splines);


        builder.Register<OrbitFacade>(Lifetime.Singleton).As<IOrbitFacade>();

        
    }
}
