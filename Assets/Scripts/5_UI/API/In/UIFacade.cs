using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IUIFacade
{
    void AddScore(ScoreRuleType type);
    bool IsMaxLongJumpedCount();
    void SpawnNewOrbitPointUI(Vector3 displayPos);

}

public class UIFacade : IUIFacade
{
    private readonly HUDPresenter _hudPresenter;
    private readonly PlayUIFactory _playUIFactory;

    public UIFacade( HUDPresenter hudPresenter, PlayUIFactory playUIFactory)
    {
        _hudPresenter = hudPresenter;
        _playUIFactory = playUIFactory;

    }

    public void AddScore(ScoreRuleType type)
    {
        _hudPresenter.AddScore(type);
    }

    public bool IsMaxLongJumpedCount()
    {
        return _hudPresenter.IsMaxLongJumpedCount();
    }

    public void SpawnNewOrbitPointUI(Vector3 displayPos)
    {
        _playUIFactory.Spawn(PlayUIType.NewOrbitPoint, displayPos);
    }



}
