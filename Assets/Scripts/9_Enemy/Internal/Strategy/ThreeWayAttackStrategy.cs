using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Enemy2,4で汎用的に使うAttackStrategy。プレイヤーの方向に3方向に弾を撃つ。
/// </summary>
public sealed class ThreeWayAttackStrategy : IAttackStrategy
{
    private readonly EnemyController _ctx;

    private readonly float _spreadAngle;

    public ThreeWayAttackStrategy(
        EnemyController ctx,
        float spreadAngle = 25f)
    {
        _ctx = ctx;
        _spreadAngle = spreadAngle;
    }

    public UniTask OnEnterTelegraph()
    {
        return UniTask.CompletedTask;
    }

    public void TickTelegraph()
    {
        var dirs = CreateDirections();
        _ctx.ShowTelegraphs(dirs);
    }

    public UniTask Fire()
    {
        _ctx.HideTelegraphs();

        var dirs = CreateDirections();

        foreach (var dir in dirs)
        {
            _ctx.FireBullet(dir);
        }

        return UniTask.CompletedTask;
    }

    private Vector3[] CreateDirections()
    {
        var center = _ctx.DirToPlayerNormalized();

        return new[]
        {
            RotateZ(center, -_spreadAngle),
            center,
            RotateZ(center, _spreadAngle),
        };
    }

    private static Vector3 RotateZ(Vector3 v, float angleDeg)
    {
        return Quaternion.Euler(0f, 0f, angleDeg) * v.normalized;
    }
}