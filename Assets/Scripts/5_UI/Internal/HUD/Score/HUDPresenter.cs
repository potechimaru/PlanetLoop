using System;
using UniRx;

public class HUDPresenter : IDisposable
{
    private readonly HUDModel _model;
    private readonly ScoreView _view;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly IUIExternalFacade _uiExternalFacade;

    public HUDPresenter(ScoreView view, IUIExternalFacade uiExternalFacade)
    {
        _model = new HUDModel();
        _view = view;

        _uiExternalFacade = uiExternalFacade;

        _model.Score
            .Subscribe(score => _view.SetScore(score))
            .AddTo(_disposables);

        _uiExternalFacade.OnPointCollected
            .Subscribe(_ => AddScore(ScoreRuleType.PointObject, _))
            .AddTo(_disposables);

        _view.SetScore(_model.Score.Value);
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

    public void Dispose()
    {
        _disposables.Dispose();
    }
}