using System;
using UnityEngine;
public interface IPlayerExternalFacade
{
    // Input
    void MoveSubscribe(Action move);
    void JumpReleasedSubscribe(Action jumpReleased);

    void JumpPressedSubscribe(Action jumpPressed);

    // Orbit
    bool TryFindTouchedSpline(
        Vector3 pos,
        float radius,
        ClosedSplineLine exclude,
        out ClosedSplineLine result
    );

    // BlackHole
    Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt);

}

public class PlayerExternalFacade : IPlayerExternalFacade
{
    private readonly IInputFacade _inputFacade;
    private readonly IOrbitFacade _orbitFacade;
    private readonly IBlackHoleFacade _blackHoleFacade;

    public PlayerExternalFacade(
        IInputFacade inputFacade,
        IOrbitFacade orbitFacade,
        IBlackHoleFacade blackHoleFacade)
    {
        _inputFacade = inputFacade;
        _orbitFacade = orbitFacade;
        _blackHoleFacade = blackHoleFacade;

    }

    // Input
    public void MoveSubscribe(Action move)
    {
        _inputFacade.MoveSubscribe(move);
    }

    public void JumpReleasedSubscribe(Action jumpReleased)
    {
        _inputFacade.JumpReleasedSubscribe(jumpReleased);
    }

    public void JumpPressedSubscribe(Action jumpPressed)
    {
        _inputFacade.JumpPressedSubscribe(jumpPressed);
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

    // BlackHole
    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        return _blackHoleFacade.BendDirection(worldPos, dir, dt);
    }

}
