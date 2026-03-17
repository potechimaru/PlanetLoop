using System;
using System.Diagnostics;
using UniRx;
using UnityEngine;

public class HUDPresenter : IDisposable
{
    private readonly HUDModel _model;
    private readonly ScoreView _scoreView;
    private readonly DefeatEnemyCountView _defeatEnemyCountView;
    private readonly VisitedSplineCountView _visitedSplineCountView;
    private readonly LongJumpedCountView _longJumpedCountView;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IUIExternalFacade _uiExternalFacade;

    public HUDPresenter(ScoreView scoreView, DefeatEnemyCountView defeatEnemyCountView ,VisitedSplineCountView visitedSplineCountView, LongJumpedCountView longJumpedCountView, IUIExternalFacade uiExternalFacade)
    {
        _model = new HUDModel();
        _scoreView = scoreView;
        _defeatEnemyCountView = defeatEnemyCountView;
        _visitedSplineCountView = visitedSplineCountView;
        _longJumpedCountView = longJumpedCountView;

        _uiExternalFacade = uiExternalFacade;

        _model.Score
        .Subscribe(score => _scoreView.SetScore(score))
        .AddTo(_disposables);

        _model.DefeatEnemyCount
            .CombineLatest(
                _model.AllEnemyCount,
                (defeat, all) => (defeat, all))
            .Subscribe(x => _defeatEnemyCountView.SetEnemyCount(x.defeat, x.all))
            .AddTo(_disposables);

        _model.VisitedSplineCount
            .CombineLatest(
                _model.AllSplineCount,
                (visited, all) => (visited, all))
            .Subscribe(x => _visitedSplineCountView.SetSplineCount(x.visited, x.all))
            .AddTo(_disposables);

        _model.LongJumpedCount
            .Subscribe(count => _longJumpedCountView.SetLongJumpedCount(count, _model.MaxLongJumpedCount))
            .AddTo(_disposables);

        _uiExternalFacade.OnNewOrbitAttached
            .Subscribe(_ => AddScore(ScoreRuleType.NewOrbit))
            .AddTo(_disposables);

        _uiExternalFacade.OnLongJumped
            .Subscribe(_ =>
            {
                IncrementLongJumpedCount();
                AddScore(ScoreRuleType.LongJumped);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnPointCollected
            .Subscribe(_ =>
            {
                AddScore(ScoreRuleType.PointObject, _);
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
                AddScore(ScoreRuleType.DefeatEnemy);
            })
            .AddTo(_disposables);

        _scoreView.SetScore(_model.Score.Value);
        

    }

    public void AddScore(ScoreRuleType type)
    {
        _model.AddScore(type);
    }

    public void AddScore(ScoreRuleType type, PointObjectType pointObjectType)
    {
        if (type != ScoreRuleType.PointObject) return;
        _model.AddScore(type, pointObjectType);
    }

    public void ReflectEnemyCount(int defeatEnemyCount, int allEnemyCount)
    {
        UnityEngine.Debug.Log($"HUDPresenter.ReflectEnemyCount called with defeatEnemyCount: {defeatEnemyCount}, allEnemyCount: {allEnemyCount}");
        _model.ReflectEnemyCount(defeatEnemyCount, allEnemyCount);
    }

    public void ReflectSplineCount(int visitedCount, int allCount)
    {
        _model.ReflectSplineCount(visitedCount, allCount);
    }

    public void IncrementLongJumpedCount()
    {
        _model.IncrementLongJumpedCount();
    }

    public bool IsMaxLongJumpedCount()
    {
        return _model.IsMaxLongJumpedCount();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}