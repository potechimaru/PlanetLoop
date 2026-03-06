using UnityEngine;

public class EnemyViewCommon : MonoBehaviour
{
    [Header("Telegraph Guide")]
    [SerializeField] private EnemyTelegraphGuide telegraph;

    [Header("Bullet")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float bulletSpeed = 6f;

    private void Awake()
    {
        // Žè‚ÅŽh‚µ–Y‚ê‚Ä‚à“®‚­‚æ‚¤‚É•ÛŒ¯
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
}