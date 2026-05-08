using UnityEngine;
using VContainer;
using VContainer.Unity;

public class LoadingSceneLifetimeScope : LifetimeScope
{
    [SerializeField] private LoadingSceneController _controller;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_controller);

    }
}
