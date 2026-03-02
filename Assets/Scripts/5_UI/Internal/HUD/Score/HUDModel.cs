using UniRx;

public class HUDModel
{
    private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> Score => _score;

    private readonly int _NEW_ORBIT_SCORE = 100;
    private readonly int _DEFEAT_ENEMY_SCORE = 50;

    private readonly int _NORMAL_POINT_OBJECT_SCORE = 100;
    private readonly int _HIGH_POINT_OBJECT_SCORE = 200;

    public void SetScore(int value)
    {
        _score.Value = value;
    }

    public void AddScore(ScoreRuleType type)
    {
        if (type == ScoreRuleType.NewOrbit)
        {
            _score.Value += _NEW_ORBIT_SCORE;
        }
        else if (type == ScoreRuleType.DefeatEnemy)
        {
            _score.Value += _DEFEAT_ENEMY_SCORE;
        }
    }

    /// <summary>
    /// PointObjectのスコアを追加する。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="pointObjectType"></param>
    public void AddScore(ScoreRuleType type, PointObjectType pointObjectType)
    {
        if (type != ScoreRuleType.PointObject) return;
        if (pointObjectType == PointObjectType.High)
        {
            _score.Value += _HIGH_POINT_OBJECT_SCORE; // High point object score
        }
        else if (pointObjectType == PointObjectType.Normal)
        {
            _score.Value += _NORMAL_POINT_OBJECT_SCORE; // Normal point object score
        }
    }

    public void ResetScore()
    {
        _score.Value = 0;
    }
}