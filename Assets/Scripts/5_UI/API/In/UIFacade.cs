using UnityEngine;

public interface IUIFacade
{
    void AddScore(ScoreRuleType type);

}

public class UIFacade : IUIFacade
{
    private readonly HUDPresenter _hudPresenter;
    
    public UIFacade(HUDPresenter hudPresenter)
    {
        _hudPresenter = hudPresenter;
    
    }

    public void AddScore(ScoreRuleType type)
    {
        _hudPresenter.AddScore(type);
    }

}
