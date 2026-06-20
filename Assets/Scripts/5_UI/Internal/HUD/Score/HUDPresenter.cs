using System;
using UniRx;
using UnityEngine;

public class HUDPresenter : IDisposable
{
    private readonly HUDModel _model;
    private readonly ScoreView _scoreView;
    private readonly IDefeatEnemyCountView _defeatEnemyCountView;
    private readonly IVisitedSplineCountView _visitedSplineCountView;
    private readonly ILongJumpedCountView _longJumpedCountView;
    private readonly IResetConditionView _resetConditionView;
    private readonly TimerView _timerView;

    private readonly PlayUIFactory _playUIFactory;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IUIExternalFacade _uiExternalFacade;
    private readonly Transform _playerTransform;

    private IDisposable _timerDisposable;
    private float _elapsedTime;

    public HUDPresenter(
        ScoreView scoreView,
        IDefeatEnemyCountView defeatEnemyCountView,
        IVisitedSplineCountView visitedSplineCountView,
        ILongJumpedCountView longJumpedCountView,
        IResetConditionView resetConditionView,
        TimerView timerView,
        IUIExternalFacade uiExternalFacade,
        PlayUIFactory playUIFactory,
        Transform playerTransform)
    {
        _model = new HUDModel();

        _scoreView = scoreView;
        _defeatEnemyCountView = defeatEnemyCountView;
        _visitedSplineCountView = visitedSplineCountView;
        _longJumpedCountView = longJumpedCountView;
        _resetConditionView = resetConditionView;
        _timerView = timerView;

        _uiExternalFacade = uiExternalFacade;
        _playUIFactory = playUIFactory;
        _playerTransform = playerTransform;

        _model.Score
            .Subscribe(score => _scoreView.SetScore(score))
            .AddTo(_disposables);

        _model.DefeatEnemyCount
            .CombineLatest(
                _model.AllEnemyCount,
                (defeat, all) => (defeat, all))
            .Subscribe(x =>
            {
                _defeatEnemyCountView.SetCount(x.defeat, x.all);
                _resetConditionView.SetCount(x.defeat, _model.EnemyCountToReset);
            })
            .AddTo(_disposables);

        _model.VisitedSplineCount
            .CombineLatest(
                _model.AllSplineCount,
                (visited, all) => (visited, all))
            .Subscribe(x =>
            {
                _visitedSplineCountView.SetCount(x.visited, x.all);
            })
            .AddTo(_disposables);

        _model.LongJumpedCount
            .Subscribe(count =>
            {
                _longJumpedCountView.SetCount(count, _model.MaxLongJumpedCount);
            })
            .AddTo(_disposables);

        _model.OnEnemyResetThresholdReached
            .Subscribe(_ =>
            {
                _uiExternalFacade.ResetAllOrbits();
                _uiExternalFacade.ResetAllPoints();
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnNewOrbitAttached
            .Subscribe(_ =>
            {
                AddScore(ScoreRuleType.NewOrbit, _playerTransform.position);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnLongJumped
            .Subscribe(_ =>
            {
                IncrementLongJumpedCount(_playerTransform.position);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnPointCollected
            .Subscribe(type =>
            {
                AddScore(ConvertToScoreRuleType(type), _playerTransform.position);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnSplineCountChanged
            .Subscribe(counts =>
            {
                ReflectSplineCount(counts.visitedCount, counts.allCount);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnEnemyCountChanged
            .Subscribe(counts =>
            {
                ReflectEnemyCount(counts.defeatEnemyCount, counts.allEnemyCount);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnEnemyDefeated
            .Subscribe(_ =>
            {
                AddScore(ScoreRuleType.DefeatEnemy, _playerTransform.position);
            })
            .AddTo(_disposables);

        _scoreView.SetScore(_model.Score.Value);
        _timerView.ResetTime();
    }

    public void StartTimer()
    {
        StopTimer();

        _elapsedTime = 0f;
        _timerView.ResetTime();

        _timerDisposable = Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                _elapsedTime += Time.deltaTime;
                _timerView.SetTime(_elapsedTime);
            });
    }

    public void StopTimer()
    {
        _timerDisposable?.Dispose();
        _timerDisposable = null;
    }

    public void AddScore(ScoreRuleType type, Vector3 position)
    {
        _model.AddScore(type);

        _playUIFactory.Spawn(
            ConvertToPlayUIType(type),
            new Vector2(position.x, position.y + _model.OffsetY)
        );
    }

    public void ReflectEnemyCount(int defeatEnemyCount, int allEnemyCount)
    {
        _model.ReflectEnemyCount(defeatEnemyCount, allEnemyCount);
    }

    public void ReflectSplineCount(int visitedCount, int allCount)
    {
        _model.ReflectSplineCount(visitedCount, allCount);
    }

    public void IncrementLongJumpedCount(Vector3 position)
    {
        if (_model.IsMaxLongJumpedCount())
            return;

        _model.IncrementLongJumpedCount();
        AddScore(ScoreRuleType.LongJumped, position);
    }

    public bool IsMaxLongJumpedCount()
    {
        return _model.IsMaxLongJumpedCount();
    }

    public int GetScore()
    {
        return _model.Score.Value;
    }

    public float GetElapsedTime()
    {
        return _elapsedTime;
    }

    private PlayUIType ConvertToPlayUIType(ScoreRuleType type)
    {
        switch (type)
        {
            case ScoreRuleType.NewOrbit:
                return PlayUIType.NewOrbitPoint;
            case ScoreRuleType.DefeatEnemy:
                return PlayUIType.DefeatEnemyPoint;
            case ScoreRuleType.LongJumped:
                return PlayUIType.LongJumpPoint;
            case ScoreRuleType.PointVeryHigh:
                return PlayUIType.PointVeryHigh;
            case ScoreRuleType.PointHigh:
                return PlayUIType.PointHigh;
            case ScoreRuleType.PointMedium:
                return PlayUIType.PointMedium;
            case ScoreRuleType.PointLow:
                return PlayUIType.PointLow;
            default:
                return PlayUIType.NewOrbitPoint;
        }
    }

    private ScoreRuleType ConvertToScoreRuleType(PointObjectType type)
    {
        switch (type)
        {
            case PointObjectType.VeryHigh:
                return ScoreRuleType.PointVeryHigh;
            case PointObjectType.High:
                return ScoreRuleType.PointHigh;
            case PointObjectType.Medium:
                return ScoreRuleType.PointMedium;
            case PointObjectType.Low:
                return ScoreRuleType.PointLow;
            default:
                return ScoreRuleType.PointMedium;
        }
    }

    public void Dispose()
    {
        StopTimer();
        _disposables.Dispose();
    }
}