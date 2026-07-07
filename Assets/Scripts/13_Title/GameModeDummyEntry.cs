using UnityEngine;

/// <summary>
/// DummyPlayerとGameModeを紐づけるためのエントリークラス。これが無いと暗黙的な対応になってしまう
/// </summary>
public class GameModeDummyEntry : MonoBehaviour
{
    [SerializeField] private GameModeType _gameMode;
    [SerializeField] private DummyPlayerSplineStopAnimator _dummyPlayerAnimator;
    [SerializeField] private int _order;

    public GameModeType GameMode => _gameMode;
    public DummyPlayerSplineStopAnimator DummyPlayerAnimator => _dummyPlayerAnimator;
    public int Order => _order;

    private void Reset()
    {
        if (_dummyPlayerAnimator == null)
        {
            _dummyPlayerAnimator = GetComponent<DummyPlayerSplineStopAnimator>();
        }
    }
}