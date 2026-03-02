using System;
using UnityEngine;

public interface IPointObjectFacade
{
    IObservable<PointObjectType> OnPointCollected { get; }

}

public class PointObjectFacade : IPointObjectFacade
{
    private readonly PointObjectManager _manager;
    public PointObjectFacade(PointObjectManager manager)
    {
        _manager = manager;
    }

    public IObservable<PointObjectType> OnPointCollected => _manager.OnPointCollected;


}
