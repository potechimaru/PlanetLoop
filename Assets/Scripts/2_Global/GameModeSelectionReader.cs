using System;
using UniRx;

/// <summary>
/// Gameシーンで選択されているゲームモードを取得するためのインターフェース
/// </summary>
public interface IGameModeSelectionReader
{
    GameModeType CurrentSelectedMode { get; }
    IObservable<GameModeType> OnSelectedModeChanged { get; }
}

/// <summary>
/// タイトルシーンで選んだモードを書き込むためのインターフェース
/// </summary>
public interface IGameModeSelectionWriter
{
    void SetSelectedMode(GameModeType gameModeType);
}

/// <summary>
/// RootLifetimeScopeに置いて、Gameシーンでも選んだモード等が取得できるようにするためのサービス
/// </summary>
public class GameModeSelectionService :
    IGameModeSelectionReader,
    IGameModeSelectionWriter,
    IDisposable
{
    private readonly ReactiveProperty<GameModeType> _selectedMode =
        new(GameModeType.Endless);

    public GameModeType CurrentSelectedMode => _selectedMode.Value;

    public IObservable<GameModeType> OnSelectedModeChanged => _selectedMode;

    public void SetSelectedMode(GameModeType gameModeType)
    {
        if (_selectedMode.Value == gameModeType)
            return;

        _selectedMode.Value = gameModeType;
    }

    public void Dispose()
    {
        _selectedMode.Dispose();
    }
}