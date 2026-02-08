using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IOrbitFacade
{
    bool TryFindTouchedSpline(
        Vector3 pos, 
        float radius, 
        ClosedSplineLine exclude, 
        out ClosedSplineLine result
        );

}
public class OrbitFacade : IOrbitFacade
{
    private readonly OrbitManager _orbitManager;
    public OrbitFacade(OrbitManager orbitManager)
    {
        _orbitManager = orbitManager;
    }

    public bool TryFindTouchedSpline(Vector3 pos, float radius, ClosedSplineLine exclude, out ClosedSplineLine result)
    {
        return _orbitManager.TryFindTouchedSpline(pos, radius, exclude, out result);
    }



}
