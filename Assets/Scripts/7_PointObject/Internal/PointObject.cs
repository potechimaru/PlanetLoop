using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(PointCollectMover))]
public class PointObject : MonoBehaviour
{
    [SerializeField] private PointObjectType _type = PointObjectType.Normal;

    private readonly Subject<PointObjectType> _onTriggered = new();
    public IObservable<PointObjectType> OnTriggered => _onTriggered;

    private bool _collected;
    private Collider2D _collider;
    private PointCollectMover _mover;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;

        // スコア通知は即時
        _onTriggered.OnNext(_type);

        // 再衝突防止
        if (_collider != null)
            _collider.enabled = false;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _onTriggered.OnCompleted();
        _onTriggered.Dispose();
    }
}