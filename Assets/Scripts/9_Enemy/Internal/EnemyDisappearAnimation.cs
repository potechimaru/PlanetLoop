using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Threading;
using System;

public class EnemyDisappearAnimation : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.InBack;

    private Sequence _sequence;

    private void Awake()
    {
        if (target == null)
            target = transform;
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        if (target == null)
            target = transform;

        _sequence?.Kill();
        _sequence = null;

        Vector3 startScale = target.localScale;
        Vector3 endScale = new Vector3(0f, startScale.y, startScale.z);

        _sequence = DOTween.Sequence()
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        _sequence.Join(
            target.DOScale(endScale, duration)
                .SetEase(ease)
        );

        using var registration = cancellationToken.Register(() =>
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
            }
        });

        try
        {
            await UniTask.WaitUntil(
                () => _sequence == null || !_sequence.IsActive() || !_sequence.IsPlaying(),
                cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (_sequence != null && _sequence.IsActive())
            {
                _sequence.Kill();
            }

            target.localScale = endScale;
            throw;
        }
        catch(Exception ex) 
        {
            Debug.LogException(ex);

        }
        finally
        {
            _sequence = null;
        }
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}