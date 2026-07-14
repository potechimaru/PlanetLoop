using System;
using UniRx;
using UnityEngine;

/// <summary>
/// プレイヤーが障害物に衝突した際の処理を管理する。ぶつかるとゲームオーバーになる。
/// </summary>
public class Obstacle : MonoBehaviour
{
    private readonly Subject<Obstacle> _onPlayerHit = new();
    public IObservable<Obstacle> OnPlayerHit => _onPlayerHit;

    private bool _hit = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hit) return;
        if (!other.CompareTag("Player")) return;

        _hit = true;

        _onPlayerHit.OnNext(this);

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _hit = false;
    }

    private void OnDestroy()
    {
        _onPlayerHit.OnCompleted();
        _onPlayerHit.Dispose();
    }
}