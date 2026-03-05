using UnityEngine;

public class EnemyViewCommon : MonoBehaviour
{
    [Header("Telegraph")]
    [SerializeField] private LineRenderer line;
    [SerializeField] private float telegraphLineLength = 30f;

    [Header("Bullet")]
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private Transform muzzle;
    [SerializeField] private float bulletSpeed = 6f;

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        if (line == null) return;

        line.enabled = true;
        var a = muzzle != null ? muzzle.position : transform.position;
        var b = a + dirNormalized * telegraphLineLength;

        line.positionCount = 2;
        line.SetPosition(0, a);
        line.SetPosition(1, b);
    }

    public void HideTelegraph()
    {
        if (line == null) return;
        line.enabled = false;
    }

    public void FireBullet(Vector3 dirNormalized)
    {
        if (bulletPrefab == null) return;

        var spawnPos = muzzle != null ? muzzle.position : transform.position;
        var b = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        b.Launch(dirNormalized, bulletSpeed);
    }
}