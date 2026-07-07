using UnityEngine;

/// <summary>
/// プレイヤーがブラックホールの重力に引き寄せられる挙動を制御するクラス
/// </summary>
public class BlackHoleGravity : MonoBehaviour
{
    [Header("Radii (world units)")]
    public float outerRadius = 6f;
    public float innerRadius = 3f;

    [Header("Strength")]
    public float maxTurnAccel = 20f;

    [Header("Visual Sync")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Material backgroundMaterial;
    [SerializeField] private string centerProperty = "_Center";
    [SerializeField] private string radiusProperty = "_Radius";
    [SerializeField] private string innerRadiusProperty = "_InnerRadius";

    [Header("Gizmos")]
    public bool drawGizmos = true;
    public Color outerColor = new Color(0f, 1f, 1f, 1f);
    public Color innerColor = new Color(1f, 0.5f, 0f, 1f);

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateShaderProperties();
    }

    private void UpdateShaderProperties()
    {
        if (targetCamera == null || backgroundMaterial == null)
            return;

        Vector3 centerVp = targetCamera.WorldToViewportPoint(transform.position);

        Vector3 outerVp = targetCamera.WorldToViewportPoint(
            transform.position + Vector3.right * outerRadius
        );

        Vector3 innerVp = targetCamera.WorldToViewportPoint(
            transform.position + Vector3.right * innerRadius
        );

        float outerRadiusUv = Mathf.Abs(outerVp.x - centerVp.x);
        float innerRadiusUv = Mathf.Abs(innerVp.x - centerVp.x);

        backgroundMaterial.SetVector(centerProperty, new Vector4(centerVp.x, centerVp.y, 0f, 0f));
        backgroundMaterial.SetFloat(radiusProperty, outerRadiusUv);
        backgroundMaterial.SetFloat(innerRadiusProperty, innerRadiusUv);
    }

    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        Vector2 p = new Vector2(worldPos.x, worldPos.y);
        Vector2 c = new Vector2(transform.position.x, transform.position.y);

        Vector2 toCenter = c - p;
        float dist = toCenter.magnitude;

        if (dist <= 1e-5f) return dir;
        if (dist >= outerRadius) return dir;

        float t = Mathf.InverseLerp(outerRadius, innerRadius, dist);
        float w = Mathf.SmoothStep(0f, 1f, t);

        Vector2 pullDir = toCenter / dist;
        Vector3 a = new Vector3(pullDir.x, pullDir.y, 0f) * (maxTurnAccel * w);

        Vector3 newDir = (dir + a * dt).normalized;
        newDir.z = 0f;

        return newDir.normalized;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = outerColor;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        Gizmos.color = innerColor;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }

    private void OnValidate()
    {
        outerRadius = Mathf.Max(outerRadius, 0.01f);
        innerRadius = Mathf.Clamp(innerRadius, 0.01f, outerRadius - 0.001f);
        maxTurnAccel = Mathf.Max(0f, maxTurnAccel);
    }
}