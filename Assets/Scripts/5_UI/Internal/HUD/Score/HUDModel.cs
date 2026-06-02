using UniRx;

public class HUDModel
{
    private readonly ReactiveProperty<int> _score = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> Score => _score;

    private readonly ReactiveProperty<int> _defeatEnemyCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> DefeatEnemyCount => _defeatEnemyCount;

    private readonly ReactiveProperty<int> _allEnemyCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> AllEnemyCount => _allEnemyCount;

    private readonly ReactiveProperty<int> _allSplineCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> AllSplineCount => _allSplineCount;

    private readonly ReactiveProperty<int> _visitedSplineCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> VisitedSplineCount => _visitedSplineCount;

    private readonly ReactiveProperty<int> _longJumpedCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> LongJumpedCount => _longJumpedCount;

    public  int MaxLongJumpedCount { get; private set; } = 3;


    private readonly int _NEW_ORBIT_SCORE = 50;
    private readonly int _DEFEAT_ENEMY_SCORE = 50;

    private readonly int _LOW_POINT_OBJECT_SCORE = 5;
    private readonly int _NORMAL_POINT_OBJECT_SCORE = 20;
    private readonly int _HIGH_POINT_OBJECT_SCORE = 100;
    private readonly int _VERY_HIGH_POINT_OBJECT_SCORE = 500;

    private readonly int _LONG_JUMP_SCORE = 1500;

    public float OffsetY { get; } = 1.5f;

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
        else if (type == ScoreRuleType.LongJumped)
        {
            _score.Value += _LONG_JUMP_SCORE;
        }
        else if (type == ScoreRuleType.PointVeryHigh)
        {
            _score.Value += _VERY_HIGH_POINT_OBJECT_SCORE;
        }
        else if (type == ScoreRuleType.PointHigh)
        {
            _score.Value += _HIGH_POINT_OBJECT_SCORE;
        }
        else if (type == ScoreRuleType.PointMedium)
        {
            _score.Value += _NORMAL_POINT_OBJECT_SCORE;
        }
        else if (type == ScoreRuleType.PointLow)
        {
            _score.Value += _LOW_POINT_OBJECT_SCORE;
        }
    }

    //public void AddScore(ScoreRuleType type, PointObjectType pointObjectType)
    //{
    //    if (!(type == ScoreRuleType.PointMedium || type == ScoreRuleType.PointHigh || type == ScoreRuleType.PointLow)) return;

    //    if (pointObjectType == PointObjectType.High)
    //    {
    //        _score.Value += _HIGH_POINT_OBJECT_SCORE;
    //    }
    //    else if (pointObjectType == PointObjectType.Medium)
    //    {
    //        _score.Value += _NORMAL_POINT_OBJECT_SCORE;
    //    }
    //    else if (pointObjectType == PointObjectType.Low)
    //    {
    //        _score.Value += _LOW_POINT_OBJECT_SCORE;
    //    }
    //}

    public void IncrementLongJumpedCount()
    {
        _longJumpedCount.Value += 1;
    }

    public void ReflectEnemyCount(int defeatEnemyCount, int allEnemyCount)
    {
        _defeatEnemyCount.Value = defeatEnemyCount;
        _allEnemyCount.Value = allEnemyCount;
    }

    public void ReflectSplineCount(int visitedSplineCount, int allSplineCount)
    {
        _visitedSplineCount.Value = visitedSplineCount;
        _allSplineCount.Value = allSplineCount;
    }

    public bool IsMaxLongJumpedCount()
    {
        return _longJumpedCount.Value >= MaxLongJumpedCount;
    }

    public void ResetScore()
    {
        _score.Value = 0;
    }
}