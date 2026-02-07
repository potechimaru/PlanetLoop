using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlayerInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PlayerView _playerView;
    public void Install(IContainerBuilder builder)
    {
        builder.Register<PlayerStateMachine>(Lifetime.Singleton);
        builder.Register<PlayerController>(Lifetime.Singleton).As<ITickable>().AsSelf();

        // Controllerà»äOíºê⁄éQè∆ã÷é~
        builder.RegisterComponent(_playerView);

        builder.Register<PlayerFacade>(Lifetime.Singleton).As<IPlayerFacade>();
        builder.Register<PlayerExternalFacade>(Lifetime.Singleton).As<IPlayerExternalFacade>();

    }
}
