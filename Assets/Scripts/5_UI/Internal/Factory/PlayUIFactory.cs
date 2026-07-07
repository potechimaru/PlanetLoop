using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Point獲得時のUIを生成するFactory。多種類。
/// </summary>
public sealed class PlayUIFactory
{
    [Header("Pools")]
    private DefeatEnemyPointPool _enemyDefeatedPointPool;
    private NewOrbitPointPool _newOrbitPointPool;
    private LongJumpPointPool _longJumpPointPool;
    private VeryHighPointPool _veryHighPointPool;
    private HighPointPool _highPointPool;
    private MediumPointPool _mediumPointPool;
    private LowPointPool _lowPointPool;

    private IUIExternalFacade _uIExternalFacade;

    private float _offsetY = 1.5f; // UIのYオフセット（例: 敵撃破ポイントがキャラクターの頭上に表示されるように）

    public PlayUIFactory(DefeatEnemyPointPool enemyDefeatedPointPool,
                        NewOrbitPointPool newOrbitPointPool,
                        LongJumpPointPool longJumpPointPool,
                        VeryHighPointPool veryHighPointPool,
                        HighPointPool highPointPool,
                        MediumPointPool mediumPointPool,
                        LowPointPool lowPointPool,
                        IUIExternalFacade uIExternalFacade) 
    {
        _enemyDefeatedPointPool = enemyDefeatedPointPool;
        _newOrbitPointPool = newOrbitPointPool;
        _longJumpPointPool = longJumpPointPool;

        _veryHighPointPool = veryHighPointPool;
        _highPointPool = highPointPool;
        _mediumPointPool = mediumPointPool;
        _lowPointPool = lowPointPool;

        _uIExternalFacade = uIExternalFacade;


        //_uIExternalFacade.OnNewOrbitAttached
        //    .Subscribe(pos => Spawn(PlayUIType.NewOrbitPoint, new Vector2(pos.x, pos.y + _offsetY)));

        //_uIExternalFacade.OnLongJumped
        //    .Subscribe(pos => Spawn(PlayUIType.LongJumpPoint, new Vector2(pos.x, pos.y + _offsetY)));

        //_uIExternalFacade.OnEnemyDefeated
        //    .Subscribe(pos => Spawn(PlayUIType.DefeatEnemyPoint, new Vector2(pos.x, pos.y + _offsetY)));

        //Debug.Log("PlayUIFactory initialized and subscribed to OnNewOrbitAttached event.");
    }

    /// <summary>
    /// 指定UIを出現（UIの anchoredPosition 指定）
    /// </summary>
    public void Spawn(PlayUIType type, Vector2 anchoredPos, RectTransform parent = null)
    {
        switch (type)
        {
            case PlayUIType.DefeatEnemyPoint:
                if (_enemyDefeatedPointPool == null) return;
                _enemyDefeatedPointPool.Rent(anchoredPos, parent).Forget();
                break;

            case PlayUIType.NewOrbitPoint:
                if (_newOrbitPointPool == null) return;
                _newOrbitPointPool.Rent(anchoredPos, parent).Forget();
                break;

            case PlayUIType.LongJumpPoint:
                if (_longJumpPointPool == null) return;
                _longJumpPointPool.Rent(anchoredPos, parent).Forget();
                break;

            case PlayUIType.PointVeryHigh:
                if (_veryHighPointPool == null) return;
                _veryHighPointPool.Rent(anchoredPos, parent).Forget();
                //Debug.Log($"Spawned VeryHighPoint at {anchoredPos} with parent {parent?.name ?? "null"}");
                break;

            case PlayUIType.PointHigh:
                //Debug.Log($"High Point Pool :{_highPointPool == null}");
                if (_highPointPool == null) return;
                _highPointPool.Rent(anchoredPos, parent).Forget();
                //Debug.Log($"Spawned HighPoint at {anchoredPos} with parent {parent?.name ?? "null"}");
                break;

            case PlayUIType.PointLow:
                //Debug.Log($"Low Point Pool :{_highPointPool == null}");
                if (_lowPointPool == null) return;
                _lowPointPool.Rent(anchoredPos, parent).Forget();
                //Debug.Log($"Spawned LowPoint at {anchoredPos} with parent {parent?.name ?? "null"}");
                break;

            case PlayUIType.PointMedium:
                if (_mediumPointPool == null) return;
                _mediumPointPool.Rent(anchoredPos, parent).Forget();
                //Debug.Log($"Spawned MediumPoint at {anchoredPos} with parent {parent?.name ?? "null"}");
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