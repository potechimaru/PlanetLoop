using System;
using System.Diagnostics;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class HUDPresenter : IDisposable
{
    private readonly HUDModel _model;
    private readonly ScoreView _scoreView;
    private readonly DefeatEnemyCountView _defeatEnemyCountView;
    private readonly VisitedSplineCountView _visitedSplineCountView;
    private readonly LongJumpedCountView _longJumpedCountView;
    private readonly PlayUIFactory _playUIFactory;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IUIExternalFacade _uiExternalFacade;

    private readonly Transform _playerTransform;


    public HUDPresenter(ScoreView scoreView, DefeatEnemyCountView defeatEnemyCountView ,VisitedSplineCountView visitedSplineCountView, LongJumpedCountView longJumpedCountView, IUIExternalFacade uiExternalFacade, PlayUIFactory playUIFactory, Transform playerTransform)
    {
        _model = new HUDModel();
        _scoreView = scoreView;
        _defeatEnemyCountView = defeatEnemyCountView;
        _visitedSplineCountView = visitedSplineCountView;
        _longJumpedCountView = longJumpedCountView;
        _playUIFactory = playUIFactory;

        _uiExternalFacade = uiExternalFacade;

        _playerTransform = playerTransform;

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
            .Subscribe(_ => {
                AddScore(ScoreRuleType.NewOrbit, _playerTransform.position);
                //UnityEngine.Debug.Log("HUDPresenter: New orbit attached, score updated");
                }
            )
            .AddTo(_disposables);

        _uiExternalFacade.OnLongJumped
            .Subscribe(_ =>
            {
                IncrementLongJumpedCount(_playerTransform.position);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnPointCollected
            .Subscribe(_ =>
            {
                AddScore(ConvertToScoreRuleType(_), _playerTransform.position);
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
                //AddScore(ScoreRuleType.DefeatEnemy);
            })
            .AddTo(_disposables);

        _uiExternalFacade.OnEnemyDefeated
            .Subscribe(_ =>
            {
                AddScore(ScoreRuleType.DefeatEnemy, _playerTransform.position);
            })
            .AddTo(_disposables);

        _scoreView.SetScore(_model.Score.Value);
        

    }

    public void AddScore(ScoreRuleType type, Vector3 position)
    {

        int before = _model.Score.Value;

        _model.AddScore(type);

        //UnityEngine.Debug.Log($"HUDPresenter.AddScore called with type: {type}, position: {position}, score before: {before}, score after: {_model.Score.Value}");
        _playUIFactory.Spawn(ConvertToPlayUIType(type), new Vector2(position.x, position.y + _model.OffsetY));

        int added = _model.Score.Value - before;
    }

    //public void AddScore(ScoreRuleType type, PointObjectType pointObjectType, Vector3 position)
    //{
    //    if (!(type == ScoreRuleType.PointMedium || type == ScoreRuleType.PointHigh || type == ScoreRuleType.PointLow)) return;

    //    int before = _model.Score.Value;

    //    _model.AddScore(type, pointObjectType);

    //    _playUIFactory.Spawn(ConvertToPlayUIType(type), new Vector2(position.x, position.y + _model.OffsetY));

    //    int added = _model.Score.Value - before;
    //}

    public void ReflectEnemyCount(int defeatEnemyCount, int allEnemyCount)
    {
        //UnityEngine.Debug.Log($"HUDPresenter.ReflectEnemyCount called with defeatEnemyCount: {defeatEnemyCount}, allEnemyCount: {allEnemyCount}");
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
                return PlayUIType.NewOrbitPoint; // デフォルト値
        }
    }

    private ScoreRuleType ConvertToScoreRuleType (PointObjectType type)
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
                return ScoreRuleType.PointMedium; // デフォルト値
        }

    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}