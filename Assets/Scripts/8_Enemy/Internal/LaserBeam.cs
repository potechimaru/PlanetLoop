using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private CapsuleCollider2D capsuleCollider;

    [Header("Outer")]
    [SerializeField] private LineRenderer outerUpperLineRenderer;
    [SerializeField] private LineRenderer outerLowerLineRenderer;
    [SerializeField] private float outerThickness = 0.08f;
    [SerializeField] private float outerDistanceFromCore = 0.22f;

    [Header("Outer Particles")]
    [SerializeField] private ParticleSystem upperOuterParticle;
    [SerializeField] private ParticleSystem lowerOuterParticle;
    [SerializeField] private float particleOutsideOffset = 0.08f;

    [Header("Laser")]
    [SerializeField] private float maxLength = 12f;
    [SerializeField] private float growDuration = 0.35f;
    [SerializeField] private float activeDuration = 0.4f;
    [SerializeField] private float disappearDuration = 0.15f;
    [SerializeField] private Ease disappearEase = Ease.InCubic;
    [SerializeField] private float fireOffset = 0.5f;

    [Header("Collision")]
    [SerializeField] private float beamWidth = 0.3f;

    private readonly Subject<Unit> _onHitPlayer = new();
    public IObservable<Unit> OnHitPlayer => _onHitPlayer;

    private bool _isHit;
    private float _currentLength;
    private Tween _disappearTween;

    private void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider2D>();

        SetupLineRenderer(lineRenderer);
        SetupLineRenderer(outerUpperLineRenderer);
        SetupLineRenderer(outerLowerLineRenderer);

        if (capsuleCollider != null)
        {
            capsuleCollider.isTrigger = true;
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.enabled = false;
        }

        StopParticle(upperOuterParticle);
        StopParticle(lowerOuterParticle);
    }

    private void SetupLineRenderer(LineRenderer target)
    {
        if (target == null) return;

        target.useWorldSpace = false;
        target.positionCount = 2;
        target.enabled = false;
    }

    public async UniTask PlayAsync(
        Vector3 startPos,
        Vector3 direction,
        CancellationToken cancellationToken)
    {
        _isHit = false;

        _disappearTween?.Kill();
        _disappearTween = null;

        direction = direction.normalized;

        transform.position = startPos + direction * fireOffset;
        transform.right = direction;

        EnableLaserVisuals();

        if (capsuleCollider != null)
            capsuleCollider.enabled = true;

        SetWidthScale(1f);

        float elapsed = 0f;

        while (elapsed < growDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / growDuration);
            float currentLength = maxLength * t;

            SetLaserLength(currentLength);
            SetWidthScale(1f);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        SetLaserLength(maxLength);
        SetWidthScale(1f);

        float activeElapsed = 0f;

        while (activeElapsed < activeDuration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            activeElapsed += Time.deltaTime;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        await DisappearAsync(cancellationToken);

        ReturnToPool();
    }

    private async UniTask DisappearAsync(CancellationToken cancellationToken)
    {
        StopParticle(upperOuterParticle);
        StopParticle(lowerOuterParticle);

        float widthScale = 1f;

        _disappearTween?.Kill();

        _disappearTween = DOTween.To(
                () => widthScale,
                x =>
                {
                    widthScale = x;
                    SetWidthScale(widthScale);
                },
                0f,
                disappearDuration)
            .SetEase(disappearEase);

        try
        {
            while (_disappearTween != null && _disappearTween.IsActive() && !_disappearTween.IsComplete())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _disappearTween?.Kill();
            throw;
        }

        SetWidthScale(0f);
    }

    private void EnableLaserVisuals()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = true;

        if (outerUpperLineRenderer != null)
            outerUpperLineRenderer.enabled = true;

        if (outerLowerLineRenderer != null)
            outerLowerLineRenderer.enabled = true;

        PlayParticle(upperOuterParticle);
        PlayParticle(lowerOuterParticle);
    }

    private void SetLaserLength(float length)
    {
        _currentLength = length;

        Vector3 startLocal = Vector3.zero;
        Vector3 endLocal = Vector3.right * length;

        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startLocal);
            lineRenderer.SetPosition(1, endLocal);
        }

        SetOuterLine(outerUpperLineRenderer, length, outerDistanceFromCore);
        SetOuterLine(outerLowerLineRenderer, length, -outerDistanceFromCore);

        SetOuterParticlePositions(length);

        if (capsuleCollider == null) return;

        capsuleCollider.offset = new Vector2(length * 0.5f, 0f);
    }

    private void SetWidthScale(float scale)
    {
        scale = Mathf.Clamp01(scale);

        if (lineRenderer != null)
        {
            lineRenderer.startWidth = beamWidth * scale;
            lineRenderer.endWidth = beamWidth * scale;
        }

        float scaledOuterThickness = outerThickness * scale;
        float scaledOuterDistance = outerDistanceFromCore * scale;

        SetOuterLineWidthAndOffset(
            outerUpperLineRenderer,
            scaledOuterThickness,
            scaledOuterDistance);

        SetOuterLineWidthAndOffset(
            outerLowerLineRenderer,
            scaledOuterThickness,
            -scaledOuterDistance);

        if (capsuleCollider != null)
        {
            capsuleCollider.size = new Vector2(
                _currentLength,
                beamWidth * scale
            );

            capsuleCollider.offset = new Vector2(
                _currentLength * 0.5f,
                0f
            );

            capsuleCollider.enabled = scale > 0.01f;
        }
    }

    private void SetOuterLine(
        LineRenderer target,
        float length,
        float yOffset)
    {
        if (target == null) return;

        target.startWidth = outerThickness;
        target.endWidth = outerThickness;

        Vector3 startLocal = new Vector3(0f, yOffset, 0f);
        Vector3 endLocal = new Vector3(length, yOffset, 0f);

        target.SetPosition(0, startLocal);
        target.SetPosition(1, endLocal);
    }

    private void SetOuterLineWidthAndOffset(
        LineRenderer target,
        float thickness,
        float yOffset)
    {
        if (target == null) return;

        target.startWidth = thickness;
        target.endWidth = thickness;

        Vector3 startLocal = new Vector3(0f, yOffset, 0f);
        Vector3 endLocal = new Vector3(_currentLength, yOffset, 0f);

        target.SetPosition(0, startLocal);
        target.SetPosition(1, endLocal);
    }

    private void SetOuterParticlePositions(float length)
    {
        float halfOuterThickness = outerThickness * 0.5f;

        float upperY =
            outerDistanceFromCore + halfOuterThickness + particleOutsideOffset;

        float lowerY =
            -outerDistanceFromCore - halfOuterThickness - particleOutsideOffset;

        if (upperOuterParticle != null)
        {
            upperOuterParticle.transform.localPosition =
                new Vector3(length * 0.5f, upperY, 0f);

            var shape = upperOuterParticle.shape;
            Vector3 scale = shape.scale;
            scale.x = length;
            shape.scale = scale;
        }

        if (lowerOuterParticle != null)
        {
            lowerOuterParticle.transform.localPosition =
                new Vector3(length * 0.5f, lowerY, 0f);

            var shape = lowerOuterParticle.shape;
            Vector3 scale = shape.scale;
            scale.x = length;
            shape.scale = scale;
        }
    }

    private void PlayParticle(ParticleSystem particle)
    {
        if (particle == null) return;
        if (particle.isPlaying) return;

        particle.Play();
    }

    private void StopParticle(ParticleSystem particle)
    {
        if (particle == null) return;

        particle.Stop(
            true,
            ParticleSystemStopBehavior.StopEmitting
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isHit) return;
        if (!collision.CompareTag("Player")) return;

        _isHit = true;
        _onHitPlayer.OnNext(Unit.Default);
    }

    public void ReturnToPool()
    {
        Hide();
        GetComponent<PooledLaserBeamObject>()?.ReturnToPool();
    }

    public void Hide()
    {
        _disappearTween?.Kill();
        _disappearTween = null;

        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (outerUpperLineRenderer != null)
            outerUpperLineRenderer.enabled = false;

        if (outerLowerLineRenderer != null)
            outerLowerLineRenderer.enabled = false;

        if (capsuleCollider != null)
            capsuleCollider.enabled = false;

        if (upperOuterParticle != null)
            upperOuterParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (lowerOuterParticle != null)
            lowerOuterParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnDestroy()
    {
        _disappearTween?.Kill();
        _onHitPlayer.Dispose();
    }
}