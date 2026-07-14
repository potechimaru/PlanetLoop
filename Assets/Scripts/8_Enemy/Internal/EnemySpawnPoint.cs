using UnityEngine;

/// <summary>
/// Enemyがスポーンする位置を表すクラス。スポーンポイントの状態（占有されているかどうか）を管理する。
/// スポーンする候補点一個につき一つのEnemySpawnPointを用意する。
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] private Color freeColor = new(0.2f, 1f, 0.2f, 0.8f);
    [SerializeField] private Color occupiedColor = new(1f, 0.2f, 0.2f, 0.8f);

    public Vector3 Position => transform.position;
    public bool IsOccupied { get; private set; }

    public bool TryReserve()
    {
        if (IsOccupied) return false;

        IsOccupied = true;
        return true;
    }

    public void Release()
    {
        IsOccupied = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? occupiedColor : freeColor;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
#endif
}