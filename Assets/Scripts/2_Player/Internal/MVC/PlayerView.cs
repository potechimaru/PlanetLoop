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

    public ClosedSplineLine Spline => _spline;
    public bool UseLocalPlaneXY => _useLocalPlaneXY;

    public void SetPosition(Vector3 worldPos)
    {
        // 既存仕様：少し手前に出す
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
        GetComponentInChildren<MeshRenderer>().material = material;
    }

    public void ShowJumpNormalGuide(Vector3 normal)
    {
        _jumpNormalGuide.Show(transform.position, normal);
    }

    public void HideJumoNormalGuide()
    {
        _jumpNormalGuide.Hide();
    }

    public void PlaySplineAttachFx(ClosedSplineLine spline, float distance, Vector3 hitWorldPos)
    {
        if (spline == null) return;

        var emitter = spline.GetComponent<SplineBurstEmitter>();
        if (emitter == null) return;

        // 着地近く用 + Spline全体用（種類分け済みのEmitter想定）
        emitter.BurstLocalAtDistance(distance);
        emitter.ScatterGlobal();
        // emitter.BurstAtWorldPos(hitWorldPos);
    }
}