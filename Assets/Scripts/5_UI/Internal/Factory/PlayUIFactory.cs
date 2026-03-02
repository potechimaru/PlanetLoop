using UnityEngine;

public sealed class PlayUIFactory
{
    [Header("Pools")]
    private EnemyDefeatedPointPool _enemyDefeatedPointPool;
    private NewOrbitPointPool _newOrbitPointPool;

    public PlayUIFactory(EnemyDefeatedPointPool enemyDefeatedPointPool, NewOrbitPointPool newOrbitPointPool) 
    {
        _enemyDefeatedPointPool = enemyDefeatedPointPool;
        _newOrbitPointPool = newOrbitPointPool;

    }

    /// <summary>
    /// 指定UIを出現（UIの anchoredPosition 指定）
    /// </summary>
    public RectTransform Spawn(PlayUIType type, Vector2 anchoredPos, RectTransform parent = null)
    {
        switch (type)
        {
            case PlayUIType.EnemyDefeated:
                if (_enemyDefeatedPointPool == null)
                {
                    return null;
                }
                return _enemyDefeatedPointPool.Rent(anchoredPos, parent);

            case PlayUIType.NewOrbitPoint:
                if (_newOrbitPointPool == null)
                {
                    return null;
                }
                return _newOrbitPointPool.Rent(anchoredPos, parent);

            default:
                return null;
        }
    }

    /// <summary>
    /// World座標から出したい場合（ScreenSpace-Overlay想定）
    /// ※CanvasがScreenSpace-Camera/WorldSpaceの場合は変換が変わるので注意
    /// </summary>
    public RectTransform SpawnFromWorld(
        PlayUIType type,
        Vector3 worldPos,
        Camera worldCamera,
        RectTransform canvasRect,
        RectTransform parent = null)
    {
        if (worldCamera == null || canvasRect == null)
        {
            return null;
        }

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPos, null, out Vector2 localPos))
        {
            return null;
        }

        return Spawn(type, localPos, parent);
    }
}