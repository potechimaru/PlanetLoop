using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// フルスクリーンのビネットやグリッチ効果をアニメーションさせるクラス（GameOpening）
/// </summary>
public class VignetteIntensityAnimation : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Volume _volume;

    [Header("Vignette Animation")]
    [SerializeField]
    private AnimationCurve _curve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 0.6f);

    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private bool _useUnscaledTime = true;

    [Header("Screen Glitch Material")]
    [SerializeField] private Material _screenGlitchMaterial;

    [Header("Custom Time")]
    [SerializeField] private float _customTimeSpeed = 1f;

    [Header("Glitch Animation Timing")]
    [SerializeField, Range(0f, 1f)]
    private float _glitchStartRate = 0.5f;

    [Header("Glitch Values")]
    [SerializeField] private float _noiseStart = 100f;
    [SerializeField] private float _noiseEnd = 0f;

    [SerializeField] private float _scanLinesStart = 0f;
    [SerializeField] private float _scanLinesEnd = 1f;

    private static readonly int NoiseAmountId =
        Shader.PropertyToID("_NoiseAmount");

    private static readonly int ScanLinesStrengthId =
        Shader.PropertyToID("_ScanLinesStrength");

    private static readonly int CustomTimeId =
        Shader.PropertyToID("_CustomTime");

    private Vignette _vignette;
    private Tween _tween;
    private float _customTime;

    private void Awake()
    {
        if (_volume == null)
            _volume = GetComponent<Volume>();

        if (_volume == null)
        {
            Debug.LogError("Volume が見つかりません。", this);
            enabled = false;
            return;
        }

        if (!_volume.profile.TryGet(out _vignette))
        {
            Debug.LogError("Volume Profile に Vignette がありません。", this);
            enabled = false;
            return;
        }

        _vignette.intensity.overrideState = true;
    }

    /// <summary>
    /// ビネットとグリッチのアニメーションを再生する
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask PlayAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            _tween?.Kill();

            float tValue = 0f;

            SetGlitchInitialValues();

            _tween = DOTween.To(
                    () => tValue,
                    x =>
                    {
                        float delta = x - tValue;
                        tValue = x;

                        UpdateCustomTime(delta);
                        UpdateVignette(tValue);
                        UpdateGlitch(tValue);
                    },
                    1f,
                    _duration)
                .SetEase(Ease.Linear)
                .SetUpdate(_useUnscaledTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            await _tween.AsyncWaitForCompletion();

            cancellationToken.ThrowIfCancellationRequested();

            SetGlitchFinalValues();
        }
        catch (OperationCanceledException)
        {
            _tween?.Kill();
        }
        catch (Exception e)
        {
            Debug.LogError(e, this);
        }
    }

    private void UpdateCustomTime(float deltaRate)
    {
        if (_screenGlitchMaterial == null)
            return;

        float deltaTime = deltaRate * _duration;
        _customTime += deltaTime * _customTimeSpeed;

        _screenGlitchMaterial.SetFloat(CustomTimeId, _customTime);
    }

    private void UpdateVignette(float t)
    {
        float intensity = _curve.Evaluate(t);
        _vignette.intensity.value = intensity;
    }

    private void UpdateGlitch(float totalRate)
    {
        if (_screenGlitchMaterial == null)
            return;

        if (totalRate < _glitchStartRate)
        {
            SetGlitchInitialValues();
            return;
        }

        float glitchRate = Mathf.InverseLerp(
            _glitchStartRate,
            1f,
            totalRate
        );

        float noiseAmount = Mathf.Lerp(
            _noiseStart,
            _noiseEnd,
            glitchRate
        );

        float scanLinesStrength = Mathf.Lerp(
            _scanLinesStart,
            _scanLinesEnd,
            glitchRate
        );

        _screenGlitchMaterial.SetFloat(NoiseAmountId, noiseAmount);
        _screenGlitchMaterial.SetFloat(ScanLinesStrengthId, scanLinesStrength);
    }

    private void SetGlitchInitialValues()
    {
        if (_screenGlitchMaterial == null)
            return;

        _screenGlitchMaterial.SetFloat(NoiseAmountId, _noiseStart);
        _screenGlitchMaterial.SetFloat(ScanLinesStrengthId, _scanLinesStart);
        _screenGlitchMaterial.SetFloat(CustomTimeId, _customTime);
    }

    private void SetGlitchFinalValues()
    {
        if (_screenGlitchMaterial == null)
            return;

        _screenGlitchMaterial.SetFloat(NoiseAmountId, _noiseEnd);
        _screenGlitchMaterial.SetFloat(ScanLinesStrengthId, _scanLinesEnd);
        _screenGlitchMaterial.SetFloat(CustomTimeId, _customTime);
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}