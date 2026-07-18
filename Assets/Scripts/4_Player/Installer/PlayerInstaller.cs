using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Player�R���|�[�l���g�Q��DI�R���e�i�ɓo�^����
/// </summary>
public class PlayerInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private LayerMask playerSpawnBlockingLayerMask;
    public void Install(IContainerBuilder builder)
    {
        builder.Register<PlayerStateMachine>(Lifetime.Singleton).As<IDisposable>().AsSelf();
        builder.Register<PlayerController>(Lifetime.Singleton).As<ITickable>().AsSelf().As<IDisposable>();

        // Controller�ȊO���ڎQ�Ƌ֎~
        builder.RegisterComponent(_playerView);


        builder.Register<PlayerFacade>(Lifetime.Singleton).As<IPlayerFacade>();
        builder.Register<PlayerExternalFacade>(Lifetime.Singleton).As<IPlayerExternalFacade>();

        builder.Register<PlayerSpawnOverlapResolver>(Lifetime.Singleton)
        .WithParameter(playerSpawnBlockingLayerMask);

    }
}
