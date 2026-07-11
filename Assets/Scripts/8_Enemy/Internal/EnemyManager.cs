using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using VContainer;

public class EnemyManager : IDisposable
{
    private readonly IObjectResolver _resolver;
    private readonly CompositeDisposable _disposables = new();

    private readonly HashSet<Enemy> _enemySet = new();

    private readonly ReactiveProperty<(int defeatedCount, int totalEnemyCount)> _enemyCount = new();
    public IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> EnemyCount => _enemyCount;

    private readonly Subject<Unit> _onEnemyDefeated = new();
    public IObservable<Unit> OnEnemyDefeated => _onEnemyDefeated;

    private readonly Subject<IEnemyContactHandle> _onPlayerTouchedEnemy = new();
    public IObservable<IEnemyContactHandle> OnPlayerTouchedEnemy => _onPlayerTouchedEnemy;

    private int _defeatedCount = 0;
    private int _totalEnemyCount = 0;

    private readonly EnemyFactory _enemyFactory;

    private bool _isDetectionEnabled = true;

    public EnemyManager(
    IObjectResolver resolver,
    EnemyFactory enemyFactory,
    IEnumerable<Enemy> enemies)
    {
        _resolver = resolver;
        _enemyFactory = enemyFactory;

        if (enemies != null)
        {
            foreach (var enemy in enemies)
            {
                _resolver.Inject(enemy);
                Register(enemy);
            }
        }

        _totalEnemyCount = _enemySet.Count;
        NotifyEnemyCountChanged();
    }

    public void Register(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_enemySet.Add(enemy)) return;

        enemy.OnPlayerTouched
            .Subscribe(_ =>
            {
                _onPlayerTouchedEnemy.OnNext(enemy);
            })
            .AddTo(_disposables);
    }

    public void Unregister(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_enemySet.Remove(enemy)) return;

        _defeatedCount++;

        _onEnemyDefeated.OnNext(Unit.Default);

        NotifyEnemyCountChanged();
    }

    public Enemy SpawnRandomEnemyFromTypes(
    Vector3 position,
    IReadOnlyList<EnemyType> enemyTypes)
    {
        var enemy = _enemyFactory.CreateRandomFromTypes(enemyTypes, position);
        if (enemy == null) return null;

        Register(enemy);

        enemy.SetDetectionEnabled(_isDetectionEnabled);

        _totalEnemyCount++;
        NotifyEnemyCountChanged();

        return enemy;
    }

    public Enemy SpawnRandomEnemy(Vector3 position)
    {
        var enemy = _enemyFactory.CreateRandom(position);
        if (enemy == null) return null;

        Register(enemy);

        enemy.SetDetectionEnabled(_isDetectionEnabled);

        _totalEnemyCount++;
        NotifyEnemyCountChanged();

        return enemy;
    }

    public void SetAllDetectionEnabled(bool enabled)
    {
        _isDetectionEnabled = enabled;

        foreach (var enemy in _enemySet)
        {
            if (enemy == null) continue;
            enemy.SetDetectionEnabled(enabled);
        }
    }

    private void NotifyEnemyCountChanged()
    {
        _enemyCount.Value = (_defeatedCount, _totalEnemyCount);
    }

    public IReadOnlyList<Enemy> GetAll()
    {
        return _enemySet.ToList();
    }

    public void DefeatEnemy(IEnemyContactHandle handle)
    {
        if (handle is not Enemy enemy) return;
        if (enemy == null) return;
        if (!_enemySet.Contains(enemy)) return;

        Unregister(enemy);
        handle.Defeat();
    }

    public void ResetDefeatedCount()
    {
        _defeatedCount = 0;
        NotifyEnemyCountChanged();
    }

    public void UpdateEnemySimulationByDistance(
    Vector3 playerPosition,
    float activeRadius)
    {
        float activeRadiusSqr = activeRadius * activeRadius;

        foreach (var enemy in _enemySet)
        {
            if (enemy == null) continue;

            float sqrDistance =
                (enemy.transform.position - playerPosition).sqrMagnitude;

            bool shouldActive = sqrDistance <= activeRadiusSqr;

            enemy.SetSimulationEnabled(shouldActive);
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _enemyCount.Dispose();
        _onEnemyDefeated.Dispose();
        _onPlayerTouchedEnemy.Dispose();
    }
}