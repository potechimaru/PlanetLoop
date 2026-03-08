using UnityEngine;
using DG.Tweening;

public class EnemyView : MonoBehaviour
{
    [Header("Telegraph Guide")]
    [SerializeField] private EnemyTelegraphGuide telegraph;

    [Header("Bullet")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float bulletSpeed = 6f;

    [Header("Decoration")]
    [SerializeField] private SpriteRenderer _aroundEnemy;

    [SerializeField] private float _rotateSpeed = 120f;

    private Tween _rotateTween;

    private void Awake()
    {
        if (telegraph == null)
            telegraph = GetComponentInChildren<EnemyTelegraphGuide>();
    }

    public Vector3 GetMuzzlePosition()
        => muzzle != null ? muzzle.position : transform.position;

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        if (telegraph == null) return;
        telegraph.Show(GetMuzzlePosition(), dirNormalized);
    }

    public void HideTelegraph()
    {
        if (telegraph == null) return;
        telegraph.Hide();
    }

    public void FireBullet(Vector3 dirNormalized)
    {
        if (bulletPrefab == null) return;

        var spawnPos = GetMuzzlePosition();
        var b = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        b.Launch(dirNormalized, bulletSpeed);
    }

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
}