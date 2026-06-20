using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class LaserAttackStrategy : IAttackStrategy
{
    private readonly EnemyController _controller;

    private Vector3 _cachedDir;

    public LaserAttackStrategy(
        EnemyController controller)
    {
        _controller = controller;
    }

    public UniTask OnEnterTelegraph()
    {
        _cachedDir = _controller.DirToPlayerNormalized();

        _controller.ShowTelegraph(_cachedDir);

        return UniTask.CompletedTask;
    }

    public void TickTelegraph()
    {
        _cachedDir = _controller.DirToPlayerNormalized();

        _controller.ShowTelegraph(_cachedDir);
    }

    public UniTask Fire()
    {
        _controller.HideTelegraph();
        var dir = _controller.DirToPlayerNormalized();
        _controller.FireLaser(dir);
        return UniTask.CompletedTask;
    }
}