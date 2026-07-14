using System;
using UniRx;
using VContainer.Unity;

/// <summary>
/// GameModeSelectでのGameModeの変更を監視し、Root側へ反映するクラス。
/// </summary>
public class GameModeSelectionPresenter : IStartable, IDisposable
{
    private readonly IGameModeManager _gameModeManager;
    private readonly IGameModeSelectionWriter _selectionWriter;

    private readonly CompositeDisposable _disposables = new();

    public GameModeSelectionPresenter(
        IGameModeManager gameModeManager,
        IGameModeSelectionWriter selectionWriter)
    {
        _gameModeManager = gameModeManager;
        _selectionWriter = selectionWriter;
    }

    public void Start()
    {
        // 初期値をRoot側へ反映
        _selectionWriter.SetSelectedMode(_gameModeManager.CurrentSelectedMode);

        // 変更時もRoot側へ反映
        _gameModeManager.OnSelectedModeChanged
            .Subscribe(mode =>
            {
                _selectionWriter.SetSelectedMode(mode);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}