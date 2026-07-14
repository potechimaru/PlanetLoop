using UnityEngine;

/// <summary>
/// EnemyBulletにアタッチされるコンポーネント。プールに戻すためのインターフェースを保持する。
/// </summary>
public class PooledBulletObject : MonoBehaviour
{
    private IBulletReturner _returner;

    public void Bind(IBulletReturner returner)
    {
        _returner = returner;
    }

    /// <summary>外部から任意タイミングでプールへ戻す</summary>
    public void ReturnToPool()
    {
        if (_returner == null)
        {
            // 何にも紐づいていない場合は破棄
            Destroy(gameObject);
            return;
        }
        _returner.Return(this);
    }
}

