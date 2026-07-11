using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    private static RootLifetimeScope _instance;

    [SerializeField] private AudioPlayer _audioPlayer;

    protected override void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        base.Awake();
    }

    protected override void Configure(IContainerBuilder builder)
    {

        builder.Register<GameSessionService>(Lifetime.Singleton);
        builder.Register<SaveDataService>(Lifetime.Singleton);

        builder.Register<GameModeSelectionService>(Lifetime.Singleton)
            .As<IGameModeSelectionReader>()
            .As<IGameModeSelectionWriter>();

        builder.Register<AppStateRequestHub>(Lifetime.Singleton)
            .As<IAppStateChangeRequestSource>()
            .As<IAppStateChangeRequester>();

        builder.Register<SceneLoader>(Lifetime.Singleton);
        builder.Register<SceneLoadRequest>(Lifetime.Singleton);

        builder.RegisterEntryPoint<AppStateMachine>(Lifetime.Singleton);
        builder.Register<TitleState>(Lifetime.Singleton);
        builder.Register<ModeSelectState>(Lifetime.Singleton);
        builder.Register<GameState>(Lifetime.Singleton);

        builder.RegisterComponent(_audioPlayer);
        builder.Register<AudioManager>(Lifetime.Singleton);

        builder.Register<UnityroomRankingService>(Lifetime.Singleton);


    }

    protected override void OnDestroy()
    {
        if (_instance == this)
            _instance = null;

        base.OnDestroy();
    }
}