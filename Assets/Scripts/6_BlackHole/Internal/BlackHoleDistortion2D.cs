using UnityEngine;

public class BlackHoleDistortion2D : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Renderer distortionRenderer;
    [SerializeField] private Transform blackHoleCenter;

    [SerializeField] private float radius = 0.25f;
    [SerializeField] private float distortionPower = 0.08f;

    private Material _material;

    private void Awake()
    {
        _material = distortionRenderer.material;

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_material == null)
            return;

        Vector3 viewportPos = targetCamera.WorldToViewportPoint(blackHoleCenter.position);

        _material.SetVector("_Center", new Vector4(viewportPos.x, viewportPos.y, 0f, 0f));
        _material.SetFloat("_Radius", radius);
        _material.SetFloat("_DistortionPower", distortionPower);
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
            _material = null;
        }
    }
}