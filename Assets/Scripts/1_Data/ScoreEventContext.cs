using UnityEngine;

public readonly struct ScoreEventContext
{
    public readonly ScoreRuleType Type;
    public readonly Vector3 WorldPos;

    // NewOrbit 用（Spline着地）
    public readonly ClosedSplineLine FromSpline;
    public readonly ClosedSplineLine ToSpline;
    public readonly bool IsFirstLanding;
    public readonly float JumpTravelDistance;

    // DefeatEnemy 用
    public readonly int EnemyId;
    public readonly bool WasCharged;

    // PointObject 用
    public readonly int PointValue;

    public ScoreEventContext(
        ScoreRuleType type,
        Vector3 worldPos,
        ClosedSplineLine fromSpline = null,
        ClosedSplineLine toSpline = null,
        bool isFirstLanding = false,
        float jumpTravelDistance = 0f,
        int enemyId = 0,
        bool wasCharged = false,
        int pointValue = 0)
    {
        Type = type;
        WorldPos = worldPos;

        FromSpline = fromSpline;
        ToSpline = toSpline;
        IsFirstLanding = isFirstLanding;
        JumpTravelDistance = jumpTravelDistance;

        EnemyId = enemyId;
        WasCharged = wasCharged;

        PointValue = pointValue;
    }
}