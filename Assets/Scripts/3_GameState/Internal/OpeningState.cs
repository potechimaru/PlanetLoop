using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

/// <summary>
/// GameStateのOpeningStateを表すクラス。ゲーム開始時のオープニング処理を担当する。
/// </summary>
public class OpeningState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();
    private readonly IGameStateExternalFacade _gameStateExternalFacade;
    private readonly GameUIManager _gameUIManager;
    private readonly SaveDataService _saveDataService;
    private readonly AudioManager _audioManager;
    private readonly SceneLoadRequest _sceneLoadRequest;

    public OpeningState(IGameStateExternalFacade gameStateExternalFacade, GameUIManager gameUIManager, SaveDataService saveDataService, AudioManager audioManager, SceneLoadRequest sceneLoadRequest)
    {
        _gameStateExternalFacade = gameStateExternalFacade;
        _gameUIManager = gameUIManager;
        _saveDataService = saveDataService;
        _audioManager = audioManager;
        _sceneLoadRequest = sceneLoadRequest;
    }

    public async UniTask Enter()
    {
        _gameStateExternalFacade.RegisterPlayerSubscriptions();
        _gameStateExternalFacade.SetPlayer();
        _gameStateExternalFacade.SetAllDetectionEnabled(false);
        Time.timeScale = 0f;
        await _gameUIManager.GameOpening();

        // 開いた事あったらチュートリアルをスキップする
        if (_saveDataService.IsFirstPlay())
        {
            _audioManager.PlayBGM(BGMType.Game);
            await _gameUIManager.ShowTutorial();
            await _gameUIManager.WaitUntilTutorialClosed();
            _saveDataService.MarkTutorialShown();
        }
        else
        {
            _audioManager.PlayBGM(BGMType.Game);
        }
        await _gameUIManager.ShowPreGame();
    }
    public async UniTask Exit()
    {
        // 時間再生
        Time.timeScale = 1f;
        _gameStateExternalFacade.SetAllDetectionEnabled(true);
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        await UniTask.CompletedTask;
    }
}
