using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        DontDestroyOnLoad(this);

        // 実行中の状態
        builder.Register<GameSessionService>(Lifetime.Singleton);

        // 永続データ（PlayerPrefs）
        builder.Register<SaveDataService>(Lifetime.Singleton);

        builder.Register<AppStateRequestHub>(Lifetime.Singleton).As<IAppStateChangeRequestSource>().As<IAppStateChangeRequester>();

        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<SceneLoadRequest>(Lifetime.Singleton);

        builder.RegisterEntryPoint<AppStateMachine>(Lifetime.Singleton);
        builder.Register<TitleState>(Lifetime.Singleton);
        builder.Register<ModeSelectState>(Lifetime.Singleton);
        builder.Register<EndlessModeState>(Lifetime.Singleton);
    }
}