using UnityEngine;

public class PooledPlayUIItem : MonoBehaviour
{
    private IPlayUIReturner _returner;

    public void Bind(IPlayUIReturner returner)
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

