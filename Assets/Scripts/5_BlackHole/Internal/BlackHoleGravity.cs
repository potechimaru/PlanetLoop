using UnityEngine;

public class BlackHoleGravity : MonoBehaviour
{
    [Header("Radii (world units)")]
    [Tooltip("ここより外は引力=0")]
    public float outerRadius = 6f;

    [Tooltip("ここより内は最大引力")]
    public float innerRadius = 3f;

    [Header("Strength")]
    [Tooltip("方向を曲げる強さ（加速度相当）。速度そのものは変えない")]
    public float maxTurnAccel = 20f;

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color outerColor = new Color(0f, 1f, 1f, 1f);
    public Color innerColor = new Color(1f, 0.5f, 0f, 1f);

    /// <summary>
    /// 射出中の移動方向(dir)を、ブラックホール中心へ曲げる。
    /// 速度一定要件に合わせて dir は正規化して返す（speedは別管理）。
    /// </summary>
    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        // XY平面前提（Zは固定）
        Vector2 p = new Vector2(worldPos.x, worldPos.y);
        Vector2 c = new Vector2(transform.position.x, transform.position.y);

        Vector2 toCenter = (c - p);
        float dist = toCenter.magnitude;

        if (dist <= 1e-5f) return dir;

        // 外側はゼロ
        if (dist >= outerRadius) return dir;

        // 0..1（外=0、内=1）を滑らかに
        // dist <= innerRadius => 1
        // dist >= outerRadius => 0
        float t = Mathf.InverseLerp(outerRadius, innerRadius, dist);
        // SmoothStepで連続・滑らか
        float w = Mathf.SmoothStep(0f, 1f, t);

        Vector2 pullDir = toCenter / dist; // 単位ベクトル
        Vector3 a = new Vector3(pullDir.x, pullDir.y, 0f) * (maxTurnAccel * w);

        // 方向ベクトルだけ更新（速度は別で保持）
        Vector3 newDir = (dir + a * dt).normalized;

        // 2D運用ならZは0固定にしておくと事故りにくい
        newDir.z = 0f;
        return newDir.normalized;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        // outer: 引力ゼロ境界
        Gizmos.color = outerColor;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        // inner: キリが良い境界（最大になる境界）
        Gizmos.color = innerColor;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

    private void OnValidate()
    {
        // 逆転防止
        outerRadius = Mathf.Max(outerRadius, 0.01f);
        innerRadius = Mathf.Clamp(innerRadius, 0.01f, outerRadius - 0.001f);
        maxTurnAccel = Mathf.Max(0f, maxTurnAccel);
    }
}