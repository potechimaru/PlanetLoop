using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PointObjectManager : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly Subject<PointObjectType> _onPointCollected = new();
    public IObservable<PointObjectType> OnPointCollected => _onPointCollected;

    private readonly List<PointObject> _pointObjects = new();

    private bool _isListening;

    public PointObjectManager(IEnumerable<PointObject> pointObjects)
    {
        foreach (var po in pointObjects)
        {
            if (po == null) continue;

            po.SetCollectEnabled(false);

            _pointObjects.Add(po);
        }
    }

    public void StartListening()
    {
        if (_isListening) return;
        _isListening = true;

        foreach (var po in _pointObjects)
        {
            if (po == null) continue;

            po.SetCollectEnabled(true);

            po.OnTriggered
                .Subscribe(type =>
                {
                    _onPointCollected.OnNext(type);
                })
                .AddTo(_disposables);
        }
    }

    public void ResetAllPoints()
    {
        foreach (var point in _pointObjects)
        {
            if (point == null) continue;

            point.ResetPoint();
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _onPointCollected.OnCompleted();
        _onPointCollected.Dispose();
    }
}