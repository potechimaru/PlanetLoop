using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;   // PlayerView
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -8f);
    [SerializeField] private float _smoothTime = 0.15f;

    private Vector3 _velocity;

    private void Update()
    {
        if (_target == null) return;

        Vector3 desiredPos = _target.position + _offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPos,
            ref _velocity,
            _smoothTime
        );

        transform.LookAt(_target.position);
    }
}