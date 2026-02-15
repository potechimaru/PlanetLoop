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

    // --- Jump state ---
    private Vector3 _jumpDirection;
    private float _jumpSpeed = 5f;

    public void SetPosition(Vector3 worldPos)
    {
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

    public void StartJump(Vector3 startPos, Vector3 normal, float jumpSpeed)
    {
        transform.position = startPos;

        _jumpDirection = normal.normalized;
        _jumpSpeed = jumpSpeed;
    }

    public void TickJump(float deltaTime)
    {
        transform.position += _jumpDirection * _jumpSpeed * deltaTime;
    }

    public void ShowJumpNormalGuide(Vector3 normal)
    {
        _jumpNormalGuide.Show(transform.position, normal);
    }

    public void HideJumoNormalGuide()
    {
        _jumpNormalGuide.Hide();
    }

    //public void ClearSpline()
    //{
    //    _spline = null;
    //}
}
