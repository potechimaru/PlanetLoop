using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Playerコンポーネント群をDIコンテナに登録する
/// </summary>
public class PlayerInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private LayerMask playerSpawnBlockingLayerMask;
    public void Install(IContainerBuilder builder)
    {
        builder.Register<PlayerStateMachine>(Lifetime.Singleton).As<IDisposable>().AsSelf();
        builder.Register<PlayerController>(Lifetime.Singleton).As<ITickable>().AsSelf();

        // Controller以外直接参照禁止
        builder.RegisterComponent(_playerView);


        builder.Register<PlayerFacade>(Lifetime.Singleton).As<IPlayerFacade>();
        builder.Register<PlayerExternalFacade>(Lifetime.Singleton).As<IPlayerExternalFacade>();

        builder.Register<PlayerSpawnOverlapResolver>(Lifetime.Singleton)
        .WithParameter(playerSpawnBlockingLayerMask);

    }
}
