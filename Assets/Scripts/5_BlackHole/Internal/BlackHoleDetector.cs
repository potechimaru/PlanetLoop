using System;
using UniRx;
using UnityEngine;

public class BlackHoleDetector : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform _player;

    [Header("Outer Limit")]
    [SerializeField] private float _outerLimitRadius = 20f;

    [Header("Gizmos")]
    [SerializeField] private bool _showGizmos = true;

    private readonly Subject<Unit> _onPlayerEnteredBlackHole = new();
    private readonly Subject<Unit> _onPlayerExitedOuterLimit = new();

    private bool _isGameOverNotified;

    public IObservable<Unit> OnPlayerEnteredBlackHole
        => _onPlayerEnteredBlackHole;

    public IObservable<Unit> OnPlayerExitedOuterLimit
        => _onPlayerExitedOuterLimit;

    private void Update()
    {
        if (_player == null) return;
        if (_isGameOverNotified) return;

        float sqrDistance =
            (_player.position - transform.position).sqrMagnitude;

        float sqrLimit =
            _outerLimitRadius * _outerLimitRadius;

        if (sqrDistance > sqrLimit)
        {
            Debug.Log("Player exited black hole outer limit.");

            _isGameOverNotified = true;
            _onPlayerExitedOuterLimit.OnNext(Unit.Default);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isGameOverNotified) return;

        if (!collision.CompareTag("Player")) return;

        Debug.Log("Player entered black hole.");

        _isGameOverNotified = true;
        _onPlayerEnteredBlackHole.OnNext(Unit.Default);
    }

    public void ResetDetector()
    {
        _isGameOverNotified = false;
    }

    private void OnDestroy()
    {
        _onPlayerEnteredBlackHole.Dispose();
        _onPlayerExitedOuterLimit.Dispose();
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        Gizmos.DrawWireSphere(transform.position, _outerLimitRadius);
    }
}