using System;
using UniRx;

public interface IEnemyFacade
{
    IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged { get; }

}

public class EnemyFacade : IEnemyFacade
{
    private readonly EnemyManager _enemyManager;
    public EnemyFacade(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;

    }

    public IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged => _enemyManager.EnemyCount;


}