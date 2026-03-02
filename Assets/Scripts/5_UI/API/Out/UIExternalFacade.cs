using System;
using UnityEngine;

public interface IUIExternalFacade
{
    // PointObject
    IObservable<PointObjectType> OnPointCollected { get; }
}
public class UIExternalFacade : IUIExternalFacade
{
    private readonly IPointObjectFacade _pointObjectFacade;
    public UIExternalFacade(IPointObjectFacade pointObjectFacade)
    {
        _pointObjectFacade = pointObjectFacade;
    }

    // PointObject
    public IObservable<PointObjectType> OnPointCollected => _pointObjectFacade.OnPointCollected;
}
