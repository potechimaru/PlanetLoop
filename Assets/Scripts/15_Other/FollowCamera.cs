using UnityEngine;

/// <summary>
/// Playerを追従するカメラの制御クラス
/// </summary>
public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;        // プレイヤー
    [SerializeField] private Transform _blackHole;     // ブラックホール

    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float _smoothTime = 0f;

    [Header("Tilt Settings")]
    [SerializeField, Range(0f, 45f)]
    private float _tiltAngle = 10f; // ブラックホール方向への傾き（固定）

    private Vector3 _velocity;

    private void Update()
    {
        if (_target == null) return;

        // 位置追従
        Vector3 desiredPos = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref _velocity,
            _smoothTime
        );

        // 向き制御
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        // プレイヤーを見る方向
        Vector3 toTarget = (_target.position - transform.position).normalized;

        // ブラックホール方向
        Vector3 toBlackHole = Vector3.zero;

        if (_blackHole != null)
        {
            toBlackHole = (_blackHole.position - transform.position).normalized;
        }
        else
        {
            toBlackHole = toTarget;
        }

        // プレイヤー方向をベースに、ブラックホール方向へ少し寄せる
        Vector3 blendedDir = Vector3.RotateTowards(
            toTarget,
            toBlackHole,
            Mathf.Deg2Rad * _tiltAngle,
            0f
        );

        // 向き適用
        transform.rotation = Quaternion.LookRotation(blendedDir, Vector3.up);
    }
}