using System;
using UniRx;
using UnityEngine;

public class PointObject : MonoBehaviour
{
    [SerializeField] private PointObjectType _type = PointObjectType.Medium;

    private readonly Subject<PointObjectType> _onTriggered = new();
    public IObservable<PointObjectType> OnTriggered => _onTriggered;

    private bool _collected;
    private bool _canCollect;

    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    public void SetCollectEnabled(bool enabled)
    {
        _canCollect = enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_canCollect) return;

        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;

        _onTriggered.OnNext(_type);

        if (_collider != null)
            _collider.enabled = false;

        gameObject.SetActive(false);
    }

    public void ResetPoint()
    {
        _collected = false;

        if (_collider != null)
            _collider.enabled = true;

        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _onTriggered.OnCompleted();
        _onTriggered.Dispose();
    }
}