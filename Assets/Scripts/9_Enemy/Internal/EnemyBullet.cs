using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 8f;
    [SerializeField] private float bulletSpeed = 6f;

    private readonly Subject<Unit> _onHitPlayer = new();

    private Vector3 _vel;
    private bool _isReturned;

    public IObservable<Unit> OnHitPlayer => _onHitPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (_isReturned) return;

        Debug.Log("Player hit!");

        _onHitPlayer.OnNext(Unit.Default);

        ReturnToPool();
    }

    public async UniTask Launch(Vector3 dirNormalized)
    {
        _isReturned = false;
        _vel = dirNormalized.normalized * bulletSpeed;

        try
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(lifeTime),
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );

            ReturnToPool();
        }
        catch (OperationCanceledException)
        {
            // DestroyéûÇ»Ç«ÅBäÓñ{ìIÇ…âΩÇ‡ÇµÇ»Ç¢
        }
    }

    private void Update()
    {
        transform.position += _vel * Time.deltaTime;
    }

    private void ReturnToPool()
    {
        if (_isReturned) return;

        _isReturned = true;
        _vel = Vector3.zero;

        GetComponent<PooledBulletObject>()?.ReturnToPool();
    }

    private void OnDestroy()
    {
        _onHitPlayer.Dispose();
    }
}