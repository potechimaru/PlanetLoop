using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class EnemyView : MonoBehaviour
{
    [Header("Telegraph Guides")]
    [SerializeField] private List<EnemyTelegraphGuide> telegraphs = new();

    [Header("Decoration")]
    [SerializeField] private SpriteRenderer _aroundEnemy;

    [SerializeField] private float _rotateSpeed = 120f;
    [SerializeField] private EnemyDisappearAnimation _disappearAnimation;
    [SerializeField] private ParticleSystem _particleSystem;

    [Header("Rendering")]
    [SerializeField] private SpriteRenderer[] _spriteRenderers;

    private bool _isRenderingEnabled = true;

    private Tween _rotateTween;

    private void Awake()
    {
        if (telegraphs == null || telegraphs.Count == 0)
        {
            telegraphs = new List<EnemyTelegraphGuide>(
                GetComponentsInChildren<EnemyTelegraphGuide>(true)
            );
        }

        if (_spriteRenderers == null || _spriteRenderers.Length == 0)
        {
            _spriteRenderers =
                GetComponentsInChildren<SpriteRenderer>(true);
        }

        HideTelegraph();
    }

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        ShowTelegraphs(new[] { dirNormalized });
    }

    public void ShowTelegraphs(IReadOnlyList<Vector3> directions)
    {
        if (telegraphs == null) return;

        for (int i = 0; i < telegraphs.Count; i++)
        {
            if (telegraphs[i] == null) continue;

            if (i < directions.Count)
            {
                telegraphs[i].Show(transform.position, directions[i]);
            }
            else
            {
                telegraphs[i].Hide();
            }
        }
    }

    public void HideTelegraph()
    {
        HideTelegraphs();
    }

    public void HideTelegraphs()
    {
        if (telegraphs == null) return;

        foreach (var telegraph in telegraphs)
        {
            if (telegraph == null) continue;
            telegraph.Hide();
        }
    }

    public void RotateDecoration()
    {
        if (_aroundEnemy == null) return;

        _rotateTween?.Kill();

        float duration = 360f / _rotateSpeed;

        _rotateTween = _aroundEnemy.transform
            .DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Ease.Linear)
            .SetLoops(-1);
    }

    public void StopRotateDecoration()
    {
        _rotateTween?.Kill();
        _rotateTween = null;
    }

    public async UniTask PlayDisappearParticleAsync()
    {
        if (_particleSystem == null) return;

        _particleSystem.Play();

        await UniTask.WaitUntil(() =>
            !_particleSystem.IsAlive(true)
        );
    }

    public async UniTask PlayDisappearAnimationAsync()
    {
        if (_disappearAnimation == null) return;
        await _disappearAnimation.PlayAsync();
    }

    public void SetRenderingEnabled(bool enabled)
    {
        if (_isRenderingEnabled == enabled)
            return;

        _isRenderingEnabled = enabled;

        if (_spriteRenderers == null)
            return;

        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            if (_spriteRenderers[i] == null)
                continue;

            _spriteRenderers[i].enabled = enabled;
        }
    }

    private void OnDestroy()
    {
        _rotateTween?.Kill();
    }
}