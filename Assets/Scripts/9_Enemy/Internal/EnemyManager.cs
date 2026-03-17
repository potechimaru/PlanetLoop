using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class EnemyManager : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    private readonly HashSet<Enemy> _enemySet = new();
    private IEnumerable<Enemy> _enemies;

    private readonly ReactiveProperty<(int defeatedCount, int totalEnemyCount)> _enemyCount = new();
    public IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> EnemyCount => _enemyCount;

    private int _defeatedCount = 0;
    private int _totalEnemyCount = 0;

    public EnemyManager(IEnumerable<Enemy> enemies)
    {
        _enemies = _enemySet;

        if (enemies == null)
        {
            NotifyEnemyCountChanged();
            return;
        }

        foreach (var enemy in enemies)
        {
            Register(enemy);
        }

        _totalEnemyCount = _enemySet.Count;

        Debug.Log($"Enemy Total : {_totalEnemyCount}");

        NotifyEnemyCountChanged();
    }

    public void Register(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_enemySet.Add(enemy)) return;

        enemy.OnPlayerHit
            .Subscribe(_ =>
            {
                Unregister(enemy);
                enemy.DisableEnemy().Forget();
            })
            .AddTo(_disposables);
    }

    public void Unregister(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_enemySet.Remove(enemy)) return;

        _defeatedCount++;

        Debug.Log($"Enemy Defeated! {_defeatedCount} / {_totalEnemyCount}");

        NotifyEnemyCountChanged();
    }

    private void NotifyEnemyCountChanged()
    {
        Debug.Log($"Enemy Count Changed! Defeated: {_defeatedCount}, Total: {_totalEnemyCount}");
        _enemyCount.Value = (_defeatedCount, _totalEnemyCount);
    }

    public IReadOnlyList<Enemy> GetAll()
    {
        return _enemies.ToList();
    }

    public void ResetDefeatedCount()
    {
        _defeatedCount = 0;
        NotifyEnemyCountChanged();
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _enemyCount.Dispose();
    }
}