using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public sealed class PlayUIFactory
{
    [Header("Pools")]
    private EnemyDefeatedPointPool _enemyDefeatedPointPool;
    private NewOrbitPointPool _newOrbitPointPool;
    
    private IUIExternalFacade _uIExternalFacade;

    private float _offsetY = 1.5f; // UIのYオフセット（例: 敵撃破ポイントがキャラクターの頭上に表示されるように）

    public PlayUIFactory(EnemyDefeatedPointPool enemyDefeatedPointPool, NewOrbitPointPool newOrbitPointPool, IUIExternalFacade uIExternalFacade) 
    {
        _enemyDefeatedPointPool = enemyDefeatedPointPool;
        _newOrbitPointPool = newOrbitPointPool;
        _uIExternalFacade = uIExternalFacade;

        _uIExternalFacade.OnNewOrbitAttached
            .Subscribe(pos => Spawn(PlayUIType.NewOrbitPoint, new Vector2(pos.x, pos.y + _offsetY)));

        Debug.Log("PlayUIFactory initialized and subscribed to OnNewOrbitAttached event.");
    }

    /// <summary>
    /// 指定UIを出現（UIの anchoredPosition 指定）
    /// </summary>
    public void Spawn(PlayUIType type, Vector2 anchoredPos, RectTransform parent = null)
    {
        switch (type)
        {
            case PlayUIType.EnemyDefeated:
                if (_enemyDefeatedPointPool == null) return;
                _enemyDefeatedPointPool.Rent(anchoredPos, parent);
                break;

            case PlayUIType.NewOrbitPoint:
                if (_newOrbitPointPool == null) return;
                _newOrbitPointPool.Rent(anchoredPos, parent).Forget();
                break;

            default:
                return;
        }
    }

    /// <summary>
    /// World座標から出したい場合（ScreenSpace-Overlay想定）
    /// ※CanvasがScreenSpace-Camera/WorldSpaceの場合は変換が変わるので注意
    /// </summary>
    //public RectTransform SpawnFromWorld(
    //    PlayUIType type,
    //    Vector3 worldPos,
    //    Camera worldCamera,
    //    RectTransform canvasRect,
    //    RectTransform parent = null)
    //{
    //    if (worldCamera == null || canvasRect == null)
    //    {
    //        return null;
    //    }

    //    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);

    //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            canvasRect, screenPos, null, out Vector2 localPos))
    //    {
    //        return null;
    //    }

    //    return Spawn(type, localPos, parent);
    //}
}