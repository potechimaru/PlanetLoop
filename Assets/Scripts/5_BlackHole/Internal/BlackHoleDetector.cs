using System;
using UniRx;
using UnityEngine;

/// <summary>
/// プレイヤーがブラックホールの外側制限もしくは内側に入ったかどうかを検知するクラス
/// プレイヤーはゲームオーバーとなる。また、ブラックホールの外側制限に入った場合は、画面が赤くなる演出を行う。
/// </summary>
public class BlackHoleDetector : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform _player;

    [Header("Outer Limit")]
    [SerializeField] private float _outerLimitRadius = 20f;

    [Header("Gizmos")]
    [SerializeField] private bool _showGizmos = true;

    [Header("Outer Limit Visual")]
    [SerializeField] private Camera _targetCamera;
    [SerializeField] private Material _fullScreenMaterial;

    [SerializeField, Range(0f, 1f)]
    private float _outerLimitRedStrength = 0.4f;

    [SerializeField]
    private Color _outerLimitRedColor = new Color(1f, 0f, 0f, 1f);

    private static readonly int BlackHoleScreenCenterId =
        Shader.PropertyToID("_BlackHoleScreenCenter");

    private static readonly int OuterLimitScreenRadiusId =
        Shader.PropertyToID("_OuterLimitScreenRadius");

    private static readonly int OuterLimitRedStrengthId =
        Shader.PropertyToID("_OuterLimitRedStrength");

    private static readonly int OuterLimitRedColorId =
        Shader.PropertyToID("_OuterLimitRedColor");

    private static readonly int AspectRatioId =
    Shader.PropertyToID("_AspectRatio");

    private readonly Subject<Unit> _onPlayerEnteredBlackHole = new();
    private readonly Subject<Unit> _onPlayerExitedOuterLimit = new();

    private bool _isGameOverNotified;

    public IObservable<Unit> OnPlayerEnteredBlackHole
        => _onPlayerEnteredBlackHole;

    public IObservable<Unit> OnPlayerExitedOuterLimit
        => _onPlayerExitedOuterLimit;

    private void Awake()
    {
        if (_targetCamera == null)
            _targetCamera = Camera.main;
    }

    private void Update()
    {
        UpdateOuterLimitVisual();

        if (_player == null) return;
        if (_isGameOverNotified) return;

        float sqrDistance =
            (_player.position - transform.position).sqrMagnitude;

        float sqrLimit =
            _outerLimitRadius * _outerLimitRadius;

        if (sqrDistance > sqrLimit)
        {
            //Debug.Log("Player exited black hole outer limit.");

            _isGameOverNotified = true;
            _onPlayerExitedOuterLimit.OnNext(Unit.Default);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isGameOverNotified) return;
        if (!collision.CompareTag("Player")) return;

        //Debug.Log("Player entered black hole.");

        _isGameOverNotified = true;
        _onPlayerEnteredBlackHole.OnNext(Unit.Default);
    }

    public void ResetDetector()
    {
        _isGameOverNotified = false;
    }

    private void UpdateOuterLimitVisual()
    {
        if (_targetCamera == null)
            return;

        if (_fullScreenMaterial == null)
            return;

        Vector3 centerWorld = transform.position;

        Vector3 edgeWorld =
            centerWorld + _targetCamera.transform.right * _outerLimitRadius;

        Vector3 centerViewport =
            _targetCamera.WorldToViewportPoint(centerWorld);

        Vector3 edgeViewport =
            _targetCamera.WorldToViewportPoint(edgeWorld);

        Vector2 centerUV =
            new Vector2(centerViewport.x, centerViewport.y);

        Vector2 edgeUV =
            new Vector2(edgeViewport.x, edgeViewport.y);

        float screenRadius =
            Vector2.Distance(centerUV, edgeUV);

        float aspect =
            (float)_targetCamera.pixelWidth / _targetCamera.pixelHeight;

        _fullScreenMaterial.SetVector(
            BlackHoleScreenCenterId,
            centerUV
        );

        _fullScreenMaterial.SetFloat(
            OuterLimitScreenRadiusId,
            screenRadius
        );

        _fullScreenMaterial.SetFloat(
            AspectRatioId,
            aspect
        );

        _fullScreenMaterial.SetFloat(
            OuterLimitRedStrengthId,
            _outerLimitRedStrength
        );

        _fullScreenMaterial.SetColor(
            OuterLimitRedColorId,
            _outerLimitRedColor
        );
    }

    private void OnDestroy()
    {
        _onPlayerEnteredBlackHole.Dispose();
        _onPlayerExitedOuterLimit.Dispose();
    }

    /// <summary>
    /// 禁止領域の可視化用Gizmosを描画
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;

        Gizmos.DrawWireSphere(transform.position, _outerLimitRadius);
    }
}