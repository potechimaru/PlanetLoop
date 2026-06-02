using DG.Tweening;
using UnityEngine;

public class ContinuousRotateAnimation : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform rotateTarget;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Direction")]
    [SerializeField] private bool rotateNegativeZ = true;

    private Tween _rotateTween;
    private bool _isFlipped;

    private void Awake()
    {
        if (rotateTarget == null)
            rotateTarget = transform;
    }

    private void OnEnable()
    {
        StartRotate();
    }

    private void OnDisable()
    {
        KillTween();
    }

    private void OnDestroy()
    {
        KillTween();
    }

    public void StartRotate()
    {
        KillTween();

        float direction = rotateNegativeZ ? -1f : 1f;

        _rotateTween = rotateTarget
            .DOLocalRotate(
                new Vector3(0f, 0f, 360f * direction),
                360f / rotateSpeed,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetRelative()
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void FlipY()
    {
        Debug.Log("FlipY called");

        _isFlipped = !_isFlipped;

        // âÒì]Tweení‚é~
        KillTween();

        // ZâÒì]ÉäÉZÉbÉg
        Vector3 targetEuler = rotateTarget.localEulerAngles;
        targetEuler.z = 0f;
        rotateTarget.localEulerAngles = targetEuler;

        // YîΩì]
        Vector3 euler = transform.localEulerAngles;
        euler.y = _isFlipped ? 0f : 180f;
        transform.localEulerAngles = euler;

        // âÒì]çƒäJ
        StartRotate();
    }

    private void KillTween()
    {
        if (_rotateTween != null && _rotateTween.IsActive())
            _rotateTween.Kill();

        _rotateTween = null;
    }
}