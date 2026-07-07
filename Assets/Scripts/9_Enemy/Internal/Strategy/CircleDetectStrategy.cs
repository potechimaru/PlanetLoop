using UnityEngine;

/// <summary>
/// EnemyのDetectStrategyで、プレイヤーを円形範囲で判定するStrategyクラス
/// </summary>
public sealed class CircleDetectStrategy : IDetectStrategy
{
    private readonly EnemyController _enemyController;

    public CircleDetectStrategy(EnemyController enemyController) => _enemyController = enemyController;

    public bool IsDetected()
    {
        if (_enemyController.Player == null) return false;
        return Vector3.Distance(_enemyController.Self.position, _enemyController.Player.position) <= _enemyController.DetectRadius;
    }
}