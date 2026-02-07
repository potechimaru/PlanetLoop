using UnityEngine;
using VContainer;
using VContainer.Unity;

public class OrbitInstaller : MonoBehaviour, IInstaller
{
    public void Install(IContainerBuilder builder)
    {
        builder.Register<OrbitFacade>(Lifetime.Singleton).As<IOrbitFacade>();

    }
}
