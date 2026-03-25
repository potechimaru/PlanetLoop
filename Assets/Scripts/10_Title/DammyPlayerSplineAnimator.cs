using UnityEngine;
using DG.Tweening;

public class DummyPlayerSplineAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ClosedSplineLine spline;
    [SerializeField] private Transform target;

    [Header("Animation")]
    [SerializeField, Min(0.1f)] private float duration = 5f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Header("Start Offset (0〜1)")]
    [SerializeField, Range(0f, 1f)] private float startOffsetNormalized = 0f;

    [Header("Options")]
    [SerializeField] private bool loop = true;
    [SerializeField] private bool lookForward = true;

    private Tween _tween;
    private float _distance;
    private float _totalLength;
    private float _startOffsetDistance;

    private void Start()
    {
        if (spline == null || target == null)
            return;

        _totalLength = spline.GetTotalLength();

        //ここで距離に変換
        _startOffsetDistance = _totalLength * startOffsetNormalized;

        // 初期位置を先に反映
        ApplyPosition(_startOffsetDistance);

        Play();
    }

    public void Play()
    {
        _tween?.Kill();

        _distance = 0f;

        _tween = DOVirtual.Float(
                0f,
                _totalLength,
                duration,
                d =>
                {
                    _distance = d;

                    //オフセットを加える
                    float finalDistance = _distance + _startOffsetDistance;

                    ApplyPosition(finalDistance);
                })
            .SetEase(ease);

        if (loop)
        {
            _tween.SetLoops(-1, LoopType.Restart);
        }
    }

    private void ApplyPosition(float distance)
    {
        Vector3 pos = spline.EvaluateByDistance(distance);
        target.position = pos;

        if (lookForward)
        {
            UpdateRotation(distance);
        }
    }

    private void UpdateRotation(float distance)
    {
        float delta = 0.01f * _totalLength;

        Vector3 p1 = spline.EvaluateByDistance(distance);
        Vector3 p2 = spline.EvaluateByDistance(distance + delta);

        Vector3 dir = (p2 - p1).normalized;

        if (dir.sqrMagnitude > 0.0001f)
        {
            target.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
    }

    public void Stop()
    {
        _tween?.Kill();
    }

    private void OnDisable()
    {
        _tween?.Kill();
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}