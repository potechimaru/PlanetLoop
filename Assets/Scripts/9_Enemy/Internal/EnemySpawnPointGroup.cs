using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPointGroup : MonoBehaviour
{
    private readonly List<EnemySpawnPoint> _spawnPoints = new();

    private void Awake()
    {
        RefreshSpawnPoints();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        RefreshSpawnPoints();
    }
#endif

    private void RefreshSpawnPoints()
    {
        _spawnPoints.Clear();
        GetComponentsInChildren(true, _spawnPoints);
        _spawnPoints.RemoveAll(x => x.gameObject == gameObject);
    }

    public bool TryGetRandomFreeSpawnPoint(out EnemySpawnPoint spawnPoint)
    {
        spawnPoint = null;

        var freePoints = new List<EnemySpawnPoint>();

        foreach (var point in _spawnPoints)
        {
            if (point == null) continue;
            if (point.IsOccupied) continue;

            freePoints.Add(point);
        }

        if (freePoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawnPointGroup] ãÛÇ¢ÇƒÇ¢ÇÈSpawnPointÇ™Ç†ÇËÇ‹ÇπÇÒÅB");
            return false;
        }

        int index = Random.Range(0, freePoints.Count);
        spawnPoint = freePoints[index];

        return spawnPoint.TryReserve();
    }
}