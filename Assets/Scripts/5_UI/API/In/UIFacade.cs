using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUIFacade
{
    bool IsMaxLongJumpedCount();
    int GetScore();

}

public class UIFacade : IUIFacade
{
    private readonly HUDPresenter _hudPresenter;
    private readonly PlayUIFactory _playUIFactory;

    public UIFacade(HUDPresenter hudPresenter, PlayUIFactory playUIFactory)
    {
        _hudPresenter = hudPresenter;
        _playUIFactory = playUIFactory;

    }

    public bool IsMaxLongJumpedCount()
    {
        return _hudPresenter.IsMaxLongJumpedCount();
    }

    public int GetScore()
    {
        return _hudPresenter.GetScore();
    }

}
