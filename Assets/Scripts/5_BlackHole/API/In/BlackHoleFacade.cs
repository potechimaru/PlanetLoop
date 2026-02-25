using UnityEngine;

public interface IBlackHoleFacade
{
    Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt);

}

public class BlackHoleFacade : IBlackHoleFacade
{
    private readonly BlackHoleGravity _blackHoleGravity;

    public BlackHoleFacade(BlackHoleGravity blackHoleGravity)
    {
        _blackHoleGravity = blackHoleGravity;
    }

    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        return _blackHoleGravity.BendDirection(worldPos, dir, dt);
    }

}
