using System;
using UniRx;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private readonly Subject<Obstacle> _onPlayerHit = new();
    public IObservable<Obstacle> OnPlayerHit => _onPlayerHit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _onPlayerHit.OnNext(this);
    }

    private void OnDestroy()
    {
        _onPlayerHit.OnCompleted();
        _onPlayerHit.Dispose();
    }
}