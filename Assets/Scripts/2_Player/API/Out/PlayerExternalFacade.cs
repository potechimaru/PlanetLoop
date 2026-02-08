using System;
using UnityEngine;
public interface IPlayerExternalFacade
{
    // Input
    void MoveSubscribe(Action move);
    void JumpSubscribe(Action jump);

    // Orbit
    bool TryFindTouchedSpline(
        Vector3 pos,
        float radius,
        ClosedSplineLine exclude,
        out ClosedSplineLine result
    );

}

public class PlayerExternalFacade : IPlayerExternalFacade
{
    private readonly IInputFacade _inputFacade;
    private readonly IOrbitFacade _orbitFacade;

    public PlayerExternalFacade(
        IInputFacade inputFacade,
        IOrbitFacade orbitFacade)
    {
        _inputFacade = inputFacade;
        _orbitFacade = orbitFacade;
    }

    // Input
    public void MoveSubscribe(Action move)
    {
        _inputFacade.MoveSubscribe(move);
    }

    public void JumpSubscribe(Action jump)
    {
        _inputFacade.JumpSubscribe(jump);
    }

    // Orbit
    public bool TryFindTouchedSpline(
        Vector3 pos,
        float radius,
        ClosedSplineLine exclude,
        out ClosedSplineLine result)
    {
        return _orbitFacade.TryFindTouchedSpline(
            pos, radius, exclude, out result);
    }
}
