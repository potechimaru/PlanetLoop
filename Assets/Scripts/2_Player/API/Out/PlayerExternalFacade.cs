using System;
using UniRx;
using UnityEngine;
public interface IPlayerExternalFacade
{
    // Input
    void MoveSubscribe(Action move);
    void JumpReleasedSubscribe(Action jumpReleased);

    void JumpPressedSubscribe(Action jumpPressed);

    // Orbit
    bool TryFindTouchedSpline(
    Vector3 from,
    Vector3 to,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine touchedSpline,
    out float hitDistanceOnSpline,
    out Vector3 hitPointOnSpline);

    ClosedSplineLine StartSpline { get; }

    // BlackHole
    Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt);

    IObservable<Unit> OnPlayerEnteredBlackHole { get; }
    IObservable<Unit> OnPlayerExitedOuterLimit { get; }

    // Enemy
    public IObservable<Unit> OnPlayerHitByEnemyBullet { get; }

    public IObservable<Unit> OnPlayerHitByLaserBeam { get; }

    IObservable<IEnemyContactHandle> OnPlayerTouchedEnemy { get; }

    void DefeatEnemy(IEnemyContactHandle enemyHandle);

    // Obstacle
    IObservable<Unit> OnPlayerHitObstacle { get; }



}


public class PlayerExternalFacade : IPlayerExternalFacade
{
    private readonly IInputFacade _inputFacade;
    private readonly IOrbitFacade _orbitFacade;
    private readonly IBlackHoleFacade _blackHoleFacade;
    private readonly IPointObjectFacade _pointObjectFacade;
    private readonly IEnemyFacade _enemyFacade;
    private readonly IObstacleFacade _obstacleFacade;


    public PlayerExternalFacade(
        IInputFacade inputFacade,
        IOrbitFacade orbitFacade,
        IBlackHoleFacade blackHoleFacade,
        IPointObjectFacade pointObjectFacade,
        IEnemyFacade enemyFacade,
        IObstacleFacade obstacleFacade
        )
    {
        _inputFacade = inputFacade;
        _orbitFacade = orbitFacade;
        _blackHoleFacade = blackHoleFacade;
        _pointObjectFacade = pointObjectFacade;
        _enemyFacade = enemyFacade;
        _obstacleFacade = obstacleFacade;


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
    Vector3 from,
    Vector3 to,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine touchedSpline,
    out float hitDistanceOnSpline,
    out Vector3 hitPointOnSpline)
    {
        return _orbitFacade.TryFindTouchedSpline(
            from,
            to,
            radius,
            exclude,
            out touchedSpline,
            out hitDistanceOnSpline,
            out hitPointOnSpline);
    }

    public ClosedSplineLine StartSpline => _orbitFacade.StartSpline;

    // BlackHole
    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        return _blackHoleFacade.BendDirection(worldPos, dir, dt);
    }

    public IObservable<Unit> OnPlayerEnteredBlackHole
        => _blackHoleFacade.OnPlayerEnteredBlackHole;

    public IObservable<Unit> OnPlayerExitedOuterLimit
        => _blackHoleFacade.OnPlayerExitedOuterLimit;

    // Enemy
    public IObservable<Unit> OnPlayerHitByEnemyBullet
        => _enemyFacade.OnPlayerHitByEnemyBullet;

    public IObservable<Unit> OnPlayerHitByLaserBeam
        => _enemyFacade.OnPlayerHitByLaserBeam;

    public IObservable<IEnemyContactHandle> OnPlayerTouchedEnemy
    => _enemyFacade.OnPlayerTouchedEnemy;

    public void DefeatEnemy(IEnemyContactHandle enemyHandle)
    {
        _enemyFacade.DefeatEnemy(enemyHandle);
    }

    // Obstacle
    public IObservable<Unit> OnPlayerHitObstacle
        => _obstacleFacade.OnPlayerHitObstacle;

}
