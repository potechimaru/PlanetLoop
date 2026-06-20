using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUIFacade
{
    bool IsMaxLongJumpedCount();
    int GetScore();

    float GetElapsedTime();

    void StartTimer();
    void StopTimer();
}

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
