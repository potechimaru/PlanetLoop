using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class EnemyTelegraphGuide : MonoBehaviour
{
    [Header("Line")]
    [SerializeField] private float _length = 30f;
    [SerializeField] private float _dashTiling = 1f;
    [SerializeField] private float _width = 0.08f;
    [SerializeField] private float _startOffset = 1.0f;

    [Header("Blink")]
    [SerializeField] private Color _colorA = Color.white;
    [SerializeField] private Color _colorB = Color.red;
    [SerializeField] private float _blinkInterval = 0.4f;

    private LineRenderer _line;
    private Material _material;
    private Tween _blinkTween;

    private void Awake()
    {
        _line = GetComponent<LineRenderer>();

        _line.positionCount = 2;
        _line.useWorldSpace = true;
        _line.enabled = false;
        _line.textureMode = LineTextureMode.Tile;
        _line.widthMultiplier = _width;

        _material = _line.material;
    }

    public void Show(Vector3 start, Vector3 dirNormalized)
    {
        _line.enabled = true;

        var dir = dirNormalized.normalized;
        var startPos = start + dir * _startOffset;
        var end = startPos + dir * _length;

        _line.SetPosition(0, startPos);
        _line.SetPosition(1, end);

        if (_material != null)
        {
            _material.mainTextureScale = new Vector2(_dashTiling, 1f);

            // 初回だけ色セット＋点滅開始
            if (_blinkTween == null || !_blinkTween.IsActive())
            {
                _material.SetColor("_BaseColor", _colorA);
                StartBlink();
            }
        }
    }

    private void StartBlink()
    {
        if (_material == null) return;

        _blinkTween?.Kill();

        _blinkTween = _material
            .DOColor(_colorB, "_BaseColor", _blinkInterval)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);
    }

    public void Hide()
    {
        if (_line == null) return;

        _blinkTween?.Kill();
        _blinkTween = null;

        _line.enabled = false;

        if (_material != null)
        {
            _material.SetColor("_BaseColor", _colorA);
        }
    }

    private void OnDestroy()
    {
        _blinkTween?.Kill();
    }
}