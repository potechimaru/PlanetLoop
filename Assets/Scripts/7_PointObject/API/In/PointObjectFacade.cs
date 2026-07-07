using System;
using UnityEngine;

public interface IPointObjectFacade
{
    void StartPointObjectListening();
    IObservable<PointObjectType> OnPointCollected { get; }

    void ResetAllPoints();

}

public class PointObjectFacade : IPointObjectFacade
{
    private readonly PointObjectManager _manager;
    public PointObjectFacade(PointObjectManager manager)
    {
        _manager = manager;
    }

    public IObservable<PointObjectType> OnPointCollected => _manager.OnPointCollected;

    public void StartPointObjectListening()
    {
        _manager.StartListening();
    }

    public void ResetAllPoints()
    {
        _manager.ResetAllPoints();
    }


}
