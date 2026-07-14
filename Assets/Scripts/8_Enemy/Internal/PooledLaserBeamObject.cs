using UnityEngine;

/// <summary>
/// LaserBeamにアタッチされるコンポーネント。プールに戻すためのインターフェースを保持する。
/// </summary>
public sealed class PooledLaserBeamObject : MonoBehaviour
{
    private ILaserBeamReturner _returner;
    private bool _isReturned;

    public void Bind(ILaserBeamReturner returner)
    {
        _returner = returner;
    }

    private void OnEnable()
    {
        _isReturned = false;
    }

    public void ReturnToPool()
    {
        if (_isReturned) return;

        _isReturned = true;
        _returner?.Return(this);
    }
}