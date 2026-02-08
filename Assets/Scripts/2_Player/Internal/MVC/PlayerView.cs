using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private ClosedSplineLine _spline;
    [SerializeField] private bool _useLocalPlaneXY = true;

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

    public void StartJump(Vector3 startPos, Vector3 normal, float jumpSpeed)
    {
        transform.position = startPos;

        _jumpDirection = normal.normalized;
        _jumpSpeed = jumpSpeed;
    }

    public void UpdateJump(float deltaTime)
    {
        transform.position += _jumpDirection * _jumpSpeed * deltaTime;
    }
}
