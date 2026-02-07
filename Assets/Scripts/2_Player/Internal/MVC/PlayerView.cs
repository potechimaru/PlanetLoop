using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Spline")]
    [SerializeField] private ClosedSplineLine _spline;
    [SerializeField] private bool _useLocalPlaneXY = true;

    public ClosedSplineLine Spline => _spline;
    public bool UseLocalPlaneXY => _useLocalPlaneXY;

    // --- Jump state ---
    private bool _isJumping;
    private Vector3 _jumpDirection;
    private float _jumpSpeed = 5f;

    public void SetPosition(Vector3 worldPos)
    {
        if (_isJumping)
            return;
        transform.position = worldPos;
    }

    public void StartJump(Vector3 startPos, Vector3 normal, float moveSpeed, float jumpSpeed)
    {
        transform.position = startPos;

        _jumpDirection = normal.normalized;
        _isJumping = true;
        _jumpSpeed = jumpSpeed;
    }

    public void Tick()
    {
        if (!_isJumping)
            return;

        transform.position += _jumpDirection * _jumpSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 外部からジャンプ終了させる用（再吸着時など）
    /// </summary>
    public void EndJump()
    {
        _isJumping = false;
    }
}
