using UnityEngine;
using VContainer;
using VContainer.Unity;

public class BlachHoleInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private BlackHoleGravity _blackHoleGravity;
    [SerializeField] private BlackHoleDetector _blackHoleDetector;
    public void Install(IContainerBuilder builder)
    {
        builder.Register<BlackHoleFacade>(Lifetime.Singleton).As<IBlackHoleFacade>();
        builder.Register<BlackHoleExternalFacade>(Lifetime.Singleton).As<IBlackHoleExternalFacade>();

        builder.RegisterComponent(_blackHoleGravity);
        builder.RegisterComponent(_blackHoleDetector);
    }
}
