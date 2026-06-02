using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PointObjectManager : IDisposable
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    // ãˆÊƒNƒ‰ƒX‚Ö’Ê’m‚·‚é‚½‚ß‚Ì‘‹Œû
    private readonly Subject<PointObjectType> _onPointCollected = new Subject<PointObjectType>();
    public IObservable<PointObjectType> OnPointCollected => _onPointCollected;

    /// <summary>
    /// ŠÇ—‘ÎÛ‚ÌPointObjectŒQ‚ğ“o˜^‚µ‚Äw“ÇŠJn
    /// </summary>
    public PointObjectManager(IEnumerable<PointObject> pointObjects)
    {
        //Debug.Log($"[PointObjectManager] Registering {pointObjects} point objects");
        foreach (var po in pointObjects)
        {
            if (po == null) continue;

            po.OnTriggered
              .Subscribe(type =>
              {
                  _onPointCollected.OnNext(type);
              })
              .AddTo(_disposables);
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _onPointCollected.OnCompleted();
        _onPointCollected.Dispose();
    }
}