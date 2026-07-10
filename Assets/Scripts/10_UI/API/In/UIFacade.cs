using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUIFacade
{
    bool IsMaxLongJumpedCount();
    int GetScore();

    int GetDefeatEnemyCount();

    int GetVisitedSplineCount();

    float GetElapsedTime();

    void StartTimer();
    void StopTimer();
}

/// <summary>
/// UIコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
public class UIFacade : IUIFacade
{
    private readonly HUDPresenter _hudPresenter;

    public UIFacade(HUDPresenter hudPresenter, PlayUIFactory playUIFactory)
    {
        _hudPresenter = hudPresenter;
    }

    public bool IsMaxLongJumpedCount()
    {
        return _hudPresenter.IsMaxLongJumpedCount();
    }

    public int GetScore()
    {
        return _hudPresenter.GetScore();
    }

    public int GetDefeatEnemyCount()
    {
        return _hudPresenter.GetDefeatEnemyCount();
    }

    public int GetVisitedSplineCount()
    {
        return _hudPresenter.GetVisitedSplineCount();
    }

    public float GetElapsedTime()
    {
        return _hudPresenter.GetElapsedTime();
    }

    public void StartTimer()
    {
        _hudPresenter.StartTimer();
    }

    public void StopTimer()
    {
        _hudPresenter.StopTimer();
    }
}
