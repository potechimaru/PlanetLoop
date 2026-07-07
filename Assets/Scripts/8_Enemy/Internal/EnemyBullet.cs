using System;
using UniRx;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 6f;

    private readonly Subject<Unit> _onHitPlayer = new();

    private Vector3 _vel;
    private bool _isReturned;

    private Transform _rangeCenter;
    private float _activeRadiusSqr;

    public IObservable<Unit> OnHitPlayer => _onHitPlayer;

    public void Launch(
        Vector3 dirNormalized,
        Transform rangeCenter,
        float activeRadius)
    {
        _isReturned = false;

        _rangeCenter = rangeCenter;
        _activeRadiusSqr = activeRadius * activeRadius;

        _vel = dirNormalized.normalized * bulletSpeed;
    }

    private void Update()
    {
        if (_isReturned) return;

        transform.position += _vel * Time.deltaTime;

        CheckOutOfRange();
    }

    private void CheckOutOfRange()
    {
        if (_rangeCenter == null) return;

        float sqrDistance =
            (transform.position - _rangeCenter.position).sqrMagnitude;

        if (sqrDistance > _activeRadiusSqr)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (_isReturned) return;

        _onHitPlayer.OnNext(Unit.Default);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReturned) return;

        _isReturned = true;
        _vel = Vector3.zero;
        _rangeCenter = null;

        GetComponent<PooledBulletObject>()?.ReturnToPool();
    }

    private void OnDestroy()
    {
        _onHitPlayer.Dispose();
    }
}