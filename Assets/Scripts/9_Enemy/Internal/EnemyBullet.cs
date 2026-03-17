using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private float bulletSpeed = 6f;
    private Vector3 _vel;

    private CancellationToken _cancellationToken;

    public async UniTask Launch(Vector3 dirNormalized)
    {
        _vel = dirNormalized * bulletSpeed;
         
        try 
        {
            await UniTask.Delay(TimeSpan.FromSeconds(lifeTime), cancellationToken: _cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // キャンセルされた場合は何もしない
        }
        finally
        {
            gameObject.GetComponent<PooledBulletObject>()?.ReturnToPool();
        }
    }

    private void Update()
    {
        transform.position += _vel * Time.deltaTime;
    }

    // ここでPlayerに当たったら即死、などはプロジェクト側のルールに合わせて実装
}