using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private ClosedSplineLine _spline;
    [SerializeField] private bool _useLocalPlaneXY = true;
    [SerializeField] private JumpNormalGuide _jumpNormalGuide;

    [SerializeField] private Material _auraMaterialBlue;
    [SerializeField] private Material _auraMaterialOrange;
    [SerializeField] private Material _auraMaterialRed;

    [SerializeField] private ParticleSystem _deadEffect;
    [SerializeField] private PlayerDisappearAnimation _playerDisappearAnimation;

    //[SerializeField] private ContinuousRotateAnimation _continuousRotateAnimation;

    private MeshRenderer _auraRenderer;

    public ClosedSplineLine Spline => _spline;
    public bool UseLocalPlaneXY => _useLocalPlaneXY;

    private void Awake()
    {
        _auraRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void SetPosition(Vector3 worldPos)
    {
        worldPos = new Vector3(worldPos.x, worldPos.y, worldPos.z - 0.01f);
        transform.position = worldPos;
    }

    public void SetSpline(ClosedSplineLine spline)
    {
        _spline = spline;
    }

    internal void SetAuraColor(ChargeLevel chargeLevel)
    {
        Material material = _auraMaterialBlue;
        switch (chargeLevel)
        {
            case ChargeLevel.Normal:
                material = _auraMaterialBlue;
                break;
            case ChargeLevel.Charge1:
                material = _auraMaterialOrange;
                break;
            case ChargeLevel.Charge2:
                material = _auraMaterialRed;
                break;
        }
        if (_auraRenderer != null)
            _auraRenderer.sharedMaterial = material;
    }

    public void ShowJumpNormalGuide(Vector3 normal)
    {
        _jumpNormalGuide.Show(transform.position, normal);
    }

    public void HideJumpNormalGuide()
    {
        _jumpNormalGuide.Hide();
    }

    public void PlaySplineAttachFx(ClosedSplineLine spline, float distance, Vector3 hitWorldPos)
    {
        if (spline == null) return;

        var emitter = spline.GetComponent<SplineBurstEmitter>();
        if (emitter == null) return;

        // n?p + SplineS?pi???Emitterzj
        emitter.BurstLocalAtDistance(distance);

        if (spline.IsNewOrbit && !spline.IsStartSpline)
            emitter.ScatterGlobal();
        // emitter.BurstAtWorldPos(hitWorldPos);
    }

    public async UniTask PlayDeadEffect()
    {
        var ct = this.GetCancellationTokenOnDestroy();

        try
        {
            if (this == null) return;
            if (_deadEffect == null) return;

            _deadEffect.Play();

            if (_playerDisappearAnimation != null)
            {
                await _playerDisappearAnimation
                    .PlayAsync()
                    .AttachExternalCancellation(ct);
            }

            await UniTask.WaitUntil(
                () => _deadEffect == null || !_deadEffect.IsAlive(true),
                cancellationToken: ct
            );

            if (this == null) return;

            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    //public void FlipRotateUI()
    //{
    //    _continuousRotateAnimation.FlipY();

    //}
}