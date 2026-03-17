using UnityEngine;
using DG.Tweening;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public class EnemyView : MonoBehaviour
{
    [Header("Telegraph Guide")]
    [SerializeField] private EnemyTelegraphGuide telegraph;

    [Header("Bullet")]
    [SerializeField] private EnemyBullet bulletPrefab;

    [Header("Decoration")]
    [SerializeField] private SpriteRenderer _aroundEnemy;

    [SerializeField] private float _rotateSpeed = 120f;

    [SerializeField] private EnemyDisappearAnimation _disappearAnimation;

    [SerializeField] private ParticleSystem _particleSystem;

    private Tween _rotateTween;

    private void Awake()
    {
        if (telegraph == null)
            telegraph = GetComponentInChildren<EnemyTelegraphGuide>();
    }

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        if (telegraph == null) return;
        telegraph.Show(transform.position, dirNormalized);
    }

    public void HideTelegraph()
    {
        if (telegraph == null) return;
        telegraph.Hide();
    }

    //public void FireBullet(Vector3 dirNormalized)
    //{
    //    if (bulletPrefab == null) return;

    //    var b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    //    b.Launch(dirNormalized, bulletSpeed);
    //}

    // ----------------------------
    // Decoration Rotation
    // ----------------------------

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
}